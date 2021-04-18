using System;
using System.Runtime.CompilerServices;

namespace ImpostorHqR.Extension.Api
{
    public interface ILogManager
    {

        private static Action<(string Message, string? Source, LogType Type, bool ToFile, bool ToConsole, Exception? Exception)> _Logger;

        private static Func<int> _GetQueueSize;

        public static event Action<(string Message, string? Source, LogType Type, bool ToFile, bool ToConsole,
            Exception? Exception)> OnLogged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Log(string message, string? source, LogType type, bool toFile = true, bool toConsole = true, Exception? ex = null)
        {
            _Logger?.Invoke((message, source, type, toFile, toConsole, ex));
            OnLogged?.Invoke((message, source, type, toFile, toConsole, ex));
        }

        static int GetLogQueueSize() => _GetQueueSize();
    }

    public enum LogType
    {
        Debug,
        Information,
        Error,
        Warning
    }
}
