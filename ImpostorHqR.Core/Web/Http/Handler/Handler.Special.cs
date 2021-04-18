using System;
using ImpostorHqR.Core.Web.Http.Server.Client;

namespace ImpostorHqR.Core.Web.Http.Handler
{
    public class SpecialHandler
    {
        public string Path { get; set; }

        public Action<HttpClientHolder> Invoked { get; private set; }

        public HttpAuthOptions? Options { get; private set; }

        public SpecialHandler(string path, Action<HttpClientHolder> invoker)
        {
            Construct(path, invoker);
        }

        public SpecialHandler(string path, Action<HttpClientHolder> invoker, HttpAuthOptions options)
        {
            Construct(path, invoker);
            this.Options = options;
        }

        private void Construct(string path, Action<HttpClientHolder> invoker)
        {
            if (path.Contains('.')) throw new Exception("Special handlers can only contain folder names.");

            this.Path = path;
            this.Invoked = invoker;

            if (!this.Path.StartsWith('/')) this.Path = string.Concat("/", this.Path);
            if (!this.Path.EndsWith("/")) this.Path += '/';
            this.Options = default;
        }

        public readonly struct HttpAuthOptions
        {
            public string User { get; }
            public string Password { get; }

            public HttpAuthOptions(string user, string password)
            {
                if (string.IsNullOrEmpty(user)) throw new ArgumentException("user can't be null.");
                if (string.IsNullOrEmpty(password)) throw new ArgumentException("password can't be null.");

                this.User = user;
                this.Password = password;
            }
        }
    }
}
