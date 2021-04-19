using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses
{
    public class UnauthorizedResponse : IHqAuthResponse
    {
        public string Serialize() => "401 Unauthorized.";
    }
}
