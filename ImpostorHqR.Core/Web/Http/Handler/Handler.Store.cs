using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ImpostorHqR.Core.Web.Http.Handler
{
    public static class HttpHandleStore
    {
        public static readonly ConcurrentDictionary<string, SpecialHandler> Handles = new ConcurrentDictionary<string, SpecialHandler>();
        
        public static void AddHandler(SpecialHandler handler)
        {
            if (handler.Path.StartsWith("/")) handler.Path = handler.Path.Remove(0, 1);
            if (handler.Path.EndsWith("/")) handler.Path = handler.Path.Remove(handler.Path.Length - 1, 1);
            Handles.TryAdd(handler.Path, handler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpecialHandler Check(string path) => Handles.TryGetValue(path, out var result) ? result : null;
    }
}
