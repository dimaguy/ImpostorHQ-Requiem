using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Logging
{
    public class LogManager : IService, ILogManager
    {
        public static LogManager Instance;

        private const int FsBufferSize = 1024;

        private static FileStream Stream;

        private readonly Channel<LogEntry> Queue = Channel.CreateUnbounded<LogEntry>(new UnboundedChannelOptions()
        { SingleReader = true, SingleWriter = false });

        public LogManager()
        {
            if (Instance != null) return;
            Instance = this;
            Task.Factory.StartNew(LogTask, TaskCreationOptions.LongRunning);
        }

        public ValueTask Log(LogEntry entry) => Queue.Writer.WriteAsync(entry);

        private async Task LogTask()
        {
            var fileName = Path.Combine(ConfigHolder.Instance.LogPath, DateTime.Now.ToShortDateString().Replace('/', '.'));
            var sb = new StringBuilder();
            var path = ConfigHolder.Instance.LogAsCsv ? $"{fileName}.csv" : $"{fileName}.log";
            var encodingBuffer = new byte[FsBufferSize * 4];

            new FileInfo(path).Directory?.Create();

            // will not use an async file stream because the performance would be affected unless a large buffer size is used.
            // if a large buffer size is used, logs will be lost on force shutdown.
            await using var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, FsBufferSize, useAsync: false);

            Stream = fs;

            await foreach (var entry in Queue.Reader.ReadAllAsync())
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

                var data = sb.ToString().AsMemory();
                var offset = 0;
                for (var i = 0; i < data.Length / encodingBuffer.Length + (data.Length % encodingBuffer.Length == 0 ? 0 : 1); i++)
                {
                    var left = Math.Min(encodingBuffer.Length, data.Length - offset);
                    Encoding.UTF8.GetBytes(data.Span.Slice(offset, left), encodingBuffer);
                    offset += left;
                    fs.Write(encodingBuffer, 0, left);
                }
            }
        }

        public void Shutdown()
        {
            Log(new LogEntry()
            {
                Message = "Requiem Shutting down.",
                Source = this,
                Type = LogType.Information
            });

            Stream.Flush(true);

            Queue.Writer.Complete();
        }

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
