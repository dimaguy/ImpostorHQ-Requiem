using System.Drawing;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer
{
    public static partial class GraphPageSplicer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueVarName">The variable declared in the outer scope that is also set by the receive function, extracted from the json object.</param>
        /// <param name="ctxName">The variable declared in the outer scope, linked to the HTML declaration of the graph's container.</param>
        /// <param name="declareName">The name to declare the variable. Should not matter much, but must be valid so the JS interpreter accepts it.</param>
        /// <param name="title">The visible text above the graph.</param>
        /// <param name="delay">The delay of the graph. If it is higher, the data can be interpolated more smoothly. Thus, a delay of 2-5 usually does the job.</param>
        /// <param name="period">The time period to display. E.G if set to 10, it will display data from the last 10 seconds.</param>
        /// <param name="bgColor">The background color for the graph. It is RGBA</param>
        /// <param name="borderColor">The border color for the graph. It is RGBA.</param>
        /// <returns>Data that can be placed in %plot%</returns>
        public static string SpliceGraph(string valueVarName, string ctxName, string declareName, string title, uint delay, uint period, Color bgColor, Color borderColor)
        {
            return GraphPageSplicerConstant.GraphDeclarationCode
                .Replace(GraphPageSplicerConstant.ReplaceInDeclaration, declareName)
                .Replace(GraphPageSplicerConstant.ReplaceInCtx, ctxName).Replace(GraphPageSplicerConstant.ReplaceInTitle, title)
                .Replace(GraphPageSplicerConstant.ReplaceInBorderColor, $"rgba({borderColor.R}, {borderColor.G}, {borderColor.B}, {borderColor.A})")
                .Replace(GraphPageSplicerConstant.ReplaceInBackgroundColor, $"rgba({bgColor.R}, {bgColor.G}, {bgColor.B}, {bgColor.A})")
                .Replace(GraphPageSplicerConstant.ReplaceInDelay, delay.ToString())
                .Replace(GraphPageSplicerConstant.ReplaceInDuration, period.ToString())
                .Replace(GraphPageSplicerConstant.ReplaceInValue, valueVarName);
        }
    }
}
