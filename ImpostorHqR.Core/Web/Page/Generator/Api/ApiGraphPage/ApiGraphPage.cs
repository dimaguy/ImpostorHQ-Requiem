using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer;
using ImpostorHqR.Extension.Api.Api.Web;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiGraphPage : IGraphPage
    {
        private ApiGraph[] Graphs { get; }

        public string ApiHandle { get; }

        public string Title { get; }

        public string Code { get; }

        private ApiHandleHolder Api { get; }

        private byte[] Stream { get; }

        public ApiGraphPage(string handle, string title, ApiGraph[] graphs, byte width, WebPageAuthenticationOption? creds = null)
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
                IConfigurationStore.GetByType<RequiemConfig>().ApiPort,
                handle,
                declareVariablesInScript.ToArray(),
                graphCodeStr.ToArray(),
                graphCodeHandles.ToArray());

            this.Code = GraphPageSplicer.SpliceFinalHtml(script, title, graphCodeHandles.ToArray(), width);
            using var ms = new MemoryStream();
            if (creds == null)
            {
                using var headers = new HttpResponseHeaders(Code.Length, ResponseStatusCode.Ok200, new IResponseField[]
                {
                    new FieldAcceptRanges("bytes"),
                    new FieldContentType("text/html"),
                    new FieldServer(HttpConstant.ServerName)
                }, "HTTP/1.1");
                this.Header = headers.Compile();
                ms.Write(Header.Item1, 0, Header.Item2);
                ms.Write(Encoding.UTF8.GetBytes(this.Code));
                this.Stream = ms.ToArray();
                var httpHandle = new SpecialHandler(handle, async (ctx) => await ctx.SafeWriteAsync(Stream));
                HttpHandleStore.AddHandler(httpHandle);
                this.Api = new ApiHandleHolder(handle);
                WebApiHandleStore.Add(this.Api);
            }
            else
            {
                using var headers = new HttpResponseHeaders(Code.Length, ResponseStatusCode.Ok200, new IResponseField[]
                {
                    new FieldAcceptRanges("bytes"),
                    new FieldContentType("text/html"),
                    new FieldServer(HttpConstant.ServerName),
                    new FieldAuthentication("Basic", "New Lantea"), 
                }, "HTTP/1.1");
                this.Header = headers.Compile();
                ms.Write(Header.Item1, 0, Header.Item2);
                ms.Write(Encoding.UTF8.GetBytes(this.Code));
                this.Stream = ms.ToArray();
                var httpHandle = new SpecialHandler(handle, async (ctx) => await ctx.SafeWriteAsync(Stream), new SpecialHandler.HttpAuthOptions(creds.Value.User, creds.Value.Password));
                HttpHandleStore.AddHandler(httpHandle);
                this.Api = new ApiHandleHolder(handle);
                WebApiHandleStore.Add(this.Api);
            }
        }

        private (byte[], int) Header { get; }

        public void Update()
        {
            Api.Push(new ApiGraphPageUpdateMessage(this.Graphs));
        }

        public IEnumerable<IGraph> GetGraphs()
        {
            return Graphs.AsEnumerable();
        }
    }
}
