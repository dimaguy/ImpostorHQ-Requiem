using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImpostorHqR.Core.Web.Api.WebSockets;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public readonly struct ApiGraphPageUpdateMessage : IHqApiOutgoingMessage
    {
        public IEnumerable<ApiGraph> Values { get; }

        public ApiGraphPageUpdateMessage(IEnumerable<ApiGraph> values)
        {
            this.Values = values;
        }

        public string Serialize()
        {
            using var sb = IReusableStringBuilder.Get();
            sb.AppendLine("{");
            foreach (var value in Values)
            {
                sb.AppendLine($"\"{value.Variable}\": {value.Value}{(value == Values.Last() ? "" : ",")}");
            }
            sb.AppendLine("}");
            var data = sb.ToString();
            return data;
        }
    }
}
