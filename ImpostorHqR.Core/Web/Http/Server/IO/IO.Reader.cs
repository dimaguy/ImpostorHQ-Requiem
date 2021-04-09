using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Web.Http.Server.IO
{
    public static class HttpLineReader
    {
        private static readonly byte[] CrLfx2 = new byte[] { 13, 10, 13, 10 };
        private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<string> ReadLineSizedBuffered(Stream stream, ushort length = 128)
        {
            stream.ReadTimeout = HttpConstant.ReadTimeout;
            var buffer = Pool.Rent(length);
            using var sb = StringBuilderPool.Instance.Get();
            var total = 0;
            try
            {
                while (total < length)
                {
                    if (!stream.CanRead)
                    {
                        return sb.StringBuilder.Length > 0 ? sb.ToString() : null;
                    }
                    var read = await stream.ReadAsync(buffer, 0, length);
                    if (read == 0) return sb.ToString();

                    total += read;

                    if (ProcessChunks(buffer, length, read, sb)) return sb.ToString();
                }

                return sb.ToString();
            }
            catch (SocketException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log(new LogEntry()
                {
                    Message = $"Http line read error: {ex.ToString()}",
                    Type = LogType.Error
                });
                return null;
            }
            finally
            {
                Pool.Return(buffer);
            }
        }

        private static bool ProcessChunks(byte[] buffer, int length, int read, IReusableStringBuilder sb)
        {
            Span<char> chars = stackalloc char[length];
            var data = buffer.AsSpan().Slice(0, read);
            if (data.Slice(read - 4).SequenceEqual(CrLfx2))
            {
                Encoding.UTF8.GetChars(data, chars);
                sb.Append(chars.Slice(0, read - 4));
                return true;
            }
            Encoding.UTF8.GetChars(data, chars);
            sb.Append(chars.Slice(0, read));
            return false;
        }
    }
}
