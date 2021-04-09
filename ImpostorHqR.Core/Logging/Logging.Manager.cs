using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Logging
{
    public class LogManager : IService, ILogManager
    {
        public static LogManager Instance;

        private readonly BlockingCollection<LogEntry> Queue = new BlockingCollection<LogEntry>();

        public LogManager()
        {
            if (Instance == null)
            {
                Instance = this;
                new Thread(LoggingThread).Start();
            }
        }

        public void Log(LogEntry entry) => Queue.Add(entry);

        private void LoggingThread()
        {
            var fileName = Path.Combine(ConfigHolder.Instance.LogPath, DateTime.Now.ToShortDateString().Replace('/', '.'));
            var sb = new StringBuilder();
            var path = ConfigHolder.Instance.LogAsCsv ? $"{fileName}.csv" : $"{fileName}.log";
            new FileInfo(path).Directory?.Create();
            using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            foreach (var entry in Queue.GetConsumingEnumerable())
            {
                if (string.IsNullOrEmpty(entry.Message)) throw new Exception("Programmer error: log entry has a null message.");
                var sourceName = entry.Source == null ? "[source not specified]" : ConfigHolder.Instance.LogAsCsv ? entry.Source.ToString() + "," : $"[{entry.Source}] ";

                sb.Clear();
                if (ConfigHolder.Instance.LogAsCsv)
                {
                    sb.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ',');
                    sb.Append(sourceName + ',');
                    sb.Append(entry.Type.ToString() + ',');
                    sb.Append(entry.Message);
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " | ");
                    sb.Append(sourceName + " - ");
                    sb.Append(entry.Type + ": \"");
                    sb.Append(entry.Message + "\"");
                    sb.AppendLine();
                }

                fs.Write(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }

        public void Shutdown() => Log(new LogEntry()
        {
            Message = "Requiem Shutting down.",
            Source = this,
            Type = LogType.Information
        });

        public void PostInit()
        {
            if (!Directory.Exists(ConfigHolder.Instance.LogPath)) Directory.CreateDirectory(ConfigHolder.Instance.LogPath);
            Log(new LogEntry()
            {
                Message = "Requiem Starting.",
                Source = this,
                Type = LogType.Information
            });
        }

        public void Activate() { }
    }
}
