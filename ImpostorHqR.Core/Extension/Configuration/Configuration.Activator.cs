using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ImpostorHqR.Core.Extension.File.Store;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api.Interface;

namespace ImpostorHqR.Core.Extension.Configuration
{
    public class ExtensionConfigActivator
    {
        public static readonly ExtensionConfigActivator Instance = new ExtensionConfigActivator();

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public void Activate(Loader.Extension extension)
        {
            var holders = (from type in extension.Assembly.GetTypes()
                           where
                               typeof(IConfigurationHolder).IsAssignableFrom(type) && type.IsClass
                           select type).ToList();
            if (holders.Count == 0) return;

            foreach (var cfg in holders)
            {
                var instance = Activator.CreateInstance(cfg) as IConfigurationHolder;
                var fields = instance?.GetType().GetProperties();

                if (fields == null || fields.Length == 0) continue;
                var path = ExtensionFileStore.Instance.GetConfigPath(extension, instance.GetType().Name);

                if (!System.IO.File.Exists(path))
                {
                    instance.SetDefaults();
                    System.IO.File.WriteAllText(path, JsonSerializer.Serialize(instance, instance.GetType(), Options));
                    SetInstance(instance, instance);
                }
                else
                {
                    var file = JsonSerializer.Deserialize(System.IO.File.ReadAllBytes(path), instance.GetType(), Options);
                    if (file == null) throw new Exception($"Error loading config for {extension.PackageName}: invalid file. Please delete the file, and if the error persists, contact the extension's author.");
                    if (!(file is IConfigurationHolder)) throw new Exception($"Please delete the file located at: {path}");
                    SetInstance(instance, file);
                }
            }
        }

        private static void SetInstance(object destination, object value)
        {
            var instanceProperty = destination.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var instanceField = destination.GetType().GetField("Instance", BindingFlags.Public | BindingFlags.Static);

            if (instanceProperty != null)
            {
                instanceProperty.SetValue(destination, value);
            }
            else if(instanceField != null)
            {
                instanceField.SetValue(destination, value);
            }
            else
            {
                ConsoleLogging.Instance.LogError($"{destination.GetType().Name} has no Instance field or property.", null, true);
            }
        }
    }
}
