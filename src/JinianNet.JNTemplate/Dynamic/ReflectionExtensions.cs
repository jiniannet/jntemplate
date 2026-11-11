/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
#define NEEDFIELD
//#define USEEXPRESSION
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using JinianNet.JNTemplate.Exceptions;

#if !NF20
using System.Linq.Expressions;
#endif

namespace JinianNet.JNTemplate.Dynamic;

/// <summary>
/// 
/// </summary>
public static class ReflectionExtensions
{
    private static ConcurrentDictionary<string, object> dict = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// Searches for the public property with the specified name.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the public property to get.</param>
    /// <returns></returns>
    public static PropertyInfo? GetPropertyInfo(this Type type, string name)
        => type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);


    /// <summary>
    /// Returns the public get accessor for specified property.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the public property to get.</param>
    /// <returns></returns>
    public static MethodInfo? GetPropertyGetMethod(this Type type, string name)
    {
        var p = GetPropertyInfo(type, name);
        if (p == null)
        {
            return null;
        }
        return GetPropertyGetMethod(p);
    }

    /// <summary>
    /// Returns the public get accessor for specified property.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static MethodInfo? GetPropertyGetMethod(this PropertyInfo p)
    {
#if NF40 || NF35 || NF20
        return p?.GetGetMethod();
#else
        return p?.GetMethod;
#endif
    }
    /// <summary>
    /// Returns the public get accessor for specified property.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the public property to set.</param>
    /// <returns></returns>
    public static MethodInfo? GetPropertySetMethod(this Type type, string name)
    {
        var p = GetPropertyInfo(type, name);
        return GetPropertySetMethod(p);
    }


    /// <summary>
    /// Returns the public get accessor for specified property.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static MethodInfo? GetPropertySetMethod(this PropertyInfo? p)
    {
#if NF40 || NF35 || NF20
        return p?.GetSetMethod();
#else
        return p?.SetMethod;
#endif
    }

    /// <summary>
    /// Searches for the public field with the specified name.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the data field to get.</param>
    /// <returns></returns>
    public static FieldInfo? GetFieldInfo(this Type type, string name)
        => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);


    /// <summary>
    /// Returns all the public members of the specified name.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the method to get.</param>
    /// <returns></returns>
    public static MethodInfo[] GetCacheMethods(this Type type, string name)
    {
        var cacheKey = $"MS${Utility.ToHashCode(type)}.{name}";
        return (MethodInfo[])dict.GetOrAdd(cacheKey, (key) =>
        {
            return GetMethodInfoArray(type, name);
        });
    }

    /// <summary>
    ///Returns all the public members of the specified name.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="name">The string containing the name of the method to get.</param>
    /// <returns></returns>
    public static MethodInfo[] GetMethodInfoArray(this Type type, string name)
    {
#if NET20
        var list = new List<MethodInfo>();
        var ms = GetMethodInfoArray(type);
        foreach (var m in ms)
        {
            if (m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                list.Add(m);
        }
        return ms;
#else

        return [.. GetMethodInfoArray(type).Where(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))];
#endif
    }

    private static MethodInfo[] GetMethodInfoArray(this Type type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static MethodInfo? GetExplicitOrImplicit(Type source, Type target)
    {
        var m = GetExplicitOrImplicit(source.GetMethods(BindingFlags.Public | BindingFlags.Static), source, target);
        if (m != null)
            return m;
        return GetExplicitOrImplicit(target.GetMethods(BindingFlags.Public | BindingFlags.Static), source, target);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static MethodInfo? GetExplicitOrImplicit(MethodInfo[] ms, Type source, Type target)
    {
        foreach (MethodInfo m in ms)
        {
            if (m.Name.Equals("op_Explicit", StringComparison.OrdinalIgnoreCase) || m.Name.Equals("op_Implicit", StringComparison.OrdinalIgnoreCase))
            {
                if (m.ReturnType != target)
                    continue;

                var ps = m.GetParameters();
                if (ps.Length == 1 && ps[0].ParameterType == source)
                {
                    return m;
                }
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
    /// <param name="isStrict">Whether the parameter types are identical.</param>
    /// <returns>The <see cref="MethodInfo"/>.</returns>
    public static MethodInfo? GetMethodInfo(this Type type, string name, Type?[]? args, bool isStrict)
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
    public static MethodInfo? GetMethodInfo(this Type type, string name, Type?[]? args)
        => GetMethodInfo(type, name, args, true);


    /// <summary>
    /// Searches for the public or private generic method with the specified name.
    /// </summary>
    /// <param name="type">The type of object.</param>
    /// <param name="genericType">The generic types of method.</param>
    /// <param name="name">The string containing the name of the method to get.</param>
    /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
    /// <returns>The <see cref="MethodInfo"/>.</returns>
    public static MethodInfo? GetGenericMethod(this Type type, Type[] genericType, string name, Type?[]? args)
    {
        MethodInfo[] ms = GetMethodInfoArray(type, name);
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
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <exception cref="TemplateException"></exception>
    /// <returns></returns>
    public static MethodInfo FindRequiredMethod(this Type type, string name)
        => FindRequiredMethod(type, name, null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo FindRequiredMethod(this Type type, string name, Type[]? types)
    {
        return GetMethodInfo(type, name, types, types != null) ?? throw new TemplateException($"The {type.Name} method '{name}' not found.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <exception cref="TemplateException"></exception>
    /// <returns></returns>
    public static MethodInfo GetRequiredMethod(this Type type, string name)
        => GetRequiredMethod(type, name, null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo GetRequiredMethod(this Type type, string name, Type[]? types)
    {
        //var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var method = types == null ? type.GetMethod(name) : type.GetMethod(name, types);
        return method ?? throw new TemplateException($"The {type.Name} method '{name}' not found.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="genericTypes"></param>
    /// <returns></returns>
    public static MethodInfo GetRequiredGenericMethod(this Type type, string name, Type[] genericTypes)
        => GetRequiredGenericMethod(type, name, null, genericTypes);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <param name="genericTypes"></param>
    /// <returns></returns>
    public static MethodInfo GetRequiredGenericMethod(this Type type, string name, Type[]? types, Type[] genericTypes)
    {
        MethodInfo? m = null;
        var ms = GetCacheMethods(type, name);
        if (ms.Length == 1 && ms[0].IsGenericMethod)
            m = ms[0];
        else
        {
            for (var i = 0; i < ms.Length; i++)
            {
                if (!ms[i].IsGenericMethod)
                    continue;
                if (types != null && ms[i].GetParameters().Length != types.Length)
                    continue;
                m = ms[i];
                break;
            }
        }
        var method = m ?? throw new TemplateException($"The {type.Name} method '{name}' not found.");
        return method.MakeGenericMethod(genericTypes);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="TemplateException"></exception>
    /// <returns></returns>
    public static ConstructorInfo GetRequiredConstructor(this Type type)
        => GetRequiredConstructor(type, null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static ConstructorInfo GetRequiredConstructor(this Type type, Type[]? types)
    {
        var method = types == null ? type.GetConstructor(Type.EmptyTypes) : type.GetConstructor(types);
        return method ?? throw new TemplateException($"The {type.Name} constructor not found.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static PropertyInfo GetRequiredProperty(this Type type, string name)
        => GetRequiredProperty(type, name, null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static PropertyInfo GetRequiredProperty(this Type type, string name, Type[]? types)
    {
        var pro = types == null ? type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
            : type.GetProperty(name, types);
        return pro ?? throw new TemplateException($"The {type.Name} property {name} not found.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MethodInfo GetRequiredPropertyGetMethod(this Type type, string name)
        => GetRequiredPropertyGetMethod(type, name, null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo GetRequiredPropertyGetMethod(this Type type, string name, Type[]? types)
    {
        var pro = GetRequiredProperty(type, name, types);
        return GetRequiredPropertyGetMethod(pro);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pro"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo GetRequiredPropertyGetMethod(this PropertyInfo pro)
    {
        return GetPropertyGetMethod(pro) ?? throw new TemplateException($"The property '{pro.Name}' cannot be get.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MethodInfo GetRequiredPropertySetMethod(this Type type, string name)
        => GetRequiredPropertySetMethod(type, name, null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo GetRequiredPropertySetMethod(this Type type, string name, Type[]? types)
    {
        var pro = GetRequiredProperty(type, name, types);
        return GetRequiredPropertySetMethod(pro);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pro"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static MethodInfo GetRequiredPropertySetMethod(this PropertyInfo pro)
    {
        return GetPropertySetMethod(pro) ?? throw new TemplateException($"The property '{pro.Name}' cannot be set.");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="TemplateException"></exception>
    public static Type GetRequiredInterface(this Type type, string name)
    {
        return type.GetInterface(name) ?? throw new TemplateException($"Interface '{name}' not found in type '{type.Name}'.");
    }

    /// <summary>
    /// Whether the parameter types are identical
    /// </summary>
    /// <param name="pi">The parameter array.</param>
    /// <param name="args">An array of <see cref="Type"/> objects representing the number, order, and type of the parameters for the method to get.</param>
    /// <param name="isAllMatch"></param>
    /// <returns>bool</returns>
    public static bool IsMatch(ParameterInfo[] pi, Type?[]? args, bool isAllMatch)
    {
        if (args == null)
            return pi.Length == 0;

        if (pi.Length != args.Length)
        {
            return false;
        }
        for (int i = 0; i < pi.Length; i++)
        {
            if (args[i] == null)
            {
                continue;
            }
            if ((isAllMatch && args[i] != pi[i].ParameterType) || (!isAllMatch && !IsMatchType(args[i]!, pi[i].ParameterType)) /*&& !DynamicHelpers.CanChange(args[i], pi[i].ParameterType)*/)
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
    public static bool IsMatch(ParameterInfo[] pi, Type?[]? args)
    {
        return IsMatch(pi, args, false);
    }


    /// <summary>
    /// Indicates whether finds a match in a specified input type.
    /// </summary>
    /// <param name="original">The original tag.</param>
    /// <param name="target">The target tag.</param>
    /// <returns></returns>
    public static bool IsMatchType(this Type original, Type target)
    {
        return original == target || original.IsSubclassOf(target) || target.IsAssignableFrom(original);
    }

    /// <summary>
    /// Whether the original type can be converted to the target type
    /// </summary>
    /// <param name="original">The original type.</param>
    /// <param name="target">The target type.</param>
    /// <returns>bool</returns>
    public static bool CanChange(this Type original, Type target)
    {
        var originalCode = original.ToTypeCode();
        var targetCode = target.ToTypeCode();
        switch (targetCode)
        {
            case TypeCode.Double:
                if (originalCode == TypeCode.Int16
                    || originalCode == TypeCode.Int32
                    || originalCode == TypeCode.Int64
                    || originalCode == TypeCode.Single)
                {
                    return true;
                }
                return false;
            case TypeCode.Single:
                if (originalCode == TypeCode.Int16
                    || originalCode == TypeCode.Int32
                    || originalCode == TypeCode.Int64
                    || originalCode == TypeCode.Double)
                {
                    return true;
                }
                return false;
            case TypeCode.Int32:
                if (originalCode == TypeCode.Int16
                    || originalCode == TypeCode.Int64
                    || originalCode == TypeCode.Double
                    || originalCode == TypeCode.Single)
                {
                    return true;
                }
                return false;
            case TypeCode.Int64:
                if (originalCode == TypeCode.Int16
                    || originalCode == TypeCode.Int32
                    || originalCode == TypeCode.Double
                    || originalCode == TypeCode.Single)
                {
                    return true;
                }
                return false;
            case TypeCode.Int16:
                if (originalCode == TypeCode.Int32
                    || originalCode == TypeCode.Int64
                    || originalCode == TypeCode.Double
                    || originalCode == TypeCode.Single)
                {
                    return true;
                }
                return false;
            case TypeCode.Decimal:
                if (originalCode == TypeCode.Int16
                    || originalCode == TypeCode.Int32
                    || originalCode == TypeCode.Int64
                    || originalCode == TypeCode.Double
                    || originalCode == TypeCode.Single)
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
    private static object? DefaultForType(this Type targetType)
    {
        return targetType.IsValueType ? CreateInstance(targetType) : null;
    }

    /// <summary>
    /// Convert dictionary to matching parameters
    /// </summary>
    /// <param name="data">The dictionary. </param>
    /// <param name="pis">The parameter types.</param>
    /// <returns>object[]</returns>
    public static object?[] ChangeParameters(Dictionary<object, object> data, ParameterInfo[] pis)
    {
        var args = new object?[pis.Length];
        for (int i = 0; i < pis.Length; i++)
        {
            foreach (var kv in data)
            {
                if (pis[i].Name != null && pis[i].Name!.Equals(kv.Key.ToString()))
                {
                    args[i] = kv.Value;
                }
            }

            if (args[i] == null)
            {
                if (pis[i].DefaultValue != null && pis[i].DefaultValue?.GetType()?.Name != "DBNull")
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
    public static T? CreateInstance<T>(string typeName) where T : class
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
        return (T?)Activator.CreateInstance(type);
    }
    /// <summary>
    /// Creates an instance of the specified type. 
    /// </summary>
    /// <typeparam name="T">The result of type.</typeparam>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static T? CreateInstance<T>(this Type type)
    {
        return (T?)Activator.CreateInstance(type ?? typeof(T));
    }

    /// <summary>
    /// Creates an instance of the specified type. 
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static object? CreateInstance(this Type type)
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
    public static object? CallPropertyOrField(this object? container, string name, Type? type = null)
    {
        type = type ?? container?.GetType() ?? throw new ArgumentNullException(nameof(container));
#if NF20 || !USEEXPRESSION
        if (!char.IsDigit(name[0]))
        {
            var p = type.GetPropertyInfo(name);
            if (p != null)
            {
                return p.GetValue(container, null);
            }
        }
        var f = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
        if (f != null)
        {
            return f.GetValue(container);
        }
        return null;
#else
        var key = $"PI${Utility.ToHashCode(type)}.{name}";
        var method = (Delegate)dict.GetOrAdd(key, (cacheKey) =>
        {
            try
            {
                var parameter =
#if NF35
                Expression.Parameter(type,name);
#else
                Expression.Parameter(type);
#endif
                if (container != null)
                {
                    Expression body = Expression.PropertyOrField(parameter, name);
                    var expression = Expression.Lambda(body, parameter);
                    return expression.Compile();
                }
                else
                {
                    var p = type.GetPropertyInfo(name);
                    if (p != null)
                    {
                        var isStatic = p.GetGetMethod().IsStatic;
                        Expression body = Expression.MakeMemberAccess(isStatic ? null : parameter, p);
                        var expression = isStatic ? Expression.Lambda(body) : Expression.Lambda(body, parameter);
                        return expression.Compile();
                    }

                    var f = type.GetFieldInfo(name);
                    if (f != null)
                    {
                        var isStatic = f.IsStatic;
                        Expression body = Expression.MakeMemberAccess(isStatic ? null : parameter, f);
                        var expression = isStatic ? Expression.Lambda(body) : Expression.Lambda(body, parameter);
                        return expression.Compile();
                    }

                    return null;
                }
            }
            catch (Exception excetion)
            {
                System.Diagnostics.Debug.WriteLine(excetion);
                return null;
            }
        });
        if (container == null)
        {
            return method?.DynamicInvoke();
        }
        return method?.DynamicInvoke(container);
#endif
    }

    /// <summary>
    /// Calls the specified method and returns the result of execution
    /// </summary>
    /// <param name="container">Then Instance objects.</param>
    /// <param name="name">The name of the method.</param>
    /// <param name="args">The parameter of the method.</param>
    /// <returns>The result of execution.</returns>
    public static object? CallMethod(this object container, string name, object[] args)
    {
        return CallMethod(container?.GetType()!, container!, name, args);
    }

    /// <summary>
    /// Calls the specified method and returns the result of execution
    /// </summary>
    /// <param name="type">Then Instance objects.</param>
    /// <param name="container">Then Instance objects.</param>
    /// <param name="name">The name of the method.</param>
    /// <param name="args">The parameter of the method.</param>
    /// <returns>The result of execution.</returns>
    public static object? CallMethod(this Type type, object? container, string name, object?[]? args)
    {
        args ??= new Type[0];
        Type?[] types = [.. (args.Select(m => m?.GetType()) ?? [])];
        var method = GetMethodInfo(type, name, types);
        if (method != null)
        {
#if NF20 || !USEEXPRESSION
            bool hasNullValue = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    types[i] = args[i]!.GetType();
                }
                else
                {
                    hasNullValue = true;
                }
            }
            var pi = method.GetParameters();
            if (types.Length == 1 && types[0] == typeof(Dictionary<object, object>)
                && (pi.Length != 1 || !pi[0].ParameterType.IsSubclassOf(typeof(IDictionary))))
            {
                args = ChangeParameters((Dictionary<object, object>)args[0]!, pi);
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
                }
            }
#endif
            return Call(type, method, container, args);
        }
        return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <param name="pis"></param>
    /// <returns></returns>
    public static object?[] ChangeArguments(this object?[] args, ParameterInfo[] pis)
    {
        var newArgs = new object?[pis.Length];
        for (var i = 0; i < pis.Length; i++)
        {
            if (args.Length <= i || args[i] == null)
            {
                if (pis[i].IsOptional)
                    newArgs[i] = (pis[i].DefaultValue != null && pis[i].DefaultValue != DBNull.Value) ? pis[i].DefaultValue! : DefaultForType(pis[i].ParameterType)!;
                continue;
            }
            newArgs[i] = args[i];
            var type = args[i]!.GetType();
            if (type == pis[i].ParameterType)
                continue;
            if (pis[i].ParameterType.Name == "Nullable`1")
            {
                var genericType =
#if NF40 || NF35 || NF20
                        pis[i].ParameterType.GetGenericArguments()[0]
#else
                        pis[i].ParameterType.GenericTypeArguments[0]
#endif
                        ;

                var genericValue = args[i];
                if (genericType != type)
                {
                    genericValue = Convert.ChangeType(args[i], genericType);
                }
                if (genericValue == null)
                    continue;
                newArgs[i] = Activator.CreateInstance(pis[i].ParameterType, new object[] { genericValue })!;
                continue;
            }
            if (pis[i].ParameterType == typeof(string))
            {
                newArgs[i] = args[i]?.ToString()!;
                continue;
            }
            var changeObj = Convert.ChangeType(args[i], pis[i].ParameterType);
            if (changeObj != null)
            {
                newArgs[i] = changeObj;
                continue;
            }
            throw new TemplateException($"[FunctionTag]: parameter error. Expected \"{pis[i].ParameterType.Name}\" but got \"{type.Name}\"");
        }

        return newArgs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static object? ChangeType(this object value, Type target)
    {
        if (value == null)
            return null;
        var source = value.GetType();
        var method = GetExplicitOrImplicit(source, target);
        if (method != null)
            return method.Invoke(null, new object[] { value });
        switch (target.ToTypeCode())
        {
            case TypeCode.Decimal:
                return value.ToDecimal();
            case TypeCode.Double:
                return value.ToDouble();
            case TypeCode.Single:
                return value.ToFloat();
            case TypeCode.Int64:
                return value.ToLong();
            case TypeCode.UInt64:
                return value.ToULong();
            case TypeCode.Int32:
                return value.ToInt();
            case TypeCode.UInt32:
                return value.ToUInt();
            case TypeCode.Int16:
                return value.ToShort();
            case TypeCode.UInt16:
                return value.ToUShort();
            case TypeCode.Byte:
                return value.ToByte();
            case TypeCode.String:
                return value.ToString();
        }
        return Convert.ChangeType(value, target);
    }

    /// <summary>
    /// Get the index value.
    /// </summary>
    /// <param name="container">The object.</param>
    /// <param name="propIndex"> The zero-based index in the object. </param>
    /// <returns></returns>
    public static object? CallIndexValue(this object container, object propIndex)
    {
        IList? list;
        if (propIndex is int && (list = container as IList) != null)
        {
            return list[(int)propIndex];
        }
        IDictionary? dic;
        if ((dic = container as IDictionary) != null)
        {
            return dic[propIndex];
        }
        if (propIndex is int && container is string)
        {
            return ((string)container)[(int)propIndex];
        }
        Type t = container.GetType();
        var method = t.GetMethodInfo("get_Item", [propIndex.GetType()]);
        if (method != null)
        {
            return Call(t, method, container, new object[] { propIndex });
        }
        return null;
    }


    /// <summary>
    /// Searches for the enumerable with the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type? GetIEnumerableGenericType(this Type type)
    {
        var enumerable = typeof(IEnumerable<>);
        if (type.IsInterface
            && type.IsGenericType
            && type.GetGenericTypeDefinition() == enumerable)
        {
            return type;
        }
        foreach (var cType in type.GetInterfaces())
        {
            if (cType.IsGenericType && cType.GetGenericTypeDefinition() == enumerable)
            {
                return cType;
            }
        }
        return null;
    }

    /// <summary>
    ///  Gets a value indicating whether the <see cref="Type"/> is enumerable.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsIEnumerableGeneric(this Type type)
    {
        return GetIEnumerableGenericType(type) != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="genericType"></param>
    /// <param name="container"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? CallGenericMethod(this object container, string name, Type[] genericType, params object[] args)
    {
        var type = container.GetType();
        var genericMethod = type.GetMethod(name)?.MakeGenericMethod(genericType);
        return (genericMethod == null) ? null : Call(type, genericMethod, container, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="genericType"></param>
    /// <param name="container"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? CallGenericMethod(this Type type, object? container, string name, Type[] genericType, params object?[] args)
    {
        var genericMethod = type.GetMethod(name)?.MakeGenericMethod(genericType);
        return (genericMethod == null) ? null : Call(type, genericMethod, container, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="method"></param>
    /// <param name="container"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object? Call(this Type type, MethodInfo method, object? container, object?[]? args)
    {
        ParameterInfo[] pi = method.GetParameters();
        args = args?.ChangeArguments(pi);
#if NF20 || !USEEXPRESSION

        return method.Invoke(container, args);
#else

        var keys = pi.Select(m => Utility.ToHashCode(m.ParameterType)).ToArray();
        var name = $"D${Utility.ToHashCode(type)}.{method.Name}({string.Join(",", keys)})";

        var action = (Delegate)dict.GetOrAdd(name, (cacheKey) =>
        {
            try
            {
                var parameters = new List<ParameterExpression>();
                for (int i = 0; i < pi.Length; i++)
                {
                    parameters.Add(Expression.Parameter(pi[i].ParameterType, pi[i].Name));
                }

                ParameterExpression model = method.IsStatic ? null : Expression.Parameter(type, "__instance_");
                MethodCallExpression call = Expression.Call(model, method, parameters.ToArray());
                if (!method.IsStatic)
                {
                    parameters.Insert(0, model);
                }
                return Expression.Lambda(call, parameters.ToArray()).Compile();
            }
            catch (Exception excetion)
            {
                System.Diagnostics.Debug.WriteLine(excetion);
                return null;
            }
        });
        object[] values;
        if (method.IsStatic)
        {
            values = args;
        }
        else
        {
            values = new object[args.Length + 1];
            for (var i = 1; i < values.Length; i++)
            {
                values[i] = args[i - 1];
            }
            values[0] = container;
        }
        return action?.DynamicInvoke(values);
#endif
    }

    #region ToIEnumerable
    /// <summary>
    ///  Returns an enumerable that iterates through the object.
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns>A <see cref="IEnumerable"/> for the object.</returns>
    public static IEnumerable? ToIEnumerable(this object dataSource)
    {
        if (dataSource == null)
        {
#if NET20
            return null;
#else
            return Enumerable.Empty<object>();
#endif
        }
        if (dataSource is System.Data.DataTable dt)
        {
            return dt.Rows;
        }
        if (dataSource is IListSource source)
        {
            IList list = source.GetList();
            if (!source.ContainsListCollection)
            {
                return list;
            }
            if (list is ITypedList tl)
            {
                PropertyDescriptorCollection itemProperties = tl.GetItemProperties(new PropertyDescriptor[0]);
                if ((itemProperties == null) || (itemProperties.Count == 0))
                {
#if NET20
            return null;
#else
                    return Enumerable.Empty<object>();
#endif
                }
                PropertyDescriptor descriptor = itemProperties[0];
                if (descriptor != null)
                {
                    var value = descriptor.GetValue(list[0]);
                    if (value is IEnumerable toEnumerable)
                    {
                        return toEnumerable;
                    }
                }
#if NET20
            return null;
#else
                return Enumerable.Empty<object>();
#endif
            }
        }
        if (dataSource is IEnumerable enumerable)
        {
            return enumerable;
        }
#if NET20
            return null;
#else
        return Enumerable.Empty<object>();
#endif

    }
    #endregion
}
