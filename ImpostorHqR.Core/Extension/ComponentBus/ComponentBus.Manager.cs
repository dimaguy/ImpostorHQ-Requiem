using System;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Core.Extension.Loader.ExtensionStore;
using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.ComponentBus
{
    public class ComponentBusManager : IComponentHub
    {
        public static readonly ComponentBusManager Instance = new ComponentBusManager();

        public ComponentSearchResult Acquire(string package, string name)
        {
            var extension = ExtensionStore.Instance.Extensions.FirstOrDefault(ext => ext.PackageName.Equals(package));
            if (extension == null) return new ComponentSearchResult() { Error = ComponentSearchError.ExtensionNotFound };

            var holder = extension.Exports.FirstOrDefault(item => item.Names.Contains(name));
            if (holder == null) return new ComponentSearchResult() { Error = ComponentSearchError.ValueNotFound };

            var instance = holder.Bus.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(holder.Bus, null);

            return new ComponentSearchResult()
            {
                Error = ComponentSearchError.None,
                Result = instance
            };
        }
    }
}
