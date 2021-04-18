using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;
using ImpostorHqR.Extension.Api.Configuration;
using Timer = System.Timers.Timer;

namespace ImpostorHqR.Extension.Graphs.Load.WebPages
{
    public class WebPageExceptions 
    {
        private IApiPage Page { get; set; }

        private volatile int ExceptionsLastSecond = 0;

        private readonly ConcurrentDictionary<string, int> Exceptions = new ConcurrentDictionary<string, int>();

        public void Start()
        {
            if (!IConfigurationStore.GetByType<WebPageConfig>().EnableExceptionsPage) return;
            AppDomain.CurrentDomain.FirstChanceException += OnException;
            this.Page = IApiPage.Create(
                "Exceptions / second", Color.Red, IConfigurationStore.GetByType<WebPageConfig>().ExceptionsPageHandle);
            var tmr = new Timer(1000) {AutoReset = true};
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
            var exceptions = Exceptions.OrderByDescending(x => x.Value).ToArray();
            Exceptions.Clear();
            using var sb = IReusableStringBuilder.Get();
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
                    sb.Append(value.ToString());
                    sb.Append("<br>");
                }
            }

            Page.Set(sb.ToString(), Color.Transparent);
        }
    }
}
