using System.Drawing;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage
{
    public class ApiPageElementGraph : IGraph
    {
        private static int _varIndex = 0;

        private static readonly object Sync = new object();

        public string Code { get; }

        public string Variable { get; }

        public string GraphCtx { get; }

        public long Value { get; set; }

        public uint TimeSpan { get; }

        public string Name { get; }

        public Color FillColor { get; }

        public Color LineColor { get; }

        public uint Delay { get; }

        public ApiPageElementGraph(string title, Color backgroundColor, Color borderColor, uint delay, uint span)
        {
            this.Name = title;
            this.TimeSpan = span;
            this.FillColor = backgroundColor;
            this.LineColor = borderColor;
            this.Delay = delay;

            string graphValueName = "graphValue";
            string graphHandleName = "gHtml";
            lock (Sync)
            {
                graphValueName += _varIndex;
                graphHandleName += _varIndex++;
            }
            this.Variable = graphValueName;
            this.GraphCtx = graphHandleName;
            this.Code = GraphPageSplicer.SpliceGraph(
                graphValueName,
                graphHandleName,
                graphHandleName + "_create",
                title,
                delay,
                span,
                backgroundColor,
                borderColor);
        }

        public void Update(long val) => Value = val;
    }
}
