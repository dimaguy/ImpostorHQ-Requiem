using System;
using System.Reflection;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class LoaderExtension
    {
        public Assembly Assembly { get; }

        public Type Start { get; }

        public IExtensionInformation Information { get; }

        public string[] Dependencies { get; set; }

        public LoaderExtension(Assembly assembly, Type start, IExtensionInformation info)
        {
            this.Assembly = assembly;
            this.Start = start;
            this.Information = info;
        }
    }
}
