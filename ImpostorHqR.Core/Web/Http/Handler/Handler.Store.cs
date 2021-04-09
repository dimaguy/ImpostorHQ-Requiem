using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ImpostorHqR.Core.Web.Http.Handler
{
    public class HttpHandleStore
    {
        public static readonly HttpHandleStore Instance = new HttpHandleStore();

        public List<SpecialHandler> Handles = new List<SpecialHandler>();

        public void AddHandler(SpecialHandler handler)
        {
            if (handler.Path.StartsWith("/")) handler.Path = handler.Path.Remove(0, 1);
            if (handler.Path.EndsWith("/")) handler.Path = handler.Path.Remove(handler.Path.Length - 1, 1);
            lock (Handles)
            {
                Handles.Add(handler);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpecialHandler Check(string path)
        {
            lock (Handles) return Handles.FirstOrDefault(handler => handler.Path.Equals(path));
        }
    }
}
