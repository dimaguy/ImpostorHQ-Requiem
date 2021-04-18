﻿using System;
using System.Collections.Generic;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IGraphPage
    {
        #region Provider

        private static Converter<(IGraph[], string, string, byte), IGraphPage> _Provider;

        /// <summary>
        /// This function will slice and splice code in order to create a graph page that will be automatically registered and ready to use.
        /// </summary>
        /// <param name="graphs">Graphs, generated by the IGraphProvider</param>
        /// <param name="title">The title of the page.</param>
        /// <param name="handle">The effective path to register.</param>
        /// <param name="widthPercent">The width of the graphs (10-100).</param>
        /// <returns></returns>
        public static IGraphPage Create(IGraph[] graphs, string title, string handle, byte widthPercent)
        {
            return _Provider.Invoke((graphs, title, handle, widthPercent));
        }

        #endregion

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
