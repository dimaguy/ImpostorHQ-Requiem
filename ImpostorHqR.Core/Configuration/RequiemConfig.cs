using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Configuration
{
    [Configuration(false, true)]
    public class RequiemConfig
    {
        public int GetHttpCacheBytes => HttpFileCacheSizeMb * 1048576;

        public ushort HttpPort { get; set; } = 80;

        public bool EnableSsl { get; set; } = false;

        public ushort ApiPort { get; set; } = 22024;

        public string LogPath { get; set; } = "ImpostorHqR.Logs";

        public bool LogAsCsv { get; set; } = true;

        public int RequestRatePerMinuteToBlock { get; set; } = 100;

        public ushort UnblockAfterMinutes { get; set; } = 5;

        public int FileIoSize { get; set; } = 1024 * 1024;  // 1 MB Async IO

        public short ResourcePoolTimeout { get; set; } = 3000;

        public ushort ResourcePoolSize { get; set; } = 500; // 500 CCU

        public bool EnableHttpCache { get; set; } = true;   // recommended

        public int HttpFileCacheSizeMb { get; set; } = 10;  // hold scripts, font and images.

        public ushort HttpFileCacheTimeoutSeconds { get; set; } = ushort.MaxValue;  // no updating files.

        public string[] HttpCacheExclude { get; set; } = new string[]{/* no files excluded */};

        public int ApiAuthTimeoutSeconds { get; set; } = 5;
    }
}
