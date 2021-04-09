using System.Collections.Generic;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph
{
    public interface IGraphPage
    {
        /// <summary>
        /// The effective path to the page.
        /// </summary>
        string ApiHandle { get; }

        /// <summary>
        /// Will iterate trough the graphs on the page.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGraph> GetGraphs();

        /// <summary>
        /// Will update the page on all clients. Call this when you want to post data.
        /// </summary>
        void Update();
    }
}
