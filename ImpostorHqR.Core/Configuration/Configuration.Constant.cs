namespace ImpostorHqR.Core.Configuration
{
    public static class ConfigConstant
    {
        public const ushort ApiPort = 22024, HttpPort = 4200;

        public const bool EnableSsl = false;

        public const string ConfigFileName = "ImpostorHq.Core.cfg";

        public const string LogFolderPath = "ImpostorHq.Logs";

        public const bool LogAsCsv = false;

        public const ushort RequestRateToBlock = 500;

        public const ushort BlockLifeTime = 5;

        public const int FileTransferSize = 1024 * 1024;

        public const short ResourcePoolTimeout = 5000;

        public const ushort ResourcePoolSize = 100;
    }
}
