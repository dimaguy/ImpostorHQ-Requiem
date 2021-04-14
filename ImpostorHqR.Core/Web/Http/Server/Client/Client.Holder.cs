using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public struct HttpClientHolder
    {
        public bool Connected { get; private set; }

        public Socket Client { get; }

        public Stream Transport { get; }

        public HttpInitialRequest Request { get; }

        public HttpClientHolder(Stream transport, Socket client, HttpInitialRequest request)
        {
            this.Connected = true;
            this.Client = client;
            this.Transport = transport;
            this.Request = request;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<bool> SafeWriteAsync(byte[] data, int length = 0)
        {
            if (!Transport.CanWrite || !Client.Connected)
            {
                this.Connected = false;
                return false;
            }

            try
            {
                var len = length == 0 ? data.Length : length;
                await Transport?.WriteAsync(data, 0, len);
                await Transport.FlushAsync();
                Interlocked.Add(ref HttpClientProcessor.TransferRateKBytes, len / 1024);
                return true;
            }
            catch (IOException)
            {
                this.Connected = false;
                return false;
            }
            catch (Exception ex)
            {
                this.Connected = false;
                LogEx(ex);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SafeWrite(Span<byte> data)
        {
            if (!Transport.CanWrite || !Client.Connected)
            {
                this.Connected = false;
                return;
            }

            try
            {
                Transport?.Write(data);
                Interlocked.Add(ref HttpClientProcessor.TransferRateKBytes, data.Length / 1024);
            }
            catch (IOException)
            {
                this.Connected = false;
            }
            catch (Exception ex)
            {
                this.Connected = false;
                LogEx(ex);
            }
        }

        private void LogEx(Exception ex)
        {
            LogManager.Instance.Log(new LogEntry()
            {
                Message = ex.ToString(),
                Source = this,
                Type = LogType.Error
            });
        }
    }
}
