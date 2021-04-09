using ImpostorHqR.Core.Properties;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage.Splicer
{
    public static class ApiConsolePageSplicer
    {
        public static readonly string Html = Resources.OneWayConsoleHtmlFull;
        public const string ReplaceInColor = "%color%";
        public const string ReplaceInPort = "%port%";
        public const string ReplaceInHandle = "%handle%";
        public const string ReplaceInTitle = "%txt%";
        public const string ReplaceInMainPlaceholder = "%mainplaceholder%";
    }
}
