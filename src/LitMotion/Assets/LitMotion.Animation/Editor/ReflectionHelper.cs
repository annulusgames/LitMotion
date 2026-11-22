using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion.Animation.Editor
{
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Immutable reflection caches.
#endif
    internal static class ReflectionHelper
    {
        static readonly Dictionary<(Type, string, BindingFlags, bool), FieldInfo> cacheFieldInfo = new();
        static readonly Dictionary<(Type, string, BindingFlags, bool), MethodInfo> cacheMethodInfo = new();
        static readonly Dictionary<(Type, string, BindingFlags, bool), PropertyInfo> cachePropertyInfo = new();
        static readonly Dictionary<(Type, BindingFlags, bool), MemberInfo[]> cacheAllMembers = new();

        static readonly Dictionary<(Type, string), Func<object, object>> cacheGetFieldValue = new();
        static readonly Dictionary<(Type, string), Func<object, object>> cacheGetPropertyValue = new();
        static readonly Dictionary<(Type, string), Func<object, object>> cacheGetMethodValue = new();

        public static Func<object, object> CreateGetter(FieldInfo fieldInfo)
        {
            if (fieldInfo == null) return null;
            if (fieldInfo.IsStatic)
            {
                var body = Expression.Convert(Expression.MakeMemberAccess(null, fieldInfo), typeof(object));
                var lambda = Expression.Lambda<Func<object>>(body).Compile();
                return _ => lambda();
            }
            if (fieldInfo.DeclaringType != null)
            {
                var objParam = Expression.Parameter(typeof(object), "obj");
                var tParam = Expression.Convert(objParam, fieldInfo.DeclaringType);
                var body = Expression.Convert(Expression.MakeMemberAccess(tParam, fieldInfo), typeof(object));
                return Expression.Lambda<Func<object, object>>(body, objParam).Compile();
            }
            return null;
        }

        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return null;
            if (propertyInfo.GetGetMethod(true).IsStatic)
            {
                var body = Expression.Convert(Expression.MakeMemberAccess(null, propertyInfo), typeof(object));
                var lambda = Expression.Lambda<Func<object>>(body).Compile();
                return _ => lambda();
            }
            if (propertyInfo.DeclaringType != null)
            {
                var objParam = Expression.Parameter(typeof(object), "obj");
                var tParam = Expression.Convert(objParam, propertyInfo.DeclaringType);
                var body = Expression.Convert(Expression.MakeMemberAccess(tParam, propertyInfo), typeof(object));
                return Expression.Lambda<Func<object, object>>(body, objParam).Compile();
            }
            return null;
        }

        public static Func<object, object> CreateGetter(MethodInfo methodInfo)
        {
            if (methodInfo == null) return null;
            if (methodInfo.IsStatic)
            {
                var body = Expression.Convert(Expression.Call(null, methodInfo), typeof(object));
                var lambda = Expression.Lambda<Func<object>>(body).Compile();
                return _ => lambda();
            }
            if (methodInfo.DeclaringType != null)
            {
                var objParam = Expression.Parameter(typeof(object), "obj");
                var tParam = Expression.Convert(objParam, methodInfo.DeclaringType);
                var body = Expression.Convert(Expression.Call(tParam, methodInfo), typeof(object));
                return Expression.Lambda<Func<object, object>>(body, objParam).Compile();
            }
            return null;
        }

        public static object GetFieldValue(object target, Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (!cacheGetFieldValue.TryGetValue((type, name), out var value))
            {
                var info = type.GetField(name, bindingAttr);
                value = CreateGetter(info);
                cacheGetFieldValue.Add((type, name), value);
            }
            return value?.Invoke(target);
        }

        public static object GetPropertyValue(object target, Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (!cacheGetPropertyValue.TryGetValue((type, name), out var value))
            {
                var info = type.GetProperty(name, bindingAttr);
                value = CreateGetter(info);
                cacheGetPropertyValue.Add((type, name), value);
            }
            return value?.Invoke(target);
        }

        public static object GetMethodValue(object target, Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            if (!cacheGetMethodValue.TryGetValue((type, name), out var value))
            {
                var info = type.GetMethod(name, bindingAttr);
                value = CreateGetter(info);
                cacheGetMethodValue.Add((type, name), value);
            }
            return value?.Invoke(target);
        }

        public static FieldInfo GetField(Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, bool includingBaseNonPublic = false)
        {
            if (!cacheFieldInfo.TryGetValue((type, name, bindingAttr, includingBaseNonPublic), out var info))
            {
                if (includingBaseNonPublic)
                {
                    info = GetAllFieldsIncludingBaseNonPublic(type, bindingAttr).FirstOrDefault(x => x.Name == name);
                }
                else
                {
                    info = type.GetField(name, bindingAttr);
                }
                cacheFieldInfo.Add((type, name, bindingAttr, includingBaseNonPublic), info);
            }
            return info;
        }

        static IEnumerable<Type> EnumerateTypeHierarchy(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static IEnumerable<FieldInfo> GetAllFieldsIncludingBaseNonPublic(Type type, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return EnumerateTypeHierarchy(type).Reverse().SelectMany(t => t.GetFields(bindingAttr | BindingFlags.DeclaredOnly));
        }

        public static PropertyInfo GetProperty(Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, bool includingBaseNonPublic = false)
        {
            if (!cachePropertyInfo.TryGetValue((type, name, bindingAttr, includingBaseNonPublic), out var info))
            {
                if (includingBaseNonPublic)
                {
                    info = GetAllPropertiesIncludingBaseNonPublic(type, bindingAttr).FirstOrDefault(x => x.Name == name);
                }
                else
                {
                    info = type.GetProperty(name, bindingAttr);
                }
                cachePropertyInfo.Add((type, name, bindingAttr, includingBaseNonPublic), info);
            }
            return info;
        }

        public static IEnumerable<PropertyInfo> GetAllPropertiesIncludingBaseNonPublic(Type type, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return EnumerateTypeHierarchy(type).Reverse().SelectMany(t => t.GetProperties(bindingAttr | BindingFlags.DeclaredOnly));
        }

        public static MethodInfo GetMethod(Type type, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, bool includingBaseNonPublic = false)
        {
            if (!cacheMethodInfo.TryGetValue((type, name, bindingAttr, includingBaseNonPublic), out var info))
            {
                if (includingBaseNonPublic)
                {
                    info = GetAllMethodsIncludingBaseNonPublic(type, bindingAttr).FirstOrDefault(x => x.Name == name);
                }
                else
                {
                    info = type.GetMethods(bindingAttr).Where(x => x.Name == name).FirstOrDefault();
                }
                cacheMethodInfo.Add((type, name, bindingAttr, includingBaseNonPublic), info);
            }
            return info;
        }

        public static IEnumerable<MethodInfo> GetAllMethodsIncludingBaseNonPublic(Type type, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return EnumerateTypeHierarchy(type).Reverse().SelectMany(t => t.GetMethods(bindingAttr | BindingFlags.DeclaredOnly));
        }

        public static object GetValue(object target, string name, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, bool allowProperty = true, bool allowMethod = true)
        {
            if (target == null) return null;
            Type type = target.GetType();
            object result;

            while (type != null)
            {
                result = GetFieldValue(target, type, name, bindingAttr);
                if (result != null) return result;

                if (allowProperty)
                {
                    result = GetPropertyValue(target, type, name, bindingAttr);
                    if (result != null) return result;
                }

                if (allowMethod)
                {
                    result = GetMethodValue(target, type, name, bindingAttr);
                    if (result != null) return result;
                }

                type = type.BaseType;
            }
            return null;
        }

        public static object GetValue(object target, string name, int index)
        {
            if (GetValue(target, name, allowMethod: false) is not IEnumerable enumerable) return null;
            IEnumerator enumerator = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) return null;
            }
            return enumerator.Current;
        }

        public static bool GetValueBool(object target, string name)
        {
            if (GetValue(target, name) is bool cond)
            {
                return cond;
            }
            return false;
        }

        public static MemberInfo[] GetMembers(Type type, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, bool includingBaseNonPublic = false)
        {
            if (cacheAllMembers.TryGetValue((type, bindingAttr, includingBaseNonPublic), out var memberInfoArray))
            {
                return memberInfoArray;
            }
            else
            {
                if (includingBaseNonPublic)
                {
                    memberInfoArray = GetMembersIncludingBaseNonPublic(type, bindingAttr).ToArray();
                }
                else
                {
                    memberInfoArray = type.GetMembers(bindingAttr);
                }
                cacheAllMembers.Add((type, bindingAttr, includingBaseNonPublic), memberInfoArray);
                return memberInfoArray;
            }
        }

        public static IEnumerable<MemberInfo> GetMembersIncludingBaseNonPublic(Type type, BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return EnumerateTypeHierarchy(type).Reverse().SelectMany(t => t.GetMembers(bindingAttr | BindingFlags.DeclaredOnly));
        }

        public static object Invoke(object target, string name, params object[] parameters)
        {
            if (target == null) return false;
            Type type = target.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            while (type != null)
            {
                var m = GetMethod(type, name, bindingAttr);
                if (m != null) return m.Invoke(m.IsStatic ? null : target, parameters);

                type = type.BaseType;
            }
            return false;
        }

        public static bool TryInvoke(object target, string name, object[] parameters, out object result)
        {
            if (target == null)
            {
                result = default;
                return false;
            }

            Type type = target.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            while (type != null)
            {
                var m = GetMethod(type, name, bindingAttr);
                if (m != null)
                {
                    result = m.Invoke(m.IsStatic ? null : target, parameters);
                    return true;
                }

                type = type.BaseType;
            }

            result = default;
            return false;
        }

        public static object GetCollectionValue(object target, int index)
        {
            var type = target.GetType();

            if (type.IsArray) return ((Array)target).GetValue(index);
            else if (target is IList list) return list[index];

            return null;
        }

        public static object CreateDefaultInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type == typeof(string)) return "";
            if (type.IsSubclassOf(typeof(UnityEngine.Object))) return null;
            return Activator.CreateInstance(type);
        }

    }
}