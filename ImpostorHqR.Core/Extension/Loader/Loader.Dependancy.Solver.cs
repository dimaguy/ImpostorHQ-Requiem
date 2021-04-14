using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Extension.Loader
{
    public static class DependencySolver
    {
        public static Assembly ResolveCrossPluginDependencies(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".resources")) return null;

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null) return assembly;

            ConsoleLogging.Instance.LogError($"Plugin Loader : Could not implicitly resolve assembly \"{args.Name}\".", null);
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();
            string asmFile = Path.Combine(@"..\", ExtensionLoaderConstant.ExtensionDir, filename);
            try
            {
                return Assembly.LoadFrom(asmFile);
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log(new LogEntry()
                {
                    Message = $"Plugin Loader: Resolve assembly error: {ex.ToString()}",
                    Type = LogType.Error
                });
                return null;
            }
        }
    }
}
