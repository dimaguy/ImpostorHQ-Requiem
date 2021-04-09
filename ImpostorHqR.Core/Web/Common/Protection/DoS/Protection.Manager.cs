using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;

namespace ImpostorHqR.Core.Web.Common.Protection.DoS
{
    public class HqServerProtector
    {
        public static readonly HqServerProtector Instance = new HqServerProtector();

        private readonly List<ServerAccessingEntity> Blocked = new List<ServerAccessingEntity>();

        private readonly List<ServerAccessingEntity> Lookout = new List<ServerAccessingEntity>();

        public HqServerProtector()
        {
            var clock = new System.Timers.Timer(10000);
            clock.Elapsed += Tick;
            clock.AutoReset = true;
            clock.Start();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAttacking(IPAddress address)
        {
            lock (Blocked)
            {
                if (Blocked.FirstOrDefault(item => item.Address.Equals(address)) != null) return true;
            }

            lock (Lookout)
            {
                var item = Lookout.FirstOrDefault(item => item.Address.Equals(address));
                if (item != null) Interlocked.Increment(ref item.Requests);
                else
                {
                    item = new ServerAccessingEntity(address);
                    Lookout.Add(item);
                }
            }

            return false;
        }

        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Blocked)
            {
                for (int i = 0; i < Blocked.Count; i++)
                {
                    var entity = Blocked[i];
                    if ((DateTime.Now - entity.TimeLastAccess).Minutes <
                        ConfigHolder.Instance.UnblockAfterMinutes) continue;
                    Blocked.Remove(entity);
                    entity.Dispose();
                }
            }

            lock (Lookout)
            {
                for (int i = 0; i < Lookout.Count; i++)
                {
                    var entity = Lookout[i];
                    if (entity.Requests > ConfigHolder.Instance.RequestRatePerMinuteToBlock * 6)
                    {
                        Lookout.Remove(entity);
                        lock (Blocked) Blocked.Add(entity);
                        entity.TimeLastAccess = DateTime.Now;
                        OnBlocked?.Invoke(entity.Address);
                        ConsoleLogging.Instance.LogInformation($"Blocked {entity.Address} at {DateTime.Now} for {ConfigHolder.Instance.UnblockAfterMinutes} minutes.");
                    }
                    else
                    {
                        Lookout.Remove(entity);
                    }
                }
            }
        }

        public event Action<IPAddress> OnBlocked;
    }
}
