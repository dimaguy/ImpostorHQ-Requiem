using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Extension.Loader
{
    static class LoaderAssemblyResolve
    {
        public static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".resources")) return null;

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null) return assembly;

            ILogManager.Log($"Could not implicitly resolve assembly \"{args.Name}\".", "Extension Loader", LogType.Warning);
            var filename = args.Name.Split(',')[0] + ".dll".ToLower();
            var asmFile = Path.Combine("..\\", ExtensionLoader.Dir, filename);
            try
            {
                return Assembly.LoadFrom(asmFile);
            }
            catch (Exception ex)
            {
                ILogManager.Log($"Cannot load assembly at: {asmFile}.", "Extension Loader", LogType.Error, ex:ex);
                return null;
            }
        }
    }
}
