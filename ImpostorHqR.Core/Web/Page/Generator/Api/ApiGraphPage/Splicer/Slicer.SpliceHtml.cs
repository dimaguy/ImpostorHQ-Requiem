
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer
{
    public static partial class GraphPageSplicer
    {
        public static string SpliceFinalHtml(string script, string title, string[] graphNames, byte width)
        {
            using var sb = IReusableStringBuilder.Get();

            foreach (var graphName in graphNames)
            {
                sb.Append("\r\n");
                sb.AppendLine("<div class=\"chart-container\">");
                sb.AppendLine($"    <canvas id=\"{graphName}\"></canvas>");
                sb.AppendLine("</div>");
                sb.AppendLine("\r\n");
            }

            var declarations = sb.ToString();

            return GraphPageSplicerConstant.Html
                .Replace(GraphPageSplicerConstant.ReplaceInScript, script)
                .Replace(GraphPageSplicerConstant.ReplaceInTitle, title)
                .Replace(GraphPageSplicerConstant.ReplaceInHtmlDeclarations, declarations)
                .Replace(GraphPageSplicerConstant.ReplaceInWidth, width.ToString());
        }
    }
}
