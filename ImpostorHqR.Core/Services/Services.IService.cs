namespace ImpostorHqR.Core.Services
{
    public interface IService
    {
        void PostInit();
        void Shutdown();
        void Activate();
    }
}
