using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Core.Configuration.Loader;
using ImpostorHqR.Core.Helper;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Extension.Loader
{
    public static class ExtensionLoader
    {
        public const string Dir = "ImpostorHqR.Extensions";

        private static readonly List<Assembly> Assemblies = new List<Assembly>();

        public static ExtensionStore Store;

        public static void Initialize()
        {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
                return;
            }

            var files = Directory.GetFiles(Dir).Where(file => file.EndsWith(".dll"));
            foreach (var file in files)
            {
                var assembly = Assembly.Load(System.IO.File.ReadAllBytes(file));
                Assemblies.Add(assembly);
            }

            AppDomain.CurrentDomain.AssemblyResolve += LoaderAssemblyResolve.Resolve;
        }

        public static void Start()
        {
            if (Assemblies.Count == 0)
            {
                ILogManager.Log("No plugins to load.", "Extension Loader", LogType.Warning);
                return;
            }

            var validAssemblies = Assemblies.Where(assembly => ReflectionHelper.GetTypeInAssembly<IExtensionRequiem>(assembly).Any() && ReflectionHelper.GetTypeInAssembly<IExtensionInformation>(assembly).Any()).ToArray();
            if (validAssemblies.Length == 0)
            {
                ILogManager.Log("Invalid DLL files in plugins folder!", "Extension Loader", LogType.Warning);
                return;
            }
            var packages = new List<LoaderExtension>();

            // load types and get information.

            foreach (var validAssembly in validAssemblies)
            {
                var information = ReflectionHelper.GetTypeInAssembly<IExtensionInformation>(validAssembly).ToArray();
                if(information.Length > 1) ILogManager.Log($"Extension tried to load multiple types of IExtensionInformation. Only 1 is permitted [{String.Join(',',information.Select(i=>i.Name))}]", "Extension Loader", LogType.Error);

                var start = ReflectionHelper.GetTypeInAssembly<IExtensionRequiem>(validAssembly).ToArray();
                if (start.Length > 1) ILogManager.Log($"Extension tried to load multiple types of IMinPlugin. Only 1 is permitted [{String.Join(',', start.Select(i => i.Name))}]", "Extension Loader", LogType.Error);

                var info = ReflectionHelper.CreateInstance<IExtensionInformation>(information[0]);
                var package = new LoaderExtension(validAssembly, start[0], info);

                ConfigurationLoader.LoadConfigs(validAssembly);

                packages.Add(package);
            }

            // solve dependencies
            for (var index = 0; index < packages.Count; index++)
            {
                var core = packages[index];
                var dependencyInfoType = ReflectionHelper.GetTypeInAssembly<IDependencyList>(core.Assembly).ToArray();
                var dependencies = new List<string>();

                foreach (var type in dependencyInfoType)
                {
                    var instance = ReflectionHelper.CreateInstance<IDependencyList>(type);
                    if (instance.Packages.Length > 0) dependencies.AddRange(instance.Packages);
                    else
                        ILogManager.Log(
                            $"The extension \"{core.Information.DisplayName}\" [{core.Information.Package}] contains empty dependency list [{instance.GetType().Name}].",
                            "Extension Loader", LogType.Warning);
                }
                core.Dependencies = dependencies.ToArray();
                if (dependencies.Count == 0) continue;
                foreach (var type in dependencies.Where(type =>
                    !packages.Any(package => package.Information.Package.Equals(type))))
                {
                    ILogManager.Log(
                        $"FATAL \"{core.Information.DisplayName}\" [{core.Information.Package}] is dependent on [{type}], which is not installed. Execution will not continue.",
                        "Extension Loader", LogType.Error);
                    packages.RemoveAt(index);
                    break;
                }
            }

            Store = new ExtensionStore();

            ILogManager.Log($"Loaded {packages.Count} extensions.", "Extension Loader", LogType.Information);
            foreach (var loaderExtension in packages)
            {
                Store.Add(loaderExtension);
            }
            
            Store.ExtensionInit();
        }
    }
}
