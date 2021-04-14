using System.Threading.Tasks;

namespace ImpostorHqR.Extension.Api.Interface.Logging
{
    public interface ILogManager
    {
        /// <summary>
        /// This function will log to the disk.
        /// </summary>
        /// <param name="entry"></param>
        ValueTask Log(LogEntry entry);
    }

    public struct LogEntry
    {
        /// <summary>
        /// Use this object to help locate the failing system. You may pass a null value.
        /// </summary>
        public object Source { get; set; }

        public LogType Type { get; set; }

        public string Message { get; set; }

        public LogEntry(string message, LogType type, object source = null)
        {
            this.Source = source;
            this.Message = message;
            this.Type = type;
        }
    }

    public enum LogType
    {
        Information, Warning, Error, Debug
    }
}
