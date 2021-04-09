using System.Collections.Concurrent;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Handles
{
    public class WebApiHandleStore
    {
        public static readonly WebApiHandleStore Instance = new WebApiHandleStore();

        public ConcurrentBag<ApiHandleHolder> Handles = new ConcurrentBag<ApiHandleHolder>();

        public void Add(ApiHandleHolder handle)
        {
            Handles.Add(handle);
        }
    }
}
