using System;
using System.Linq;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiGraphPageProvider : IGraphPageProvider
    {
        public static readonly IGraphPageProvider Instance = new ApiGraphPageProvider();

        public IGraphPage Create(IGraph[] graphs, string title, string handle, byte width)
        {
            if (width < 10 || width > 100) throw new Exception($"Extension tried to register page [{title}] with a width of {width}. Only a width between 10 - 100 is allowed.");

            return new ApiGraphPage(handle, title, graphs.Select(item => item as ApiPageElementGraph).ToArray(), width);
        }
    }
}
