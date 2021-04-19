using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage;
using ImpostorHqR.Core.Web.Page.Generator.NoApi.TableSite;
using ImpostorHqR.Extension.Api.Api.Web;

namespace ImpostorHqR.Core.Web
{
    static class WebBinder
    {
        public static void Bind()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            const string function = "_Provider";
            const string functionSecure = "_SecureProvider";

            typeof(IApiPage).GetField(function, flags)!.SetValue(null, new Converter<(string, Color, string), IApiPage>(ApiPageProvider));
            typeof(IGraph).GetField(function, flags)!.SetValue(null, new Converter<(string, Color, Color, uint, uint), IGraph>(GraphProvider));
            typeof(IGraphPage).GetField(function, flags)!.SetValue(null, new Converter<(IGraph[], string, string, byte), IGraphPage>(GraphPageProvider));
            typeof(IReadonlyConsolePage).GetField(function, flags)!.SetValue(null, new Converter<(string, Color, string, string), IReadonlyConsolePage>(ReadonlyConsolePageProvider));
            typeof(IStaticPage).GetField(function, flags)!.SetValue(null, new Converter<(Color webColor, string title, string handle), IStaticPage>(StaticPageProvider));

            typeof(IApiPage).GetField(functionSecure, flags)!.SetValue(null, new Converter<(string, Color, string, WebPageAuthenticationOption), IApiPage>(ApiPageProviderSecure));
            typeof(IGraphPage).GetField(functionSecure, flags)!.SetValue(null, new Converter<(IGraph[], string, string, byte, WebPageAuthenticationOption), IGraphPage>(GraphPageProviderSecure));
            typeof(IReadonlyConsolePage).GetField(functionSecure, flags)!.SetValue(null, new Converter<(string, Color, string, string, WebPageAuthenticationOption), IReadonlyConsolePage>(ReadonlyConsolePageProviderSecure));
            typeof(IStaticPage).GetField(functionSecure, flags)!.SetValue(null, new Converter<(Color webColor, string title, string handle, WebPageAuthenticationOption), IStaticPage>(StaticPageProviderSecure));
        }

        private static IGraph GraphProvider((string title, Color fillColor, Color lineColor, uint span, uint delay) args)
        {
            return new ApiGraph(args.title, args.fillColor, args.lineColor, args.delay, args.span);
        }

        #region Insecure Providers

        private static IApiPage ApiPageProvider((string title, Color color, string handle) args)
        {
            return new SimpleApiPage(args.color, args.title, args.handle);
        }

        private static IGraphPage GraphPageProvider((IGraph[] graphs, string title, string handle, byte width) args)
        {
            return new ApiGraphPage(args.handle, args.title, args.graphs.Select(a=>(ApiGraph)a).ToArray(), args.width);
        }
        private static IReadonlyConsolePage ReadonlyConsolePageProvider((string title, Color color, string placeholder, string handle) args)
        {
            return new ApiConsolePage(args.color, args.title, args.handle, args.placeholder);
        }

        private static IStaticPage StaticPageProvider((Color webColor, string title, string handle) args)
        {
            return new TableSite(args.webColor, args.title, args.handle);
        }

        #endregion

        #region Secure Providers

        private static IApiPage ApiPageProviderSecure((string title, Color color, string handle, WebPageAuthenticationOption creds) args)
        {
            return new SimpleApiPage(args.color, args.title, args.handle, args.creds);
        }

        private static IGraphPage GraphPageProviderSecure((IGraph[] graphs, string title, string handle, byte width, WebPageAuthenticationOption creds) args)
        {
            return new ApiGraphPage(args.handle, args.title, args.graphs.Select(a => (ApiGraph)a).ToArray(), args.width, args.creds);
        }
        private static IReadonlyConsolePage ReadonlyConsolePageProviderSecure((string title, Color color, string placeholder, string handle, WebPageAuthenticationOption creds) args)
        {
            return new ApiConsolePage(args.color, args.title, args.handle, args.placeholder, args.creds);
        }

        private static IStaticPage StaticPageProviderSecure((Color webColor, string title, string handle, WebPageAuthenticationOption creds) args)
        {
            return new TableSite(args.webColor, args.title, args.handle, args.creds);
        }

        #endregion
    }
}
