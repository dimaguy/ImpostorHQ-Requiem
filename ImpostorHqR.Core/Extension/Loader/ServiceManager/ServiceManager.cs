using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Service;

namespace ImpostorHqR.Core.Extension.Loader.ServiceManager
{
    public static class ServiceManager
    {
        #region Fields
        
        private static readonly Dictionary<Type, object> Singletons = new Dictionary<Type, object>();
        
        private static readonly Dictionary<(string, Type), object> Instances = new Dictionary<(string, Type), object>();

        private static readonly Dictionary<(string,Type), Func<object>> Boxes = new Dictionary<(string, Type), Func<object>>();

        private static readonly HashSet<string> EventEmitters = new HashSet<string>();

        private static readonly List<(string, Action<object>, int)> Events = new List<(string, Action<object>, int)>();

        private static int _eventId = Int32.MinValue;

        #endregion

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void Init()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            typeof(IServiceManager).GetField("_RegisterSingleton", flags).SetValue(null,  new Action<object>(RegisterSingleton));
            typeof(IServiceManager).GetField("_RegisterInstance", flags).SetValue(null, new Action<object, string>(RegisterInstance));
            typeof(IServiceManager).GetField("_RegisterValue", flags).SetValue(null, new Action<Func<object>, Type, string>(RegisterValue));
            typeof(IServiceManager).GetField("_RegisterEvent", flags).SetValue(null, new Action<string>(RegisterAnonymousEvent));
            typeof(IServiceManager).GetField("_RegisterEventListener", flags).SetValue(null, new Action<string, Action<object>>(RegisterAnonymousEventListener));
            typeof(IServiceManager).GetField("_GetSingleton", flags).SetValue(null, new Converter<Type, object>(GetSingleton));
            typeof(IServiceManager).GetField("_GetInstance", flags).SetValue(null, new Converter<(Type,string), object>(GetInstance));
            typeof(IServiceManager).GetField("_GetValue", flags).SetValue(null, new Converter<(Type, string), object>(GetValue));
            typeof(IServiceManager).GetField("_CallEvent", flags).SetValue(null, new Action<string,object>(CallAnonymousEvent));
            ILogManager.Log("Started service manager...", null, LogType.Information);
        }

        #region Object Sharing

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterSingleton(object instance)
        {
            lock (Singletons)
            {
                if(Singletons.ContainsKey(instance.GetType())) throw new Exception($"Singleton of type {instance.GetType().Name} is already registered.");

                Singletons.Add(instance.GetType(), instance);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetSingleton(Type type)
        {
            lock (Singletons)
            {
                if (!Singletons.TryGetValue(type, out var val))
                {
                    throw new Exception($"Singleton of type {type.Name} not found.");
                }
                else
                {
                    return val;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterInstance(object instance, string id)
        {
            lock (Instances)
            {
                if (Instances.TryGetValue((id, instance.GetType()), out var duplicate))
                {
                    throw new Exception($"An object of type {instance.GetType().Name} with an ID of \"{id}\" is already registered.");
                }

                Instances.Add((id, instance.GetType()), instance);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetInstance((Type, string) binding)
        {
            lock (Instances)
            {
                var (type, name) = binding;
                if(!Instances.TryGetValue((name, type), out var result)) throw new Exception($"There is no \"{type.Name}\" with an ID of \"{name}\".");
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterValue(Func<object> boxedValue, Type type, string id)
        {
            lock (Boxes)
            {
                if(Boxes.TryGetValue((id, type), out _)) throw new Exception($"Box with the ID of \"{id}\" and type of {type.Name} already registered.");
                Boxes.Add((id,type), boxedValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValue((Type type, string id) binding)
        {
            lock (Boxes)
            {
                var (type, id) = binding;
                if (!Boxes.TryGetValue((id, type), out var get)) throw new Exception($"No value with type {type.Name} and id \"{id}\" found.");
                return get();
            }
        }

        #endregion

        #region Events

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterAnonymousEvent(string id)
        {
            lock(EventEmitters)
            {
                if(EventEmitters.Contains(id)) throw new Exception($"Event with the ID of \"{id}\" already exists.");
                EventEmitters.Add(id);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CallAnonymousEvent(string id, object value)
        {
            lock (EventEmitters) if(!EventEmitters.Contains(id)) throw new Exception($"Event with the ID of \"{id}\" not found.");
            IEnumerable<(string, Action<object>, int)> destinations = null;
            lock (Events) destinations = Events.Where(evt => evt.Item1.Equals(id));
            foreach (var destination in destinations) destination.Item2?.Invoke(value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterAnonymousEventListener(string eventId, Action<object> invoker)
        {
            lock (EventEmitters)
            {
                if(!EventEmitters.Contains(eventId)) throw new Exception($"There is no event with an ID of \"{eventId}\".");
            }
            lock (Events)
            {
                Events.Add((eventId, invoker, Interlocked.Increment(ref _eventId)));
            }
        }

        #endregion
    }
}
