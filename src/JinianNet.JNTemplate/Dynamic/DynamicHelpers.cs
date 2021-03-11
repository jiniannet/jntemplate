/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using System;
using System.Collections;
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
            PropertyInfo p = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Storage.BindIgnoreCase);
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
            FieldInfo f = type.GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Storage.BindIgnoreCase);

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
            IEnumerable<MethodInfo> ms = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Runtime.Storage.BindIgnoreCase);
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (MethodInfo m in ms)
            {
                if (m.Name.Equals(methodName, Runtime.Storage.ComparisonIgnoreCase))
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

        /// <summary>
        /// 获取属性或字段的值
        /// </summary>
        /// <param name="container">原对象</param>
        /// <param name="propName">属性或字段名，有参数属性为参数值</param>
        /// <returns></returns>
        public static object CallPropertyOrField(object container, string propName)
        {
            Type t = container.GetType();
            //此处的属性包括有参属性（索引）与无参属性（属性）
            //if (propName.IndexOfAny(indexExprStartChars) < 0)
            //因属性与字段均不可能以数字开头，如第一个字符为数字则直接跳过属性判断以加快处理速度
            if (!char.IsDigit(propName[0]))
            {
#if !NET20_NOTUSER
                PropertyInfo p = DynamicHelpers.GetPropertyInfo(t, propName);
                //取属性
                if (p != null)
                {
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //取字段
                FieldInfo f = t.GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |_bindingIgnoreCase);
                if (f != null)
                {
                    return f.GetValue(container);
                }
#endif
#else
                System.Linq.Expressions.MemberExpression exp;
#if NEEDFIELD
                exp = System.Linq.Expressions.Expression.PropertyOrField(System.Linq.Expressions.Expression.Constant(container), propName);
#else
                exp = System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(container), propName);
#endif
                if (exp != null)
                {
                    return System.Linq.Expressions.Expression.Lambda(exp).Compile().DynamicInvoke();
                }
#endif
            }

            return null;
        }


        /// <summary>
        /// 调用实例方法
        /// </summary>
        /// <param name="container">实例对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">形参</param>
        /// <returns>object</returns>
        public static object CallMethod(object container, string methodName, object[] args)
        {
            Type[] types = new Type[args.Length];
            bool hasNullValue = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    types[i] = args[i].GetType();
                }
                else
                {
                    hasNullValue = true;
                }
            }

            Type t = container.GetType();
            MethodInfo method = DynamicHelpers.GetMethod(t, methodName, types);

            if (method != null)
            {
                //if (hasParam)
                //{
                //    Array arr;
                //    if (types.Length - 1 == args.Length)
                //    {
                //        arr = null;
                //    }
                //    else
                //    {
                //        arr = Array.CreateInstance(types[types.Length - 1].GetElementType(), args.Length - types.Length + 1);
                //        for (int i = types.Length - 1; i < args.Length; i++)
                //        {
                //            arr.SetValue(args[i], i - (types.Length - 1));
                //        }

                //        object[] newArgs = new object[types.Length];

                //        for (int i = 0; i < newArgs.Length - 1; i++)
                //        {
                //            newArgs[i] = args[i];
                //        }
                //        newArgs[newArgs.Length - 1] = arr;

                //        return method.Invoke(container, newArgs);
                //    }
                //}

                ParameterInfo[] pi = method.GetParameters();
                //处理可选参数
                if (types.Length == 1 && types[0] == typeof(Dictionary<object, object>)
                    && (pi.Length != 1 || !pi[0].ParameterType.IsSubclassOf(typeof(IDictionary))))
                {
                    //实参
                    args = DynamicHelpers.ChangeParameters((Dictionary<object, object>)args[0], pi);
                }
                else if (hasNullValue)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == null && pi[i].DefaultValue != null)
                        {
                            args[i] = pi[i].DefaultValue;
                        }
                        //else
                        //{
                        //    args[i] = DefaultForType(pi[i].ParameterType);
                        //}
                    }
                }
                return method.Invoke(container, args);

            }

            return null;
        }
        /// <summary>
        /// 动态获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="propIndex">索引</param>
        /// <returns>返回结果</returns>
        public static object CallIndexValue(object container, object propIndex)
        {
            IList list;
            if (propIndex is int && (list = container as IList) != null)
            {
                return list[(int)propIndex];
            }
            IDictionary dic;
            if ((dic = container as IDictionary) != null)
            {
                return dic[propIndex];
            }
            if (propIndex is int && container is string)
            {
                return ((string)container)[(int)propIndex];
            }
            Type t = container.GetType();
            var info = DynamicHelpers.GetPropertyGetMethod(t, "Item");
            if (info != null)
            {
                return info.Invoke(container, new object[] { propIndex });
            }
            return null;
        }
    }
}
