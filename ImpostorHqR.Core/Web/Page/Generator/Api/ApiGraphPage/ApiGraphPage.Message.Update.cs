using System.Linq;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Core.Web.Api.WebSockets;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiGraphPageUpdateMessage : IHqApiOutgoingMessage
    {
        public ApiPageElementGraph[] Values { get; }

        public ApiGraphPageUpdateMessage(ApiPageElementGraph[] values)
        {
            this.Values = values;
        }

        public string Serialize()
        {
            var sb = StringBuilderPool.Pool.Get();
            sb.AppendLine("{");

            foreach (var value in Values)
            {
                sb.AppendLine($"\"{value.Variable}\": {value.Value}{(value == Values.Last() ? "" : ",")}");
            }

            sb.AppendLine("}");

            var data = sb.ToString();

            StringBuilderPool.Pool.Return(sb);

            return data;
        }
    }
}
