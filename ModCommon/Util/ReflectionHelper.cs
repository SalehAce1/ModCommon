﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using MonoMod.Utils;

namespace ModCommon.Util
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> Fields =
            new Dictionary<Type, Dictionary<string, FieldInfo>>();

        public static T GetAttr<T>(object obj, string name, bool instance = true)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return default(T);

            Type t = obj.GetType();

            if (!Fields.ContainsKey(t))
            {
                Fields.Add(t, new Dictionary<string, FieldInfo>());
            }

            Dictionary<string, FieldInfo> typeFields = Fields[t];

            if (!typeFields.ContainsKey(name))
            {
                typeFields.Add(name,
                    t.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public |
                        (instance ? BindingFlags.Instance : BindingFlags.Static)));
            }

            return (T) typeFields[name]?.GetValue(obj);
        }

        public static void SetAttr<T>(object obj, string name, T val, bool instance = true)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return;

            Type t = obj.GetType();

            if (!Fields.ContainsKey(t))
            {
                Fields.Add(t, new Dictionary<string, FieldInfo>());
            }

            Dictionary<string, FieldInfo> typeFields = Fields[t];

            if (!typeFields.ContainsKey(name))
            {
                typeFields.Add(name,
                    t.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public |
                        (instance ? BindingFlags.Instance : BindingFlags.Static)));
            }

            typeFields[name]?.SetValue(obj, val);
        }
        
        private static readonly Dictionary<Type, Dictionary<string, MethodInfo>> METHOD_INFOS =
            new Dictionary<Type, Dictionary<string, MethodInfo>>();

        public static MethodInfo GetMethodInfo(object obj, string name, bool instance = true)
        {
            if (obj == null || string.IsNullOrEmpty(name)) return null;

            Type t = obj.GetType();

            if (!METHOD_INFOS.ContainsKey(t))
            {
                METHOD_INFOS.Add(t, new Dictionary<string, MethodInfo>());
            }

            Dictionary<string, MethodInfo> typeInfos = METHOD_INFOS[t];

            if (!typeInfos.ContainsKey(name))
            {
                typeInfos.Add(name,
                    t.GetMethod(name,
                        BindingFlags.NonPublic | BindingFlags.Public |
                        (instance ? BindingFlags.Instance : BindingFlags.Static)));
            }

            return typeInfos[name];
        }

        public static T InvokeMethod<T>(object obj, string name, bool instance = true, params object[] args)
        {
            return (T) obj.GetMethodInfo(name, instance).GetFastDelegate().Invoke(obj, args);
        }
    }

    public static class ReflectionExtensions
    {
        public static void SetAttr<T>(this object obj, string name, T val, bool instance = true) =>
            ReflectionHelper.SetAttr(obj, name, val, instance);

        public static T GetAttr<T>(this object obj, string name, bool instance = true) =>
            ReflectionHelper.GetAttr<T>(obj, name, instance);

        public static MethodInfo GetMethodInfo(this object obj, string name, bool instance = true) =>
            ReflectionHelper.GetMethodInfo(obj, name, instance);
        
        public static object InvokeMethod<T>(this object obj, string name, bool instance = true, params object[] args) => 
            ReflectionHelper.InvokeMethod<T>(obj, name, instance, args);
    }
}