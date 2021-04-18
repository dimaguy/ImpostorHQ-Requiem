using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ImpostorHqR.Core.Helper
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetTypeInAssembly<T>(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return (from type in types
                where
                    typeof(T).IsAssignableFrom(type) && type.IsClass
                select type);
        }

        public static IEnumerable<Type> GetTypeGlobal<T>()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(ass => ass.GetTypes());
            return (from type in types
                where
                    typeof(T).IsAssignableFrom(type) && type.IsClass
                select type);
        }

        public static T CreateInstance<T>(Type t)
        {
            var instance = Activator.CreateInstance(t);
            Debug.Assert(instance!=null, "The created instance is null.");

            return (T) instance;
        }

        public static void CopyProperties<T>(T source, T destination)
        {
            var sourceProps = source.GetType().GetProperties().ToArray();
            if (sourceProps.Length == 0) return;
            var destProps = destination.GetType().GetProperties()
                .Where(p => p.CanWrite && sourceProps.Any(prop => prop.Name.Equals(p.Name)));

            foreach (var propertyInfo in destProps)
            {
                propertyInfo.SetValue(destination, source.GetType().GetProperty(propertyInfo.Name)?.GetValue(source));
            }
        }

        public static void CopyInstanceToStatic(object source, object destination)
        {
            var type = source.GetType();
            var srcFields = type.GetFields();
            var srcProps = type.GetProperties();
            var dstType = destination.GetType();
            var dstFields = dstType.GetFields(BindingFlags.Public | BindingFlags.Static);
            var dstProps = dstType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var fieldInfo in dstFields)
            {
                var src = srcFields.FirstOrDefault(f => f.Name.Equals(fieldInfo.Name));
                if(src == null) continue;

                fieldInfo.SetValue(destination, src.GetValue(source));
            }

            foreach (var propInfo in dstProps)
            {
                var src = srcProps.FirstOrDefault(f => f.Name.Equals(propInfo.Name));
                if (src == null) continue;

                propInfo.SetValue(destination, src.GetValue(source));
            }
        }

        public static IEnumerable<Type> GetTypesWithAttributeInAssembly(Type attr, Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttribute(attr, true) != null);
        }
    }
}
