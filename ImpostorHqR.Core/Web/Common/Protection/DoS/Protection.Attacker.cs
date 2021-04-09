using System;
using System.Net;

namespace ImpostorHqR.Core.Web.Common.Protection.DoS
{
    public class ServerAccessingEntity : IDisposable
    {
        public IPAddress Address { get; }
        public DateTime TimeLastAccess { get; set; }

        public int Requests = 0;

        public ServerAccessingEntity(IPAddress address)
        {
            this.Address = address;
        }

        public void Dispose()
        {

        }
    }
}
