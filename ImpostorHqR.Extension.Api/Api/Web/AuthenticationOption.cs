using System;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public readonly struct WebPageAuthenticationOption
    {
        public string User { get; }

        public string Password { get; }

        public WebPageAuthenticationOption(string user, string password)
        {
            if(string.IsNullOrEmpty(user)) throw new ArgumentException("user can't be null.");
            if(string.IsNullOrEmpty(password)) throw new ArgumentException("password can't be null.");

            this.User = user;
            this.Password = password;
        }
    }
}
