using JinianNet.JNTemplate.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
            PropertyInfo p =
#if NETSTANDARD
                    type.GetRuntimeProperty(propName);
#else
                    type.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
#endif
            return p;
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
            var cacheValue = CacheHelpers.Get<MethodInfo[]>(cacheKey);
            if (cacheValue != null)
            {
                return cacheValue;
            }
            var result = GetMethods(type, methodName);
            CacheHelpers.Set(cacheKey, result);
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
            IEnumerable<MethodInfo> ms = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (MethodInfo m in ms)
            {
                if (m.Name.Equals(methodName, Engine.Runtime.ComparisonIgnoreCase))
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
                if (IsMatch(m.GetParameters(), args))
                {
                    return m;
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
        /// <returns>bool</returns>
        public static bool IsMatch(ParameterInfo[] pi, Type[] args)
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
                if (args[i] != pi[i].ParameterType && !args[i].IsSubclassOf(pi[i].ParameterType) && !DynamicHelpers.CanChange(args[i], pi[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
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
                case "System.String"://任意类型都支持toString
                    return true;
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
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
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
    }
}
