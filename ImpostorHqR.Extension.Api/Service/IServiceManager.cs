using System;
using System.Runtime.CompilerServices;

namespace ImpostorHqR.Extension.Api.Service
{
    public interface IServiceManager
    {
        #region Bindings

        #pragma warning disable
        // ReSharper disable InconsistentNaming

        private static Action<object> _RegisterSingleton;

        private static Action<object, string> _RegisterInstance;

        private static Action<Func<object>, Type, string> _RegisterValue;

        private static Action<string> _RegisterEvent;

        private static Action<string, Action<object>> _RegisterEventListener;


        private static Converter<Type, object> _GetSingleton;

        private static Converter<(Type, string), object> _GetInstance;

        private static Converter<(Type, string), object> _GetValue;

        private static Action<string, object> _CallEvent;
       
        // ReSharper restore InconsistentNaming
        #pragma warning restore

        #endregion

        #region Object Sharing

        static void RegisterSingleton<T>(T instance) where T:notnull
        {
            _RegisterSingleton?.Invoke(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T GetSingleton<T>() => (T) _GetSingleton(typeof(T));

        static void RegisterInstance<T>(T instance, string name) => _RegisterInstance?.Invoke(instance!, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T GetInstance<T>(string name) where T : notnull => (T) _GetInstance?.Invoke((typeof(T), name));

        static void RegisterValue<T>(Func<T> getObject, string name) => _RegisterValue?.Invoke(() => getObject(), typeof(T), name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T GetValue<T>(string name) where T : notnull => (T) _GetValue?.Invoke((typeof(T), name));

        #endregion

        #region Events

        static void RegisterAnonymousEvent(string name) => _RegisterEvent?.Invoke(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CallAnonymousEvent(string name, object value) => _CallEvent?.Invoke(name, value);

        static void ListenForAnonymousEvent(string name, Action<object> caller) => _RegisterEventListener?.Invoke(name, caller);

        #endregion
    }
}
