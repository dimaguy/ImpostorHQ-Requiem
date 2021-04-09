using System;
using ImpostorHqR.Core.Web.Http.Server.Client;

namespace ImpostorHqR.Core.Web.Http.Handler
{
    public class SpecialHandler
    {
        public string Path { get; set; }

        public Action<HttpClientHolder> Invoked { get; }

        public SpecialHandler(string path, Action<HttpClientHolder> invoker)
        {
            if (path.Contains('.')) throw new Exception("Special handlers can only contain folder names.");

            this.Path = path;
            this.Invoked = invoker;

            if (!this.Path.StartsWith('/')) this.Path = string.Concat("/", this.Path);
            if (!this.Path.EndsWith("/")) this.Path += '/';
        }
    }
}
