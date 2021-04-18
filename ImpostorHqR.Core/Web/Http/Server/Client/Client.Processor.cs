using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Core.Web.Http.Server.Request.Fields;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Http.Server.Response.Mime;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public static class HttpClientProcessor
    {
        private static RequiemConfig Cfg => IConfigurationStore.GetByType<RequiemConfig>();

        private static readonly SemaphoreSlim ResourcePool = new SemaphoreSlim(Cfg.ResourcePoolSize, Cfg.ResourcePoolSize);

        private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;

        public static int ConcurrentFileTransfers => (Cfg.ResourcePoolSize - ResourcePool.CurrentCount);

        private static readonly ConcurrentDictionary<(string,IPAddress), int> AuthenticationHistory  = new ConcurrentDictionary<(string, IPAddress), int>();

        public static bool IsAuthorized(string handle, IPAddress address)
        {
            if (!AuthenticationHistory.TryGetValue((handle, address), out _)) return false;
                AuthenticationHistory.TryRemove((handle, address), out _);
            return true;
        }

        public static void Initialize()
        {
            var tmr = new System.Timers.Timer(1000) {AutoReset = true};
            tmr.Elapsed += UpdateAuthorizations;
            tmr.Start();
        }

        private static void UpdateAuthorizations(object sender, ElapsedEventArgs e)
        {
            foreach (var kvp in AuthenticationHistory)
            {
                if (kvp.Value > Cfg.ApiAuthTimeoutSeconds) AuthenticationHistory.TryRemove(kvp);
                else AuthenticationHistory[kvp.Key]++;
            }
        }
        public static async ValueTask ProcessClient(HttpClientHolder holder, HttpInitialRequest request)
        {
            var handler = HttpHandleStore.Check(request.Path);
            if (handler == null)
            {
                request.Path = Path.Combine(Directory.GetCurrentDirectory(), HttpConstant.RootDirectory, request.Path);
                if (!File.Exists(request.Path))
                {
                    await holder.SafeWriteAsync(HttpErrorResponses.NotFoundResponse);
                    return;
                }
            }
            else
            {
                if (handler.Options == null)
                {
                    goto Server;
                }
                if (!request.Credentials.HasValue ||
                    !request.Credentials.Value.Password.Equals(handler.Options.Value.Password) ||
                    !request.Credentials.Value.User.Equals(handler.Options.Value.User))
                {
                    await holder.SafeWriteAsync(HttpErrorResponses.NotAuthorizedResponse);
                    return;
                }

                Server:
                {
                    Authorize();
                    handler.Invoked?.Invoke(holder);
                    return;
                }

                void Authorize()
                {
                    Trace.Assert(holder.Client.RemoteEndPoint != null, "Scary: IPEndPoint was null.");
                    var ipa = ((IPEndPoint) holder.Client.RemoteEndPoint).Address.MapToIPv6();
                    AuthenticationHistory.AddOrUpdate((handler.Path,ipa), 0, (_,__) => 0);
                }
            }

            var mime = MimeProcessor.Interpret(request.Path);
            if (mime == null)
            {
                await holder.SafeWriteAsync(HttpErrorResponses.NotImplementedResponse);
                return;
            }

            var fileInfo = new FileInfo(request.Path);
            holder.Client.SendBufferSize = HttpConstant.FileBufferSize;
            var asyncFs = fileInfo.Length > 1024 * 1024;
            Stream fs;

            if (Cfg.EnableHttpCache && !Cfg.HttpCacheExclude.Contains(request.Path))
            {
                var (stat, array, length) = HttpServerFileCache.TryGet(request.Path);
                if (stat)
                {
                    Trace.Assert(array != null, "Http cache null L58/Client.Processor!");
                    fs = new MemoryStream(array, 0, length.Value);
                    asyncFs = false;
                }
                else
                {
                    var (success, bytes, stream) = await HttpServerFileCache.TryCache(request.Path, asyncFs);
                    if (success)
                    {
                        fs = new MemoryStream(bytes, 0, (int) fileInfo.Length);
                        asyncFs = false;
                    }
                    else
                    {
                        fs = stream;
                    }
                }
            }
            else
            { 
                fs = new FileStream(request.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                    useAsync: asyncFs);
            }

            await using (fs)
            {
                if (request.Ranges == null || request.Ranges.Count == 0)
                {
                    using var header = new HttpResponseHeaders(fileInfo.Length, ResponseStatusCode.Ok200,
                        new IResponseField[]
                        {
                            new FieldServer(HttpConstant.ServerName),
                            new FieldContentType(mime),
                            new FieldAcceptRanges(HttpConstant.AcceptRanges),
                        }, request.HttpVersion);

                    var headerData = header.Compile();

                    if (!await holder.SafeWriteAsync(headerData.Item1, headerData.Item2)) return;
                    if (request.Method == HttpInitialRequestMethod.HEAD) return;

                    await CopyStream(fileInfo, fs, holder, fileInfo.Length, asyncFs);
                    return;
                }

                foreach (var range in request.Ranges)
                {
                    switch (range.Method)
                    {
                        case HttpRangeRequestMethod.SliceFromToEnd:
                        {
                            // implemented, tested
                            if (fs.Length < range.Range.From) return;

                            using var header = new HttpResponseHeaders(fileInfo.Length,
                                ResponseStatusCode.PartialContent206, new IResponseField[]
                                {
                                    new FieldContentRange(true, range.Range.From, fileInfo.Length - 1, fileInfo.Length),
                                    new FieldServer(HttpConstant.ServerName),
                                    new FieldContentType(mime),
                                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                                }, request.HttpVersion);

                            var headerData = header.Compile();
                            if (!await holder.SafeWriteAsync(headerData.Item1, headerData.Item2)) return;
                            if(request.Method == HttpInitialRequestMethod.HEAD) return;
                            fs.Seek((long) range.Range.From!, SeekOrigin.Begin);
                            await CopyStream(fileInfo, fs, holder, (int) (fs.Length - range.Range.From), asyncFs);
                            return;
                        }

                        case HttpRangeRequestMethod.SendAll:
                        {
                            // implemented, tested
                            using var header = new HttpResponseHeaders(fileInfo.Length,
                                ResponseStatusCode.PartialContent206, new IResponseField[]
                                {
                                    new FieldContentRange(true, 0, fileInfo.Length - 1, fileInfo.Length),
                                    new FieldServer(HttpConstant.ServerName),
                                    new FieldContentType(mime),
                                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                                }, request.HttpVersion);

                            var headerData = header.Compile();
                            if (!await holder.SafeWriteAsync(headerData.Item1, headerData.Item2)) return;
                            if (request.Method == HttpInitialRequestMethod.HEAD) return;
                            await CopyStream(fileInfo, fs, holder, (int) fs.Length, asyncFs);
                            return;
                        }
                        case HttpRangeRequestMethod.SliceFromTo:
                        {
                            if (fs.Length < range.Range.From || fs.Length < range.Range.To)
                            {
                                // attacker.
                                return;
                            }

                            using var header = new HttpResponseHeaders(fileInfo.Length,
                                ResponseStatusCode.PartialContent206, new IResponseField[]
                                {
                                    new FieldContentRange(true, range.Range.From, range.Range.To, fileInfo.Length),
                                    new FieldServer(HttpConstant.ServerName),
                                    new FieldContentType(mime),
                                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                                }, request.HttpVersion);

                            var headerData = header.Compile();

                            if (!await holder.SafeWriteAsync(headerData.Item1, headerData.Item2)) return;
                            if (request.Method == HttpInitialRequestMethod.HEAD) return;
                            fs.Seek((int) range.Range.From, SeekOrigin.Begin);
                            await CopyStream(fileInfo, fs, holder, (long) (range.Range.To - range.Range.From)!,
                                asyncFs);
                            return;
                        }
                        case HttpRangeRequestMethod.SliceFromStartTo:
                        {
                            if (fs.Length < range.Range.To)
                            {
                                // attacker.
                                return;
                            }

                            using var header = new HttpResponseHeaders(fileInfo.Length,
                                ResponseStatusCode.PartialContent206, new IResponseField[]
                                {
                                    new FieldContentRange(true, 0, range.Range.To, fileInfo.Length),
                                    new FieldServer(HttpConstant.ServerName),
                                    new FieldContentType(mime),
                                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                                }, request.HttpVersion);

                            var headerData = header.Compile();
                            if (!await holder.SafeWriteAsync(headerData.Item1, headerData.Item2) ||
                                range.Range.To == null) return;
                            if (request.Method == HttpInitialRequestMethod.HEAD) return;
                            await CopyStream(fileInfo, fs, holder, (long) range.Range.To, asyncFs);
                            return;
                        }
                        default:
                            return; // ??
                    }
                }
            }
        }

        public static int TransferRateKBytes = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask CopyStream(FileInfo fileInfo, Stream fs, HttpClientHolder holder, long toSendTotal, bool async)
        {
            if (!await ResourcePool.WaitAsync(Cfg.ResourcePoolTimeout)) return;
            var bufferSize = (int) Math.Min(fileInfo.Length, Cfg.FileIoSize);
            var buffer = Pool.Rent(bufferSize);
            long totalRead = 0;
            try
            {
                while (totalRead != toSendTotal)
                {
                    var toRead = (int) Math.Min(fileInfo.Length - totalRead, bufferSize);
                    var mem = buffer.AsMemory(0, toRead);
                    var read = async ? await fs.ReadAsync(mem) : fs.Read(mem.Span);
                    totalRead += read;

                    if(!await holder.SafeWriteAsync(buffer, read)) return;

                    Interlocked.Add(ref TransferRateKBytes, read / 1024);
                }
            }
            catch (Exception ex)
            {
                ILogManager.Log("Unhandled exception in Client.Processor send.","Client.Processor.CopyStream",LogType.Error,ex:ex);
            }
            finally
            {
                ResourcePool.Release();
                Pool.Return(buffer);
            }
        }
    }
}
