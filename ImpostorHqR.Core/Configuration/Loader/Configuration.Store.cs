using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Configuration.Loader
{
    public static class ConfigurationStore
    {
        private static readonly List<object> Configurations = new List<object>();

        private static readonly HashSet<Type> LookupTable = new HashSet<Type>();

        static ConfigurationStore()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            // ReSharper disable once PossibleNullReferenceException
            typeof(IConfigurationStore).GetField("Query", flags).SetValue(null, new Converter<Type, object>(GetByType));
            // ReSharper disable once PossibleNullReferenceException
            typeof(IConfigurationStore).GetField("Lookup", flags).SetValue(null, new Converter<Type, bool>(Contains));
        }

        public static void Register(object config)
        {
            lock (LookupTable)
            {
                if(LookupTable.Contains(config.GetType())) throw new Exception("Config store already contains config.");
                LookupTable.Add(config.GetType());
                lock (Configurations) Configurations.Add(config);
            }
        }

        public static object GetByType(Type type)
        {
            lock (Configurations) return Configurations.FirstOrDefault(config => config.GetType() == type);
        }

        public static bool Contains(Type type)
        {
            lock (LookupTable) return LookupTable.Contains(type);
        }
    }
}
