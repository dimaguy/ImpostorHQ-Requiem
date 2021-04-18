using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ImpostorHqR.Core.Helper;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Configuration.Loader
{
    public static class ConfigurationLoader
    {
        private static readonly Dictionary<object, string> SaveOnExit = new Dictionary<object, string>();

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions(){WriteIndented = true};

        public static void LoadConfigs(Assembly assembly)
        {
            var configs = ReflectionHelper.GetTypesWithAttributeInAssembly(typeof(ConfigurationAttribute), assembly).Select(ReflectionHelper.CreateInstance<object>).ToArray();
            foreach (var instance in configs)
            {
                var type = instance.GetType();
                var path = Path.Combine("ImpostorHqR.Configs", assembly.GetName().Name + '.' + type.Name + ".json");
                var fi = new FileInfo(path);
                fi.Directory?.Create();
                var config = (ConfigurationAttribute) type.GetCustomAttribute(typeof(ConfigurationAttribute));

                Debug.Assert(config!=null, "Configuration null.");

                if (File.Exists(path))
                {
                    var file = JsonSerializer.Deserialize(File.ReadAllText(path), type);
                    Debug.Assert(file!=null,"Null loaded config.");
                    ConfigurationStore.Register(file);
                    if(config!.SaveOnExit) lock(SaveOnExit) SaveOnExit.Add(file, path);
                }
                else 
                {
                    ConfigurationStore.Register(instance);
                    if ((config!.CreateOnLoad)) File.WriteAllText(path,JsonSerializer.Serialize(instance, Options));
                    if (config.SaveOnExit) lock (SaveOnExit) SaveOnExit.Add(instance, path);
                }
            }
        }

        public static void SafeConfigs()
        {
            lock (SaveOnExit)
            {
                foreach (var (config, path) in SaveOnExit)
                {
                    File.WriteAllText(path, JsonSerializer.Serialize(config, Options));
                }
            }
        }
    }
}
