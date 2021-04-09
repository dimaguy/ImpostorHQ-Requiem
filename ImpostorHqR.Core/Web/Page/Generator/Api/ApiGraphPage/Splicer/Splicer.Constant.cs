namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage.Splicer
{
    public static class GraphPageSplicerConstant
    {
        #region Graph Plot Function

        public static readonly string GraphDeclarationCode = Properties.Resources.GraphSiteGrapgJs;
        public const string ReplaceInDeclaration = "%varname%";
        public const string ReplaceInCtx = "%ctx%";             // also found in main
        public const string ReplaceInTitle = "%label%";         // also found in html
        public const string ReplaceInBorderColor = "%bcolor%";
        public const string ReplaceInBackgroundColor = "%bgcolor%";
        public const string ReplaceInDelay = "%delay%";
        public const string ReplaceInDuration = "%duration%";
        public const string ReplaceInValue = "%var%";           // also found in main

        #endregion

        #region Main Script

        public static readonly string MainScriptCode = Properties.Resources.GraphPageMainJs;
        public const string ReplaceInPlot = "%plot%";
        public const string ReplaceInJsonExtracts = "%receivers%";

        #endregion

        #region Html

        public static readonly string Html = Properties.Resources.GraphPageHtml;
        public const string ReplaceInHtmlDeclarations = "%graphs%";
        public const string ReplaceInScript = "%main%";
        public const string ReplaceInWidth = "%width%";

        #endregion
    }
}
