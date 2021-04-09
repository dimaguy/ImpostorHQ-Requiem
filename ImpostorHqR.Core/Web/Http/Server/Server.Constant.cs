using System;
using System.Text;

namespace ImpostorHqR.Core.Web.Http.Server
{
    public static class HttpConstant
    {
        public const string RootDirectory = "ImpostorHq.Web";

        public const string NotImplemented501 = "<h1> Seems like you hit the wrong button. </h1>";

        public const int FileBufferSize = 1024 * 8;

        public const int ReadTimeout = 5000;

        public const string ServerName = "ChrisHTTP(S) Rage, rage against the dying of the light.";

        public static readonly string NotFound404Message = Encoding.UTF8.GetString(Convert.FromBase64String(@"PGh0bWw+Cgk8aGVhZD4KCQk8c3R5bGU+CgkJCWJvZHkgewoJCQkJYmFja2dyb3VuZC1jb2xvcjogcmdiKDI0LCAyNiwgMjcpOwoJCQl9CgkJCWgxIHsKCQkJCXRleHQtYWxpZ246IGNlbnRlcjsKCQkJfQoJCTwvc3R5bGU+CgkJCgkJCgk8L2hlYWQ+Cgk8Ym9keT4KCQk8aDE+PHAgc3R5bGUgPSAiY29sb3I6cmVkIj5Pd08gd2hhdCdzIHRoaXM/PGJyPlNlZW1zIGxpa2UgeW91IGdvdCB0aGUgd3JvbmcgbGluay48L3A+PC9oMT4KCTwvYm9keT4KPC9odG1sPgo="));

        public const string AcceptRanges = "bytes";

        public const string ServerOverloadMessage = "<h1> The server is choking, please try again later. </h1>";
    }
}
