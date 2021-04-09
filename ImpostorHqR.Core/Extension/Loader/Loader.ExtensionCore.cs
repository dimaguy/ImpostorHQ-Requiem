using System;
using System.Reflection;

namespace ImpostorHqR.Core.Extension.Loader
{
    public class ExtensionCore
    {
        public Type Entry { get; set; }
        public Type Proxy { get; set; }
        public Assembly Assembly { get; set; }
    }
}
