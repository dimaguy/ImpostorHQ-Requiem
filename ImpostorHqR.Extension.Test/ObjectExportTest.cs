using ImpostorHq.Extensions.Api.Interface.Export;

namespace ImpostorHq.Extension.Test
{
    public class ObjectExportTest : IExportBus
    {
        public static ObjectExportTest Instance;

        public long FastNumber = 0;

        public IPrimitiveBox FastNumberBox { get; set; }

        public string Message { get; set; }

        public ObjectExportTest()
        {
            Instance = this;
            this.FastNumberBox = Proxy.Instance.PreInitialization.PrimitiveBoxProvider.GetLong(ref FastNumber);
            this.Message = "Do not go gentle into that good night";
        }
    }
}