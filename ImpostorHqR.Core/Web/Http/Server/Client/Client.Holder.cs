using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Server.Request;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Web.Http.Server.Client
{
    public readonly struct HttpClientHolder : IDisposable
    {
        public Socket Client { get; }

        public Stream Transport { get; }

        public HttpInitialRequest Request { get; }

        public HttpClientHolder(Stream transport, Socket client, HttpInitialRequest request)
        {
            this.Client = client;
            this.Transport = transport;
            this.Request = request;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<bool> SafeWriteAsync(byte[] data, int length = 0)
        {
            if (!Transport.CanWrite || !Client.Connected)
            {
                return false;
            }

            try
            {
                var len = length == 0 ? data.Length : length;
                await Transport?.WriteAsync(data, 0, len);
                Interlocked.Add(ref HttpClientProcessor.TransferRateKBytes, (int) (len / 1024 / 1024));
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return false;
            }
        }

        private void LogEx(Exception ex) => ILogManager.Log("Error in HTTP Client SafeWrite.", "Client.Holder", LogType.Error, ex: ex);

        public void Dispose() => Close(Client);

        public static void Close(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                //ignore
            }
            socket.Close();
        }
    }
}
