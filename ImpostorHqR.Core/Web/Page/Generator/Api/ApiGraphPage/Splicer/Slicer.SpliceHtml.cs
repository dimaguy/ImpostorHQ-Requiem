using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer
{
    public static partial class GraphPageSplicer
    {
        public static string SpliceFinalHtml(string script, string title, string[] graphNames, byte width)
        {
            var sb = StringBuilderPool.Pool.Get();

            foreach (var graphName in graphNames)
            {
                sb.AppendLine();
                sb.AppendLine("<div class=\"chart-container\">");
                sb.AppendLine($"    <canvas id=\"{graphName}\"></canvas>");
                sb.AppendLine("</div>");
                sb.AppendLine();
            }

            var declarations = sb.ToString();

            StringBuilderPool.Pool.Return(sb);

            return GraphPageSplicerConstant.Html
                .Replace(GraphPageSplicerConstant.ReplaceInScript, script)
                .Replace(GraphPageSplicerConstant.ReplaceInTitle, title)
                .Replace(GraphPageSplicerConstant.ReplaceInHtmlDeclarations, declarations)
                .Replace(GraphPageSplicerConstant.ReplaceInWidth, width.ToString());
        }
    }
}
