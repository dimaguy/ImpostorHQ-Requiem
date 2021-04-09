using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.ComponentBus
{
    public class ComponentBusItem
    {
        public IExportBus Bus { get; set; }
        public string[] Names { get; set; }
    }
}
