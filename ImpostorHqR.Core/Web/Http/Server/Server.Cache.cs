using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using Timer = System.Timers.Timer;

namespace ImpostorHqR.Core.Web.Http.Server
{
    public static class HttpServerFileCache
    {
        private static readonly ConcurrentDictionary<string, ValueTuple<byte[], int>> Records;

        private static readonly ConcurrentDictionary<string, uint> Timeouts;

        private static readonly ArrayPool<byte> Cache = ArrayPool<byte>.Create(IConfigurationStore.GetByType<RequiemConfig>().GetHttpCacheBytes, 20);

        public static int CurrentSize = 0;

        private static readonly int TimeoutCycles = IConfigurationStore.GetByType<RequiemConfig>().HttpFileCacheTimeoutSeconds;

        private static readonly SemaphoreSlim CacheLock = new SemaphoreSlim(1,1);

        private static RequiemConfig Config { get; }

        static HttpServerFileCache()
        {
            Config =  IConfigurationStore.GetByType<RequiemConfig>();
            Records = new ConcurrentDictionary<string,(byte[], int)>();
            Timeouts = new ConcurrentDictionary<string, uint>();
            var tmr = new Timer(1000) {AutoReset = true};
            tmr.Elapsed += Tick;
            tmr.Start();
        }

        private static void Tick(object sender, ElapsedEventArgs e)
        {
            foreach (var timeout in Timeouts)
            {
                var (path, cycles) = timeout;

                if (cycles >= TimeoutCycles)
                {
                    Timeouts.TryRemove(path, out _);
                    Records.TryRemove(path, out var bytes);
                    Cache.Return(bytes.Item1);
                    Interlocked.Add(ref CurrentSize, -1*(bytes.Item2));
                }
                else
                {
                    Timeouts.TryUpdate(path, cycles+1, cycles);
                }
            }
        }

        public static ValueTuple<bool, byte[]?, int?> TryGet(string path)
        {
            var success = Records.TryGetValue(path, out var result);
            if (success)
            {
                Timeouts.AddOrUpdate(path, _ => 0, (s, u) => 0);
            }

            return (success, result.Item1, result.Item2);
            
        }

        public static async ValueTask<ValueTuple<bool, byte[], Stream>> TryCache(string path, bool async)
        {
            await CacheLock.WaitAsync();
            try
            {
                //caching requires synchronization because multiple threads can enter caching before any synchronized collection
                //access happens.
                if (Records.TryGetValue(path, out var result))
                {
                    //already cached before lock was acquired
                    return (false, null, new MemoryStream(result.Item1, 0, result.Item2));
                }

                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                    useAsync: async);

                var remaining = Config.GetHttpCacheBytes - CurrentSize;
                if (fs.Length>Int32.MaxValue || remaining < fs.Length) return (false, null, fs);
                var len = (int) fs.Length;
                var buffer = Cache.Rent(len);

                if (async) await fs.ReadAsync(buffer.AsMemory(0, len));
                else fs.Read(buffer, 0, len);

                Records.AddOrUpdate(path, _ => (buffer, len), (_, i) => (buffer, len));
                Timeouts.TryAdd(path, 0);

                Interlocked.Add(ref CurrentSize, len);
                ILogManager.Log(
                    $"Cached \"{path}\" [{len} bytes]{(async ? " asynchronously." : ".")}","Server.Cache",LogType.Information);
                return (true, buffer, fs);
            }

            finally
            {
                CacheLock.Release();
            }
        }
    }
}
