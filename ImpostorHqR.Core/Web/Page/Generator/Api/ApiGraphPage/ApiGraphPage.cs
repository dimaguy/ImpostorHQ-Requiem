using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiGraphPage : IGraphPage
    {
        private ApiPageElementGraph[] Graphs { get; }

        public string ApiHandle { get; }

        public string Title { get; }

        public string Code { get; }

        private byte[] CodeBytes { get; }

        private ApiHandleHolder Api { get; }

        public ApiGraphPage(string handle, string title, ApiPageElementGraph[] graphs, byte width)
        {
            this.ApiHandle = handle;
            this.Title = title;
            this.Graphs = graphs;

            var graphCodeStr = new List<string>();
            var graphCodeHandles = new List<string>();
            var declareVariablesInScript = new List<string>();
            foreach (var graph in graphs)
            {
                graphCodeStr.Add(graph.Code);
                graphCodeHandles.Add(graph.GraphCtx);
                declareVariablesInScript.Add(graph.Variable);
            }

            var script = GraphPageSplicer.SpliceMainScript(
                ConfigHolder.Instance.ApiPort,
                handle,
                declareVariablesInScript.ToArray(),
                graphCodeStr.ToArray(),
                graphCodeHandles.ToArray());

            this.Code = GraphPageSplicer.SpliceFinalHtml(script, title, graphCodeHandles.ToArray(), width);
            this.CodeBytes = Encoding.UTF8.GetBytes(this.Code);

            var httpHandle = new SpecialHandler(handle, ServePage);
            HttpHandleStore.Instance.AddHandler(httpHandle);

            this.Api = new ApiHandleHolder(handle);
            WebApiHandleStore.Instance.Add(this.Api);
        }

        public void Update()
        {
            lock (Graphs)
            {
                Api.Push(new ApiGraphPageUpdateMessage(this.Graphs));
            }
        }

        private async void ServePage(HttpClientHolder obj)
        {
            var headers = new HttpResponseHeaders(this.CodeBytes.Length, ResponseStatusCode.Ok200, new IResponseField[]
            {
                new FieldAcceptRanges("bytes"),
                new FieldContentType("text/html"),
                new FieldServer(HttpConstant.ServerName + " over " + this.Title)
            }, "HTTP/1.1");

            await obj.SafeWriteAsync(headers.Compile());
            await obj.SafeWriteAsync(this.CodeBytes);
        }

        IEnumerable<IGraph> IGraphPage.GetGraphs()
        {
            return Graphs.AsEnumerable();
        }
    }
}
