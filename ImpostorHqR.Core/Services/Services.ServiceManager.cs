using System;
using System.Collections.Generic;
using System.Linq;
using ImpostorHqR.Extension.Api.Interface;
using ImpostorHqR.Extension.Api.Interface.Web;

namespace ImpostorHqR.Core.Services
{
    public class ServiceManager
    {
        public static readonly ServiceManager Instance = new ServiceManager();

        private List<IService> _hqServices;
        private List<IExtensionService> _pluginServices;
        private List<ISimpleWebService> _simpleWebServices;
        private List<ISimpleApiWebService> _simpleApiWebServices;


        public ServiceManager()
        {
            _hqServices = new List<IService>();
            _pluginServices = new List<IExtensionService>();
            _simpleWebServices = new List<ISimpleWebService>();
            _simpleApiWebServices = new List<ISimpleApiWebService>();
            ActivateInternalServices();
        }

        private void ActivateInternalServices()
        {
            var services = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IService)
                    .IsAssignableFrom(p) && !p.IsInterface);

            foreach (var service in services)
            {
                var instance = Activator.CreateInstance(service);
                if (instance == null) continue;
                _hqServices.Add(instance as IService);
            }
        }

        public void ActivateExtensionServices()
        {
            var extensionServices = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IExtensionService)
                    .IsAssignableFrom(p) && !p.IsInterface);

            foreach (var service in extensionServices)
            {
                if (Activator.CreateInstance(service) is IExtensionService instance) _pluginServices.Add(instance);
            }
        }

        public void PluginServiceInit()
        {
            foreach (var extensionService in _pluginServices)
            {
                extensionService.Init();
            }
        }

        public void ExtensionServicePostInit()
        {
            foreach (var extensionService in _pluginServices)
            {
                extensionService.PostInit();
            }
        }

        public ISimpleWebService[] ActivateSimpleWebServices()
        {
            var simpleServices = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ISimpleWebService)
                    .IsAssignableFrom(p) && !p.IsInterface).ToList();
            if (simpleServices.Count == 0) return new ISimpleWebService[] { };
            var instances = simpleServices.Select(service => Activator.CreateInstance(service) as ISimpleWebService).ToList();

            return instances.OrderBy(item => item.UpdateInterval).ToArray();
        }

        public ISimpleApiWebService[] ActivateSimpleApiWebServices()
        {
            var simpleServices = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ISimpleApiWebService)
                    .IsAssignableFrom(p) && !p.IsInterface).ToList();
            if (simpleServices.Count == 0) return new ISimpleApiWebService[] { };

            var instances = new List<ISimpleApiWebService>();
            foreach (var simpleService in simpleServices)
            {
                if (Activator.CreateInstance(simpleService) is ISimpleApiWebService instance) instances.Add(instance);
            }

            return instances.ToArray();
        }

        public void PluginServiceShutDown()
        {
            foreach (var extensionService in _pluginServices)
            {
                extensionService.Shutdown();
            }
        }

        public void Start()
        {
            foreach (var hqService in _hqServices) hqService.PostInit();
        }

        public void Stop()
        {
            foreach (var hqService in _hqServices) hqService.Shutdown();
        }

        public void PreInit()
        {
            foreach (var hqService in _hqServices) hqService.Activate();
        }
    }
}
