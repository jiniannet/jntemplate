/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define NEEDFIELD
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// DynamicHelpers
    /// </summary>
    public class DynamicHelpers
    {
        private static ConcurrentDictionary<string, MethodInfo[]> dict = new ConcurrentDictionary<string, MethodInfo[]>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Searches for the public property with the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the public property to get.</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            PropertyInfo p = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            return p;
        }

        /// <summary>
        /// Returns the public get accessor for specified property.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the public property to get.</param>
        /// <returns></returns>
        public static MethodInfo GetPropertyGetMethod(Type type, string name)
        {
            PropertyInfo p = GetPropertyInfo(type, name);
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
        /// Searches for the public field with the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the data field to get.</param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Type type, string name)
        {
            FieldInfo f = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);

            return f;
        }

        /// <summary>
        /// Returns all the public members of the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <returns></returns>
        public static MethodInfo[] GetCacheMethods(Type type, string name)
        {
            var cacheKey = string.Concat(type.FullName, ".", name);
            return dict.GetOrAdd(cacheKey, (key) =>
            {
                return GetMethods(type, name);
            });
        }

        /// <summary>
        ///Returns all the public members of the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <returns></returns>
        public static MethodInfo[] GetMethods(Type type, string name)
        {
            IEnumerable<MethodInfo> ms = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            List<MethodInfo> result = new List<MethodInfo>();
            foreach (MethodInfo m in ms)
            {
                if (m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(m);
                }
            }
            return result.ToArray();
        }


        /// <summary>
        /// Searches for the public or private method with the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
        /// <param name="isStrict">Whether the parameter types are identical.</param>
        /// <returns>The <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetMethod(Type type, string name, Type[] args, bool isStrict)
        {
            MethodInfo[] ms = GetCacheMethods(type, name);
            if (ms.Length == 1)
            {
                return ms[0];
            }
            foreach (var m in ms)
            {
                if (IsMatch(m.GetParameters(), args, isStrict))
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Searches for the public or private method with the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
        /// <returns>The <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetMethod(Type type, string name, Type[] args)
        {
            return GetMethod(type, name, args, true);
        }

        /// <summary>
        /// Searches for the public or private generic method with the specified name.
        /// </summary>
        /// <param name="type">The type of object.</param>
        /// <param name="genericType">The generic types of method.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
        /// <returns>The <see cref="MethodInfo"/>.</returns>
        public static MethodInfo GetGenericMethod(Type type, Type[] genericType, string name, Type[] args)
        {
            MethodInfo[] ms = GetMethods(type, name);
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
        /// Whether the parameter types are identical
        /// </summary>
        /// <param name="pi">The parameter array.</param>
        /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
        /// <param name="isAllMatch"></param>
        /// <returns>bool</returns>
        public static bool IsMatch(ParameterInfo[] pi, Type[] args, bool isAllMatch)
        {
            if (pi.Length != args.Length)
            {
                return false;
            }
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    continue;
                }
                if ((isAllMatch && args[i] != pi[i].ParameterType) || (!isAllMatch && !IsMatchType(args[i], pi[i].ParameterType)) /*&& !DynamicHelpers.CanChange(args[i], pi[i].ParameterType)*/)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the parameter types are identical
        /// </summary>
        /// <param name="pi">The parameter array.</param>
        /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
        /// <returns>bool</returns>
        public static bool IsMatch(ParameterInfo[] pi, Type[] args)
        {
            return IsMatch(pi, args, false);
        }


        /// <summary>
        /// Indicates whether finds a match in a specified input type.
        /// </summary>
        /// <param name="original">The original tag.</param>
        /// <param name="target">The target tag.</param>
        /// <returns></returns>
        public static bool IsMatchType(Type original, Type target)
        {
            return original == target || original.IsSubclassOf(target) || target.IsAssignableFrom(original);
        }

        /// <summary>
        /// Whether the original type can be converted to the target type
        /// </summary>
        /// <param name="original">The original type.</param>
        /// <param name="target">The target type.</param>
        /// <returns>bool</returns>
        public static bool CanChange(Type original, Type target)
        {
            switch (target.FullName)
            {
                //case "System.String"://
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
        /// Gets the default value for the specified type.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/>.</param>
        /// <returns></returns>
        private static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? CreateInstance(targetType) : null;
        }

        /// <summary>
        /// Convert dictionary to matching parameters
        /// </summary>
        /// <param name="data">The dictionary. </param>
        /// <param name="pis">The parameter types.</param>
        /// <returns>object[]</returns>
        public static object[] ChangeParameters(Dictionary<object, object> data, ParameterInfo[] pis)
        {
            var args = new object[pis.Length];
            for (int i = 0; i < pis.Length; i++)
            {
                foreach (var kv in data)
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
        /// Creates an instance of the type whose name is specified. 
        /// </summary>
        /// <typeparam name="T">The result of type.</typeparam>
        /// <param name="typeName">Thpe type name.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return default(T);
            }
            var type = Type.GetType(typeName);
            if (type == null)
            {
                return default(T);
            }
            return (T)Activator.CreateInstance(type);
        }
        /// <summary>
        /// Creates an instance of the specified type. 
        /// </summary>
        /// <typeparam name="T">The result of type.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type ?? typeof(T));
        }

        /// <summary>
        /// Creates an instance of the specified type. 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type.IsInterface)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the property or field value of the object.
        /// </summary>
        /// <param name="container">The object.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="name">The property or field name. </param>
        /// <returns></returns>
        public static object CallPropertyOrField(object container, string name, Type type = null)
        {
            Type t = type ?? container.GetType();
            if (!char.IsDigit(name[0]))
            {
#if !NET20_NOTUSER
                PropertyInfo p = DynamicHelpers.GetPropertyInfo(t, name);
                //Property
                if (p != null)
                {
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //Field
                FieldInfo f = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                if (f != null)
                {
                    return f.GetValue(container);
                }
#endif
#else
                System.Linq.Expressions.MemberExpression exp;
#if NEEDFIELD
                exp = System.Linq.Expressions.Expression.PropertyOrField(System.Linq.Expressions.Expression.Constant(container), name);
#else
                exp = System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(container), name);
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
        /// Calls the specified method and returns the result of execution
        /// </summary>
        /// <param name="container">Then Instance objects.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="args">The parameter of the method.</param>
        /// <returns>The result of execution.</returns>
        public static object CallMethod(object container, string name, object[] args)
        {
            return CallMethod(container?.GetType(), container, name, args);
        }
        /// <summary>
        /// Calls the specified method and returns the result of execution
        /// </summary>
        /// <param name="type">Then Instance objects.</param>
        /// <param name="container">Then Instance objects.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="args">The parameter of the method.</param>
        /// <returns>The result of execution.</returns>
        public static object CallMethod(Type type, object container, string name, object[] args)
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

            MethodInfo method = DynamicHelpers.GetMethod(type, name, types);

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

                var pi = method.GetParameters();
                if (types.Length == 1 && types[0] == typeof(Dictionary<object, object>)
                    && (pi.Length != 1 || !pi[0].ParameterType.IsSubclassOf(typeof(IDictionary))))
                {
                    args = DynamicHelpers.ChangeParameters((Dictionary<object, object>)args[0], pi);
                }
                else if (hasNullValue)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == null
                            && !pi[i].ParameterType.IsClass
                            && pi[i].DefaultValue != DBNull.Value)
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
        /// Get the index value.
        /// </summary>
        /// <param name="container">The object.</param>
        /// <param name="propIndex"> The zero-based index in the object. </param>
        /// <returns></returns>
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
