using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph
{
    public interface IGraphProvider
    {
        /// <summary>
        /// Creates a graph for the IGraphPage provider.
        /// </summary>
        /// <param name="title">The text that will be shown to the user.</param>
        /// <param name="fillColor">The color that will fill the graph.</param>
        /// <param name="lineColor">The color of the data set.</param>
        /// <param name="span">The span of time to show.</param>
        /// <param name="delay">The delay. If higher, the data can be smoothed more.</param>
        /// <returns></returns>
        public IGraph Create(string title, Color fillColor, Color lineColor, uint span, uint delay);
    }
}
