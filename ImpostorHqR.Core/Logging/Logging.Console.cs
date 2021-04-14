using ImpostorHqR.Core.Impostor;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extensions.Api.Interface.Logging;
using Microsoft.Extensions.Logging;

namespace ImpostorHqR.Core.Logging
{
    public class ConsoleLogging : IConsoleLogger
    {
        public static readonly ConsoleLogging Instance = new ConsoleLogging();
        public void LogInformation(string str, bool file = false)
        {
            ImpostorObjectHolder.Instance.ImpostorLogger.LogInformation($"ImpostorHqR Information : {str}");
            if (!file) return;
            LogManager.Instance.Log(new LogEntry()
            {
                Message = str,
                Source = null,
                Type = LogType.Information
            });
        }

        public void LogError(string str, object caller, bool file = false)
        {
            ImpostorObjectHolder.Instance.ImpostorLogger.LogError($"ImpostorHqR Error : {str} {(caller == null ? "" : $"[in {caller.ToString()}]")}");
            if (!file) return;
            LogManager.Instance.Log(new LogEntry()
            {
                Message = str,
                Source = caller,
                Type = LogType.Error
            });
        }

        public void LogDebug(string str, object caller, bool file = false)
        {
            ImpostorObjectHolder.Instance.ImpostorLogger.LogInformation($"ImpostorHqR Debug : {str} {(caller == null ? "" : $"[in {caller.ToString()}]")}");
            if (!file) return;
            LogManager.Instance.Log(new LogEntry()
            {
                Message = str,
                Source = caller,
                Type = LogType.Debug
            });
        }
    }
}
