#nullable enable
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Core.Logging
{
    public static class LoggingManager
    {
        private static StreamWriter Writer;
        private static int _QueueSize = 0;
        private static readonly Channel<(string Message, string Source, LogType, bool UseFile, bool UseConsole, Exception? ex)> Queue =
            Channel.CreateUnbounded<(string Message, string Source, LogType, bool UseFile, bool UseConsole, Exception? ex) >(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = false,
            });

        public static void Initialize()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

            typeof(ILogManager).GetField("_Logger", flags)!.SetValue(null, new Action<(string, string, LogType, bool, bool, Exception?)>(
                async (item) =>
                {
                    var (message, source, logType, toFile, toConsole, exception) = item;
                    await LogAsync(message, source, logType, toFile, toConsole, exception);
                }));
            typeof(ILogManager).GetField("_GetQueueSize", flags)!.SetValue(null, new Func<int>(()=>_QueueSize));

            Task.Factory.StartNew(Consumer, TaskCreationOptions.LongRunning);
        }

        public static ValueTask LogAsync(string message, string? source, LogType type, bool writeToFile = false, bool writeToConsole = true, Exception? ex = null)
        {
            if (!writeToConsole && !writeToFile)
            {
                return default;
            }

            Interlocked.Increment(ref _QueueSize);
            source ??= Assembly.GetCallingAssembly().GetName().Name;
            return Queue.Writer.WriteAsync((message, source, type, writeToFile, writeToConsole, ex));
        }

        private static async Task Consumer()
        {
            var path = $"ImpostorHqR.Logs/{DateTime.Now.ToString("u").Replace(":",".")}.log.csv";
            var fi = new FileInfo(path);
            fi.Directory?.Create();
            await using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            Writer = new StreamWriter(fs, Encoding.UTF8);
            await foreach (var log in Queue.Reader.ReadAllAsync())
            {
                var (message, source, type, file, console, ex) = log;
                
                if (file)
                {
                    using var sb = IReusableStringBuilder.Get();
                    sb.Append(DateTime.Now.ToString());
                    sb.Append(',');
                    sb.Append(type.ToString());
                    sb.Append(',');
                    sb.Append(source);
                    sb.Append(',');
                    sb.Append(message);
                    if (ex != null)
                    {
                        sb.Append(',');
                        var exStr = ex.ToString();
                        ReplaceNewLineVeryUnsafe(ref exStr);
                        sb.Append(exStr);
                    }
                    sb.Append("\r\n");
                    await Writer.WriteAsync(sb.StringBuilder);
                }
                if (console)
                {
                    var logger = ImpostorHqR.Extension.Api.Registry.Impostor.Logger;
                    switch (type)
                    {
                        case LogType.Debug:
                            logger.LogDebug($"Requiem Debug from \"{source}\": {message}");
                            break;
                        case LogType.Information:
                            logger.LogInformation($"Requiem Information from \"{source}\": {message}");
                            break;
                        case LogType.Error:
                            logger.LogError($"Requiem ERROR in \"{source}\": {message}");
                            break;
                        case LogType.Warning:
                            logger.LogWarning($"Requiem WARNING in \"{source}\": {message}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Interlocked.Decrement(ref _QueueSize);
            }

            // !!!! NEVER USE THIS ANYWHERE ELSE !!!!
            static unsafe void ReplaceNewLineVeryUnsafe(ref string str)
            {
                fixed (char* ptr = &str.GetPinnableReference())
                {
                    for (var i = 0; i < str.Length; i++)
                    {
                        if (ptr[i] == '\r' || ptr[i] == '\n') ptr[i] = '|';
                    }
                }
            }
        }

        

        public static void Shutdown()
        {
            Queue.Writer.Complete();
            Writer.Flush();
        }
    }
}
