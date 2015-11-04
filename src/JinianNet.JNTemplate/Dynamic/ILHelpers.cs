/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using JinianNet.JNTemplate.Common;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// IL帮助类
    /// </summary>
    public class ILHelpers : IDynamicHelpers
    {

        private delegate Object GetPropertyOrFieldDelegate(Object model, String propertyName);
        private delegate Object ExcuteMethodDelegate(Object container, Object[] args);
        private Regex isNumberRegex;

        /// <summary>
        /// IL构造函数
        /// </summary>
        public ILHelpers()
        {
            isNumberRegex = new Regex("[0-9]+", RegexOptions.Compiled);
        }

        #region 获取属性或索引
        /// <summary>
        /// 获取属性或字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public Object GetPropertyOrField(Object value, String propertyName)
        {
            if (value != null)
            {
                GetPropertyOrFieldDelegate d = CreateGetPropertyOrFieldProxy(value, propertyName);
                return d(value, propertyName);
            }
            return null;
        }

        private GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Object value, String propertyName)
        {
            Type type = value.GetType();
            String key;
            if (isNumberRegex.Match(propertyName).Success)
            {
                key = String.Concat("Dynamic.IL.GetPropertyOrField.", type.FullName, ".get_Item");
            }
            else
            {
                key = String.Concat("Dynamic.IL.GetPropertyOrField.", type.FullName, ".", propertyName);
            }
            Object result;
            if ((result = CacheHelprs.Get(key)) != null)
            {
                return (GetPropertyOrFieldDelegate)result;
            }
            GetPropertyOrFieldDelegate gpf = CreateGetPropertyOrFieldProxy(type, value, propertyName);
            CacheHelprs.Set(key, gpf);
            return gpf;
        }
        private GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Type type, Object value, String propertyName)
        {
            Type objectType = typeof(Object);
            Type stringType = typeof(String);
            MethodInfo mi;
#if NEEDFIELD
            FieldInfo fi;
#endif
            Type returnType;
            Type[] parameterTypes = {
                objectType,
                stringType
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("DynamicMethod.PropertyOrField.", type.FullName, ".", propertyName),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(objectType);//0
            il.DeclareLocal(type);//1
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            if ((mi = type.GetMethod(String.Concat("get_", propertyName), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
                Type.EmptyTypes,
                null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
            else if (isNumberRegex.Match(propertyName).Success && (mi = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
               new Type[] {
                    typeof(int)
               },
               null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldind_I4, int.Parse(propertyName));
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
            else if ((mi = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase, null,
               new Type[] {
                                stringType
               },
               null)) != null)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldstr, propertyName);
                il.Emit(OpCodes.Callvirt, mi);
                returnType = mi.ReturnType;
            }
#if NEEDFIELD
            else if ((fi = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                //Type t;
                //if ((fi.FieldType.IsArray && (fi.FieldType.GetArrayRank() > 1 || (!(t = fi.FieldType.GetElementType()).IsValueType && t != typeof(String) && t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
                //                          (!fi.FieldType.IsArray && fi.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                //    return null
                //        ;
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldfld, fi);
                returnType = fi.FieldType;

            }
#endif
            else
            {
                il.Emit(OpCodes.Ldnull);
                returnType = objectType;
            }
            if (returnType.IsValueType)
            {
                il.Emit(OpCodes.Box, returnType);
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(GetPropertyOrFieldDelegate)) as GetPropertyOrFieldDelegate;
        }
        #endregion


        #region 执行方法
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns></returns>
        public Object ExcuteMethod(Object container, String methodName, Object[] args)
        {
            if (container != null)
            {
                ExcuteMethodDelegate d = CreateExcuteMethodProxy(container, methodName, args);
                if (d != null)
                {
                    return d(container, args);
                }
            }
            return null;
        }
        private ExcuteMethodDelegate CreateExcuteMethodProxy(Object container, String methodName, Object[] args)
        {
            Type type = container.GetType();
            String key = String.Concat("Dynamic.IL.ExcuteMethod.", type.FullName);
            Object value;
            String itemKey;
            Dictionary<Int32, Dictionary<String, ExcuteMethodDelegate>> dic;
            Dictionary<String, ExcuteMethodDelegate> itemDic = null;
            ExcuteMethodDelegate d;

            if ((value = CacheHelprs.Get(key)) != null)
            {
                dic = (Dictionary<Int32, Dictionary<String, ExcuteMethodDelegate>>)value;
            }
            else
            {
                dic = new Dictionary<Int32, Dictionary<String, ExcuteMethodDelegate>>();
                MethodInfo[] mis = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                Type[] types;
                ParameterInfo[] pis;

                for (Int32 i = 0; i < mis.Length; i++)
                {
                    if (!mis[i].Name.Equals(methodName, Engine.Runtime.ComparisonIgnoreCase))
                    {
                        continue;
                    }
                    pis = mis[i].GetParameters();
                    types = new Type[pis.Length];
                    for (Int32 j = 0; j < pis.Length; j++)
                    {
                        types[j] = pis[j].ParameterType;
                    }
                    itemKey = GetArgsTypeKey(types);
                    if (!dic.TryGetValue(types.Length, out itemDic))
                    {
                        itemDic = new Dictionary<string, ExcuteMethodDelegate>();
                        dic[types.Length] = itemDic;
                    }
                    //if (!itemDic.TryGetValue(itemKey,out d))
                    itemDic[itemKey] = CreateExcuteMethodProxy(type, mis[i]);
                }
                CacheHelprs.Set(key, dic);
            }

            if (!dic.TryGetValue(args.Length, out itemDic))
            {
                return null;
            }

            itemKey = GetArgsTypeKey(args);

            if (itemKey != null)
            {
                if (itemDic.TryGetValue(itemKey, out d))
                {
                    return d;
                }
            }
            else
            {
                if (itemDic.Count > 0)
                {
                    System.Collections.IEnumerator enumerator = itemDic.Values.GetEnumerator();
                    enumerator.MoveNext();
                    return (ExcuteMethodDelegate)enumerator.Current;
                }
            }
            return null;
        }

        private ExcuteMethodDelegate CreateExcuteMethodProxy(Type type, MethodInfo mi)
        {
            Type objectType = typeof(Object);
            Type[] parameterTypes = {
                objectType,
                typeof(Object[])
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("DynamicMethod.ExcuteMethod.", type.FullName, ".", mi.Name),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(type);//0

            ParameterInfo[] pis = mi.GetParameters();
            Int32 index = pis.Length;


            for (Int32 i = 0; i < index; i++)
            {
                il.DeclareLocal(pis[i].ParameterType);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldelem_Ref);
                if (pis[i].ParameterType.IsValueType)
                {
                    //此处不需要考虑TryParse,因为实参类型必须与形参一致
                    il.Emit(OpCodes.Unbox_Any, pis[i].ParameterType);
                }
                else
                {
                    if (pis[i].ParameterType.FullName != "System.Object")
                    {
                        il.Emit(OpCodes.Castclass, pis[i].ParameterType);
                    }
                }
                il.Emit(OpCodes.Stloc, i + 1);
            }
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            for (Int32 i = 0; i < index; i++)
            {
                il.Emit(OpCodes.Ldloc, i + 1);
            }
            il.Emit(OpCodes.Callvirt, mi);
            switch (mi.ReturnType.FullName)
            {
                case "System.Void":
                    il.Emit(OpCodes.Ldnull);
                    break;
                case "System.Object":
                    break;
                default:
                    if (mi.ReturnType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, mi.ReturnType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, objectType);
                    }
                    break;
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(ExcuteMethodDelegate)) as ExcuteMethodDelegate;

        }
        #endregion

        #region 共用方法
        private Boolean HasNull(Object[] args)
        {
            if (args != null)
            {
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (args[i] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private String GetArgsTypeKey(Object[] args)
        {
            if (args != null)
            {
                if (args.Length == 0)
                {
                    return "N";
                }
                String[] values = new String[args.Length];
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (args[i] == null)
                    {
                        return null;
                    }
                    values[i] = GetTypeKeyName(args[i].GetType().FullName);
                }
                return String.Join(".", values);
            }
            return null;
        }

        private String GetArgsTypeKey(Type[] types)
        {
            if (types.Length == 0)
            {
                return "N";
            }
            String[] values = new String[types.Length];
            for (Int32 i = 0; i < types.Length; i++)
            {
                values[i] = GetTypeKeyName(types[i].FullName);
            }
            return String.Join(".", values);
        }

        private String GetTypeKeyName(string fullName)
        {
            switch (fullName)
            {
                case "System.String":
                    return "S";
                case "System.Int16":
                    return "I";
                case "System.Int32":
                    return "I4";
                case "System.Int64":
                    return "I8";
                case "System.Single":
                    return "F";
                case "System.Double":
                    return "D";
                default:
                    return fullName;
            }
        }

        #endregion

    }
}
