using System;

namespace ImpostorHqR.Extension.Api.Configuration
{
    public interface IConfigurationStore
    {
        // ReSharper disable once InconsistentNaming
        private static Converter<Type, object> Query;

        // ReSharper disable once InconsistentNaming
        private static Converter<Type, bool> Lookup;

        /// <summary>
        /// Used to get a configuration after loading.
        /// Warning: do not use this to actively get the configuration. This is intended to be used to get a reference to a config,
        /// that you can then assign to a variable. Using this actively (in hot paths) will have a performance impact.
        /// </summary>
        /// <typeparam name="T">The type of configuration to get.</typeparam>
        /// <returns></returns>
        static T GetByType<T>() where T : class => Query.Invoke(typeof(T)) as T;

        /// <summary>
        /// Used to check if the configuration store contains a certain configuration.
        /// </summary>
        /// <typeparam name="T">The type of configuration to check.</typeparam>
        /// <returns></returns>
        static bool Contains<T>() where T : class => Lookup.Invoke(typeof(T));
    }
}
