using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Core.Extension.ComponentBus;
using ImpostorHqR.Core.Extension.Configuration;
using ImpostorHqR.Core.Extension.Loader.Proxy;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage;
using ImpostorHqR.Core.Web.Page.Store;
using ImpostorHqR.Extension.Api.Loader;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class Loader : IService
    {
        public void PostInit()
        {
            foreach (var instanceExtension in ExtensionStore.ExtensionStore.Instance.Extensions)
            {
                var postInitEvent = new PostInitHolder();
                typeof(ExtensionProxy).GetProperty("PostInitialization")?.SetValue(instanceExtension.Proxy, postInitEvent, null);
                instanceExtension.Proxy.PostInit(postInitEvent);
                ServiceManager.Instance.ExtensionServicePostInit();
            }
        }

        public void Activate()
        {
            if (!Directory.Exists(ExtensionLoaderConstant.ExtensionDir))
            {
                Directory.CreateDirectory(ExtensionLoaderConstant.ExtensionDir);
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += DependencySolver.ResolveCrossPluginDependencies;
            List<Assembly> assemblies = new List<Assembly>();
            if (Directory.GetFiles(ExtensionLoaderConstant.ExtensionDir).Length <= 0) return;
            foreach (var file in Directory.GetFiles(ExtensionLoaderConstant.ExtensionDir))
            {
                if (file.EndsWith(".dll"))
                {
                    assemblies.Add(Assembly.LoadFile(Path.GetFullPath(file)));
                }
            }

            var cores = new List<ExtensionCore>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                var entryPoints = (from type in types
                                   where
                                       typeof(IExtensionEntryPoint).IsAssignableFrom(type) && type.IsClass
                                   select type).ToList();

                var proxies = (from type in types
                               where
                                   typeof(ExtensionProxy).IsAssignableFrom(type) && type.IsClass
                               select type).ToList();

                if (entryPoints.Count == 0)
                {
                    ConsoleLogging.Instance.LogError($"Assembly \"{assembly.GetName()}\" does not implement an entry point and will not be instantiated.", this);
                    continue;
                }

                if (proxies.Count == 0)
                {
                    ConsoleLogging.Instance.LogError($"Assembly \"{assembly.GetName()}\" does not implement a proxy and will not be instantiated.", this);
                    continue;
                }

                if (entryPoints.Count > 1)
                {
                    ConsoleLogging.Instance.LogError($"Assembly \"{assembly.GetName()}\" implements more than one [{entryPoints.Count}] entry points and will not be instantiated.", this);
                    continue;
                }

                if (proxies.Count > 1)
                {
                    ConsoleLogging.Instance.LogError($"Assembly \"{assembly.GetName()}\" implements more than one [{proxies.Count}] proxies and will not be instantiated.", this);
                    continue;
                }

                cores.Add(new ExtensionCore()
                {
                    Entry = entryPoints[0],
                    Proxy = proxies[0],
                    Assembly = assembly
                });
            }

            // instantiate the objects

            if (cores.Count == 0)
            {
                ConsoleLogging.Instance.LogInformation("No extensions to load.");
                return;
            }

            foreach (var extensionCore in cores)
            {
                var entryInstance = (IExtensionEntryPoint)Activator.CreateInstance(extensionCore.Entry);
                if (entryInstance == null)
                {
                    ConsoleLogging.Instance.LogError("Fatal error: entry point instance is null after activation.", this);
                    continue;
                }

                var proxyInstanceObj = Activator.CreateInstance(extensionCore.Proxy);
                var proxyInstance = proxyInstanceObj as ExtensionProxy;

                if (proxyInstance == null)
                {
                    ConsoleLogging.Instance.LogError("Fatal error: proxy instance is null after activation.", this);
                    continue;
                }

                var instanceProperty = proxyInstanceObj.GetType().GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                var instanceField = proxyInstanceObj.GetType().GetField("Instance", BindingFlags.Public | BindingFlags.Static);

                if (instanceProperty != null)
                {
                    if (!instanceProperty.CanWrite || instanceProperty.GetSetMethod() == null)
                    {
                        ConsoleLogging.Instance.LogError($"Extension {entryInstance.PackageName} has an invalid instance declaration.", this);
                    }
                    else
                    {
                        instanceProperty.SetValue(proxyInstanceObj, proxyInstanceObj);
                    }
                }
                else
                {
                    if (instanceField != null)
                    {
                        instanceField.SetValue(proxyInstanceObj, proxyInstanceObj);
                    }
                }

                if (entryInstance.ApiVersion != ExtensionLoaderConstant.ApiVersion)
                {
                    ConsoleLogging.Instance.LogError(
                        $"Extension \"{entryInstance.Name}\" " +
                        $"{entryInstance.Version} [{entryInstance.PackageName}] " +
                        $"was built against API version {entryInstance.ApiVersion}, " +
                        $"but the current one is {ExtensionLoaderConstant.ApiVersion}. " +
                        $"It will not be registered.", this);
                    continue;
                }
                ConsoleLogging.Instance.LogInformation($"Loaded \"{entryInstance.Name}\" ({entryInstance.PackageName}) version  {entryInstance.Version} by \"{entryInstance.Author}\".");
                ExtensionStore.ExtensionStore.Instance.AddExtension(new Extension(entryInstance, proxyInstance, extensionCore.Assembly));
            }

            // everything is now constructed. We may start calling proxy functions.
            foreach (var instanceExtension in ExtensionStore.ExtensionStore.Instance.Extensions)
            {
                ExtensionConfigActivator.Instance.Activate(instanceExtension);

                var preInitEvent = new PreInitHolder();
                typeof(ExtensionProxy).GetProperty("PreInitialization")!.SetValue(instanceExtension.Proxy, preInitEvent, null);

                instanceExtension.Proxy.PreInit(preInitEvent);
                ComponentBusActivator.Instance.Activate(instanceExtension);
            }

            ConsoleLogging.Instance.LogInformation($"Loaded {ExtensionStore.ExtensionStore.Instance.Extensions.Count} extensions.");

            ServiceManager.Instance.ActivateExtensionServices();
            ServiceManager.Instance.PluginServiceInit();
            foreach (var instanceExtension in ExtensionStore.ExtensionStore.Instance.Extensions)
            {
                var initEvent = new InitHolder();
                typeof(ExtensionProxy).GetProperty("Initialization")?.SetValue(instanceExtension.Proxy, initEvent, null);
                instanceExtension.Proxy.Init(initEvent);
            }

            var simpleWebServices = ServiceManager.Instance.ActivateSimpleWebServices();
            foreach (var simpleWebService in simpleWebServices)
            {
                var (name, page) = simpleWebService.Register();

                if (page == null) throw new Exception($"Extension tried to register null webpage {(string.IsNullOrEmpty(name) ? "." : $"[{name}].")}");
                if (string.IsNullOrEmpty(name)) throw new Exception($"Extension tried to register a webpage with an empty name. [{page.GetType()}]");

                WebPageRegistry.Instance.RegisterSimplePage(name, page);

                if (simpleWebService.UpdateInterval == 0) throw new Exception("Extension tried to register a webpage with an update interval of 0.");

                var timer = new System.Timers.Timer(simpleWebService.UpdateInterval);
                timer.AutoReset = true;
                timer.Elapsed += (sender, e) => simpleWebService.Update();
                timer.Start();
            }

            var simpleApiWebServices = ServiceManager.Instance.ActivateSimpleApiWebServices();
            foreach (var simpleApiWebService in simpleApiWebServices)
            {
                var page = simpleApiWebService.Register();

                if (page == null) throw new Exception($"Extension tried to register null webpage.");
                WebPageStore.Instance.SimpleApiPages.Add(page as SimpleApiPage);
            }
        }

        public void Shutdown()
        {
            foreach (var extension in ExtensionStore.ExtensionStore.Instance.Extensions)
            {
                extension.Proxy.Shutdown();
            }
            ServiceManager.Instance.PluginServiceShutDown();
        }
    }
}
