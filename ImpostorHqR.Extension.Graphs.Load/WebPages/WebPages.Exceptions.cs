using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;
using Timer = System.Timers.Timer;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    public class WebPageExtension : IExtensionService
    {
        private ISimpleApiPage Page { get; set; }

        private volatile int ExceptionsLastSecond = 0;

        private ConcurrentDictionary<string, int> Exceptions = new ConcurrentDictionary<string, int>();

        public void Init()
        {
            if (!WebPageConfig.Instance.EnableExceptionsPage) return;

            AppDomain.CurrentDomain.FirstChanceException += OnException;
            this.Page = Proxy.Instance.PreInitialization.PageProvider.SimpleApiPageProvider.ProduceApiPage(
                "Exception Statistics", Color.White, WebPageConfig.Instance.ExceptionsPageHandle);
            var tmr = new Timer(1000);
            tmr.AutoReset = true;
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        private void OnException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Interlocked.Increment(ref ExceptionsLastSecond);
            var name = e.Exception.GetType().Name;

            if (Exceptions.ContainsKey(name))
            {
                Exceptions[name]++;
            }
            else
            {
                Exceptions.TryAdd(name, 1);
            }
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            KeyValuePair<string, int>[] exceptions;
            exceptions = Exceptions.OrderByDescending(x => x.Value).ToArray();
            Exceptions.Clear();
            var rsb = Proxy.Instance.PreInitialization.StringBuilderPool.Get();
            var sb = rsb.StringBuilder;
            sb.Append("It is ");
            sb.Append(DateTime.Now.ToString());
            if (exceptions.Length != 0)
            {
                sb.Append("<br>");
                foreach (var (key, value) in exceptions)
                {
                    sb.Append("Name: \"");
                    sb.Append(key);
                    sb.Append("\", thrown last second: ");
                    sb.Append(value);
                    sb.Append("<br>");
                }
            }

            Page.Set(sb.ToString(), Color.Black);
            Proxy.Instance.PreInitialization.StringBuilderPool.Return(rsb);
        }

        public void PostInit()
        {
        }

        public void Shutdown()
        {
        }
    }
}
