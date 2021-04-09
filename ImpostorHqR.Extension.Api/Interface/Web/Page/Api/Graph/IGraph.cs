using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph
{
    public interface IGraph
    {
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
