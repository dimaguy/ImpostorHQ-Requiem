using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ImpostorHqR.Core.Logging;

namespace ImpostorHqR.Core.Configuration
{
    public class ConfigHolder
    {
        public static readonly ConfigHolder Instance = new ConfigHolder();

        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        public ushort HttpPort { get; set; }

        public bool EnableSsl { get; set; }

        public ushort ApiPort { get; set; }

        public string LogPath { get; set; }

        public bool LogAsCsv { get; set; }

        public ushort RequestRatePerMinuteToBlock { get; set; }

        public ushort UnblockAfterMinutes { get; set; }

        public int FileIoSize { get; set; }

        public short ResourcePoolTimeout { get; set; }

        public ushort ResourcePoolSize { get; set; }

        private static void Serialize() => File.WriteAllText(ConfigConstant.ConfigFileName, JsonSerializer.Serialize(ConfigHolder.Instance, Options));

        public void Shutdown() => Serialize();

        public void Start()
        {
            ConsoleLogging.Instance.LogInformation("Loading configs.");
            if (!File.Exists(ConfigConstant.ConfigFileName))
            {

                this.ApiPort = ConfigConstant.ApiPort;
                this.HttpPort = ConfigConstant.HttpPort;
                this.LogPath = ConfigConstant.LogFolderPath;
                this.LogAsCsv = ConfigConstant.LogAsCsv;
                this.RequestRatePerMinuteToBlock = ConfigConstant.RequestRateToBlock;
                this.UnblockAfterMinutes = ConfigConstant.BlockLifeTime;
                this.EnableSsl = ConfigConstant.EnableSsl;
                this.FileIoSize = ConfigConstant.FileTransferSize;
                this.ResourcePoolTimeout = ConfigConstant.ResourcePoolTimeout;
                this.ResourcePoolSize = ConfigConstant.ResourcePoolSize;
                Serialize();
            }
            else
            {
                var config = JsonSerializer.Deserialize<ConfigHolder>(File.ReadAllText(ConfigConstant.ConfigFileName));
                this.ApiPort = config.ApiPort;
                this.HttpPort = config.HttpPort;
                this.LogPath = config.LogPath;
                this.LogAsCsv = config.LogAsCsv;
                this.RequestRatePerMinuteToBlock = config.RequestRatePerMinuteToBlock;
                this.UnblockAfterMinutes = config.UnblockAfterMinutes;
                this.EnableSsl = config.EnableSsl;
                this.FileIoSize = config.FileIoSize;
                this.ResourcePoolTimeout = config.ResourcePoolTimeout;
                this.ResourcePoolSize = config.ResourcePoolSize;
            }
        }
    }
}
