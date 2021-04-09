using System;
using System.Collections.Generic;
using System.Linq;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.ComponentBus
{
    public class ComponentBusActivator
    {
        public static readonly ComponentBusActivator Instance = new ComponentBusActivator();

        public void Activate(Loader.Extension extension)
        {
            var componentBuses = (from type in extension.Assembly.GetTypes()
                                  where
                                      typeof(IExportBus).IsAssignableFrom(type) && type.IsClass
                                  select type).ToList();
            if (componentBuses.Count == 0)
            {
                ConsoleLogging.Instance.LogInformation($"No exports found for {extension.PackageName}.");
                return;
            }
            ConsoleLogging.Instance.LogInformation($"Found {componentBuses.Count} export {(componentBuses.Count == 1 ? "bus" : "buses")} for {extension.PackageName}.");

            foreach (var bus in componentBuses)
            {
                var instance = Activator.CreateInstance(bus) as IExportBus;
                var fields = instance?.GetType().GetProperties();

                if (fields == null || fields.Length == 0) return;

                extension.Exports ??= extension.Exports = new List<ComponentBusItem>();

                extension.Exports.Add(new ComponentBusItem()
                {
                    Bus = instance,
                    Names = fields.Select(x => x.Name).ToArray()
                });

            }
        }
    }
}
