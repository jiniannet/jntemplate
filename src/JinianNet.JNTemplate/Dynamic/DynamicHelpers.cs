/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 反射HELPERS
    /// </summary>
    public class DynamicHelpers
    {
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propName">属性名称</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type type, string propName)
        {
            PropertyInfo p = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Store.BindIgnoreCase);
            return p;
        }

        /// <summary>
        /// 获取属性的GET方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propName">属性名称</param>
        /// <returns></returns>
        public static MethodInfo GetPropertyGetMethod(Type type, string propName)
        {
            PropertyInfo p = GetPropertyInfo(type,propName);
            if (p == null)
            {
                return null;
            }
#if NET40 || NET20
            return p.GetGetMethod();
#else
            return p.GetMethod;
#endif
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="propName">属性名称</param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Type type, string propName)
        {
            FieldInfo f = type.GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Store.BindIgnoreCase);

            return f;
        }

        /// <summary>
        /// 根据方法名查找方法(缓存)
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        public static MethodInfo[] GetCacheMethods(Type type, string methodName)
        {
            var cacheKey = string.Concat(type.FullName, ".", methodName);
            var cacheValue = Runtime.Cache.Get<MethodInfo[]>(cacheKey);
            if (cacheValue != null)
            {
                return cacheValue;
            }
            var result = GetMethods(type, methodName);
            Runtime.Cache.Set(cacheKey, result);
            return result;
        }

        /// <summary>
        /// 根据方法名查找方法（无缓存）
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(Type type, string methodName)
        {
            IEnumerable<MethodInfo> ms = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Store.BindIgnoreCase);
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (MethodInfo m in ms)
            {
                if (m.Name.Equals(methodName, Runtime.Store.ComparisonIgnoreCase))
                {
                    result.Add(m);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 根据参数获取方法（请避免使用重载）
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetMethod(Type type, string methodName, Type[] args)
        {
            MethodInfo[] ms = GetCacheMethods(type, methodName);
            if (ms.Length == 1)
            {
                return ms[0];
            }
            foreach (var m in ms)
            {

            }
            foreach (var m in ms)
            {
                if (IsMatch(m.GetParameters(), args))
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据参数获取方法（请避免使用重载）
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="genericType">类型</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetGenericMethod(Type type, Type[] genericType, string methodName, Type[] args)
        {
            MethodInfo[] ms = GetCacheMethods(type, methodName);
            if (ms.Length == 1 && ms[0].IsGenericMethod)
            {
                return ms[0].MakeGenericMethod(genericType);
            }
            foreach (var m in ms)
            {
                if (!m.IsGenericMethod)
                {
                    continue;
                }
                var real = m.MakeGenericMethod(genericType);
                if (IsMatch(real.GetParameters(), args))
                {
                    return real;
                }
            }
            return null;
        }


        /// <summary>
        /// 根据参数获取动态方法（请尽量避免使用重载）
        /// </summary>
        /// <param name="ms">动态方法数组</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>MethodInfo</returns>
        public static DynamicMethodInfo GetDynamicMethod(string methodName, DynamicMethodInfo[] ms, Type[] args)
        {
            foreach (var m in ms)
            {
                if (IsMatch(m.Parameters, args))
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// 实参是否匹配形参
        /// </summary>
        /// <param name="pi">形参</param>
        /// <param name="args">实参</param>
        /// <param name="isAllMatch">参数类型是否完全一致</param>
        /// <returns>bool</returns>
        public static bool IsMatch(ParameterInfo[] pi, Type[] args, bool isAllMatch)
        {
            if (pi.Length != args.Length)
            {
                return false;
            }
            //暂不考虑可选参数,默认参数,param参数
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    continue;
                }
                //if (!IsMatchType(args[i], pi[i].ParameterType) && !DynamicHelpers.CanChange(args[i], pi[i].ParameterType))
                if ((isAllMatch && args[i].FullName != pi[i].ParameterType.FullName) || (!isAllMatch && !IsMatchType(args[i], pi[i].ParameterType)) /*&& !DynamicHelpers.CanChange(args[i], pi[i].ParameterType)*/)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 实参是否匹配形参
        /// </summary>
        /// <param name="pi">形参</param>
        /// <param name="args">实参</param>
        /// <returns>bool</returns>
        public static bool IsMatch(ParameterInfo[] pi, Type[] args)
        {
            return IsMatch(pi, args, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsMatchType(Type original, Type target)
        {
            return original == target || original.IsSubclassOf(target) || target.IsAssignableFrom(original);
        }

        /// <summary>
        /// 原始类型是否可以向目标类型转换
        /// </summary>
        /// <param name="original">原始类型</param>
        /// <param name="target">目标类型</param>
        /// <returns>bool</returns>
        public static bool CanChange(Type original, Type target)
        {
            switch (target.FullName)
            {
                //case "System.String"://任意类型都支持toString
                //    return true;
                case "System.Double":
                    if (original.FullName == "System.Int16"
                        || original.FullName == "System.Int32"
                        || original.FullName == "System.Int64"
                        || original.FullName == "System.Single")
                    {
                        return true;
                    }
                    return false;
                case "System.Single":
                    if (original.FullName == "System.Int16"
                        || original.FullName == "System.Int32"
                        || original.FullName == "System.Int64"
                        || original.FullName == "System.Double")
                    {
                        return true;
                    }
                    return false;
                case "System.Int32":
                    if (original.FullName == "System.Int16"
                        || original.FullName == "System.Int64"
                        || original.FullName == "System.Double"
                        || original.FullName == "System.Single")
                    {
                        return true;
                    }
                    return false;
                case "System.Int64":
                    if (original.FullName == "System.Int16"
                        || original.FullName == "System.Int32"
                        || original.FullName == "System.Double"
                        || original.FullName == "System.Single")
                    {
                        return true;
                    }
                    return false;
                case "System.Int16":
                    if (original.FullName == "System.Int32"
                        || original.FullName == "System.Int64"
                        || original.FullName == "System.Double"
                        || original.FullName == "System.Single")
                    {
                        return true;
                    }
                    return false;
                case "System.Decimal":
                    if (original.FullName == "System.Int16"
                        || original.FullName == "System.Int32"
                        || original.FullName == "System.Int64"
                        || original.FullName == "System.Double"
                        || original.FullName == "System.Single")
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }


        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 参数转换
        /// </summary>
        /// <param name="dict">数据</param>
        /// <param name="pis">参数</param>
        /// <returns>object[]</returns>
        public static object[] ChangeParameters(Dictionary<object, object> dict, ParameterInfo[] pis)
        {
            //实参
            var args = new object[pis.Length];
            //处理实参
            for (int i = 0; i < pis.Length; i++)
            {
                foreach (var kv in dict)
                {
                    if (pis[i].Name.Equals(kv.Key.ToString()))
                    {
                        args[i] = kv.Value;
                    }
                }

                if (args[i] == null)
                {
                    if (pis[i].DefaultValue != null && pis[i].DefaultValue.GetType().Name != "DBNull")
                    {
                        args[i] = pis[i].DefaultValue;
                    }
                    else
                    {
                        args[i] = DefaultForType(pis[i].ParameterType);
                    }
                }
            }
            return args;
        }

        /// <summary>
        /// 创建实例 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="typeName">类型</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return default(T);
            }
            var type = Type.GetType(typeName);
            if(type == null)
            {
                return default(T);
            }
            return (T)Activator.CreateInstance(type);
        }
        /// <summary>
        /// 创建实例 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type ?? typeof(T));
        }

        /// <summary>
        /// 创建实例 
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例对象</returns>
        public static object CreateInstance(Type type)
        {
            if (type.IsInterface)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }
    }
}
