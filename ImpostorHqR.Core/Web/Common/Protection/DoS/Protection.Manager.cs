using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Core.Web.Common.Protection.DoS
{
    public static class HqServerProtector
    {
        private static readonly ConcurrentDictionary<IPAddress, int> Connections;
        private static readonly ConcurrentDictionary<IPAddress, byte> Blocked;
        private static readonly int Rate;

        static HqServerProtector()
        {
            Connections = new ConcurrentDictionary<IPAddress, int>();
            Blocked = new ConcurrentDictionary<IPAddress, byte>();

            Rate = IConfigurationStore.GetByType<RequiemConfig>().RequestRatePerMinuteToBlock;

            var tmr = new System.Timers.Timer(1000) {AutoReset = true};
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        private static void Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var connection in Connections)
                {
                    var count = connection.Value - 1;
                    if (count > 0) Connections.TryUpdate(connection.Key, count, connection.Value);
                    else Connections.TryRemove(connection);
                }
            }
            catch (Exception ex)
            {
                ILogManager.Log($"Rate limit error.", "Rate limiter.", LogType.Error, ex:ex);
            }
        }

        public static bool IsAttacking(IPAddress address)
        {
            if (Connections.TryGetValue(address, out var value) && value >= Rate)
            {
                if (!Blocked.ContainsKey(address))
                {
                    Blocked.TryAdd(address, 0);
                    OnBlocked?.Invoke(address);
                }

                return true;
            }

            Connections.AddOrUpdate(address, _ => 1, (_, i) => i + 1);
            return false;
        }

        public static event Action<IPAddress> OnBlocked;
    }
}
