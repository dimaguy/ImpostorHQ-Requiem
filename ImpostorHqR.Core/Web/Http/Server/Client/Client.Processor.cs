using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Core.Web.Http.Server.Request.Fields;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Http.Server.Response.Mime;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public static class HttpClientProcessor
    {
        private static readonly SemaphoreSlim ResourcePool = new SemaphoreSlim(ConfigHolder.Instance.ResourcePoolSize, ConfigHolder.Instance.ResourcePoolSize);

        private static ArrayPool<byte> Pool = ArrayPool<byte>.Shared;

        public static int ConcurrentFileTransfers => (ConfigHolder.Instance.ResourcePoolSize - ResourcePool.CurrentCount);

        public static async Task ProcessClient(HttpClientHolder holder, HttpInitialRequest request)
        {
            var handler = HttpHandleStore.Instance.Check(request.Path);
            if (handler == null)
            {
                request.Path = Path.Combine(Directory.GetCurrentDirectory(), HttpConstant.RootDirectory, request.Path.Replace("/", "\\"));
                if (!File.Exists(request.Path))
                {
                    await holder.SafeWriteAsync(HttpErrorResponses.Instance.NotFoundResponse);
                    return;
                }
            }
            else
            {
                handler.Invoked?.Invoke(holder);
                return;
            }

            var mime = MimeProcessor.Interpret(request.Path);
            if (mime == null)
            {
                await holder.SafeWriteAsync(HttpErrorResponses.Instance.NotImplementedResponse);
                return;
            }
            if (request.Method == HttpInitialRequestMethod.HEAD || !holder.Connected) return;

            var fileInfo = new FileInfo(request.Path);
            holder.Client.SendBufferSize = HttpConstant.FileBufferSize;
            await using var fs = new FileStream(request.Path, FileMode.Open,FileAccess.Read, FileShare.Read, 4096, useAsync: fileInfo.Length > 1024 * 1024 ? true : false);

            if (request.Ranges == null || request.Ranges.Count == 0)
            {
                var header = new HttpResponseHeaders((int)fileInfo.Length, ResponseStatusCode.Ok200, new IResponseField[]
                {
                    new FieldServer(HttpConstant.ServerName),
                    new FieldContentType(mime),
                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                }, request.HttpVersion);

                var headerData = header.Compile();
                await holder.SafeWriteAsync(headerData);
                if (!holder.Connected) return;
                await SendSubRoutine(fileInfo, fs, holder, fileInfo.Length);

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

                            var header = new HttpResponseHeaders((int)fileInfo.Length, ResponseStatusCode.PartialContent206, new IResponseField[]
                            {
                                new FieldContentRange(true, range.Range.From, fileInfo.Length-1, fileInfo.Length),
                                new FieldServer(HttpConstant.ServerName),
                                new FieldContentType(mime),
                                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                            }, request.HttpVersion);

                            var headerData = header.Compile();
                            await holder.SafeWriteAsync(headerData);
                            if (!holder.Connected) return;
                            fs.Seek((long)range.Range.From, SeekOrigin.Begin);
                            await SendSubRoutine(fileInfo, fs, holder, (int)(fs.Length - range.Range.From));
                            return;
                        }

                    case HttpRangeRequestMethod.SendAll:
                        {
                            // implemented, tested
                            var header = new HttpResponseHeaders((int)fileInfo.Length, ResponseStatusCode.PartialContent206, new IResponseField[]
                            {
                                new FieldContentRange(true,0,fileInfo.Length-1, fileInfo.Length),
                                new FieldServer(HttpConstant.ServerName),
                                new FieldContentType(mime),
                                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                            }, request.HttpVersion);

                            var headerData = header.Compile();
                            await holder.SafeWriteAsync(headerData);
                            if (!holder.Connected) return;
                            await SendSubRoutine(fileInfo, fs, holder, (int)fs.Length);
                            return;
                        }
                    case HttpRangeRequestMethod.SliceFromTo:
                        {
                            if (fs.Length < range.Range.From || fs.Length < range.Range.To)
                            {
                                // attacker.
                                return;
                            }

                            var header = new HttpResponseHeaders((int)fileInfo.Length, ResponseStatusCode.PartialContent206, new IResponseField[]
                            {
                                new FieldContentRange(true,range.Range.From,range.Range.To,fileInfo.Length),
                                new FieldServer(HttpConstant.ServerName),
                                new FieldContentType(mime),
                                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                            }, request.HttpVersion);

                            var headerData = header.Compile();
                            await holder.SafeWriteAsync(headerData);
                            if (!holder.Connected) return;
                            Debug.Assert(range.Range.From != null, "range.Range.From != null");
                            fs.Seek((int)range.Range.From, SeekOrigin.Begin);

                            await SendSubRoutine(fileInfo, fs, holder, (long)(range.Range.To - range.Range.From));

                            return;
                        }
                    case HttpRangeRequestMethod.SliceFromStartTo:
                        {

                            if (fs.Length < range.Range.To)
                            {
                                // attacker.
                                return;
                            }

                            var header = new HttpResponseHeaders((int)fileInfo.Length, ResponseStatusCode.PartialContent206, new IResponseField[]
                            {
                                new FieldContentRange(true,0,range.Range.To, fileInfo.Length),
                                new FieldServer(HttpConstant.ServerName),
                                new FieldContentType(mime),
                                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                            }, request.HttpVersion);

                            var headerData = header.Compile();
                            await holder.SafeWriteAsync(headerData);
                            if (!holder.Connected) return;
                            if (range.Range.To != null) await SendSubRoutine(fileInfo, fs, holder, (long)range.Range.To);
                            return;
                        }
                    default:
                        return; // ??
                }
            }
        }

        public static int TransferRateKBytes = 0;

        private static async Task SendSubRoutine(FileInfo fileInfo, Stream fs, HttpClientHolder holder, long toSendTotal)
        {
            if (!await ResourcePool.WaitAsync(ConfigHolder.Instance.ResourcePoolTimeout)) return;
            var bufferSize = (int) Math.Min(fileInfo.Length, ConfigHolder.Instance.FileIoSize);
            var buffer = Pool.Rent(bufferSize);
            long totalRead = 0;
            try
            {
                while (totalRead != toSendTotal)
                {
                    var toRead = (int) Math.Min(fileInfo.Length - totalRead, bufferSize);
                    var read = await fs.ReadAsync(buffer.AsMemory(0, toRead));
                    totalRead += read;

                    var result = await holder.SafeWriteAsync(buffer, read);

                    if(!result) return;

                    Interlocked.Add(ref TransferRateKBytes, read / 1024);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogging.Instance.LogError($"Unhandled exception in send subroutine : {ex}", null, true);
            }
            finally
            {
                ResourcePool.Release();
                Pool.Return(buffer);
            }
        }
    }
}
