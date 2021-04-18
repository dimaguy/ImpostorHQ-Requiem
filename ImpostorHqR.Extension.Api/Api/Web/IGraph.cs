using System;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IGraph
    {
        #region Provider

        private static Converter<(string, Color, Color, uint, uint), IGraph> _Provider;

        /// <summary>
        /// Creates a graph for the IGraphPage.
        /// </summary>
        /// <param name="title">The text that will be shown to the user.</param>
        /// <param name="fillColor">The color that will fill the graph.</param>
        /// <param name="lineColor">The color of the data set.</param>
        /// <param name="span">The span of time to show.</param>
        /// <param name="delay">The delay. If higher, the data can be smoothed more.</param>
        /// <returns></returns>
        public static IGraph Create(string title, Color fillColor, Color lineColor, uint span, uint delay)
        {
            return _Provider.Invoke((title, fillColor, lineColor, span, delay));
        }

        #endregion

        /// <summary>
        /// The name that will be presented to the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Shows how temporally large the graph is.
        /// </summary>
        uint TimeSpan { get; }

        uint Delay { get; }

        Color LineColor { get; }

        Color FillColor { get; }

        /// <summary>
        /// The current value (may not be synchronized with the clients unless the page's Update() method is called.
        /// </summary>
        long Value { get; }

        /// <summary>
        /// Call this to update the graph.
        /// </summary>
        /// <param name="value">The value to set.
        ///     Warning: the value will not be sent to the client if the page's update method is not called
        /// </param>
        void Update(long value);
    }
}
