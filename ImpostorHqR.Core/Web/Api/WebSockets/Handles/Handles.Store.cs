using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Handles
{
    public static class WebApiHandleStore
    {
        public static readonly ConcurrentBag<ApiHandleHolder> Handles = new ConcurrentBag<ApiHandleHolder>();

        public static void Add(ApiHandleHolder handle)
        {
            Handles.Add(handle);
        }

        public static int GetClientCount()
        {
            var count = 0;
            foreach (var apiHandleHolder in Handles)
            {
                lock (apiHandleHolder.Users) count += apiHandleHolder.Users.Count;
            }

            return count;
        }
    }
}
