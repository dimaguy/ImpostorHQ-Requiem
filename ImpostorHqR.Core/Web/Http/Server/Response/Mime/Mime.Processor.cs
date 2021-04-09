using System.IO;
using System.Runtime.CompilerServices;

namespace ImpostorHqR.Core.Web.Http.Server.Response.Mime
{
    public static class MimeProcessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Interpret(string path)
        {
            var extension = Path.GetExtension(path);
            return extension.ToLower() switch
            {
                ".html" => "text/html",
                ".htm" => "text/html",
                ".js" => "text/javascript",
                ".css" => "text/css",
                ".ico" => "image/vnd.microsoft.icon",
                ".jpeg" => "image/jpeg",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".ttf" => "application/x-font-ttf",
                ".mp4" => "video/mp4",
                ".mkv" => "video/x-matroska",
                _ => null
            };
        }
    }
}
