namespace ImpostorHqR.Extensions.Api.Interface.Logging
{
    public interface IConsoleLogger
    {
        void LogInformation(string info, bool useDisk = false);

        void LogError(string info, object source, bool useDisk = false);

        void LogDebug(string info, object source, bool useDisk = false);
    }
}
