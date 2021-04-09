using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiGraphPageElementProvider : IGraphProvider
    {
        public static readonly IGraphProvider Instance = new ApiGraphPageElementProvider();

        public IGraph Create(string title, Color fillColor, Color lineColor, uint span, uint delay)
        {
            return new ApiPageElementGraph(title, fillColor, lineColor, delay, span);
        }
    }
}
