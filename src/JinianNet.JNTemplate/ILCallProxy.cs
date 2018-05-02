/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// IL操作类
    /// 注：本类并非最终版本，请勿使用本类
    /// </summary>
    public class ILCallProxy : ICallProxy
    {
        private Regex isNumberRegex;
        /// <summary>
        /// IL构造函数
        /// </summary>
        public ILCallProxy()
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
        public Object CallPropertyOrField(Object value, String propertyName)
        {
            if (value != null)
            {
                CallPropertyOrFieldDelegate d = CreateCallPropertyOrFieldProxy(value, propertyName);
                return d(value, propertyName);
            }
            return null;
        }

        private CallPropertyOrFieldDelegate CreateCallPropertyOrFieldProxy(Object value, String propertyName)
        {
            Type type = value.GetType();
            String key;
            if (isNumberRegex.Match(propertyName).Success)
            {
                key = String.Concat("Dynamic.IL.Property.", type.FullName, ".get_Item");
            }
            else
            {
                key = String.Concat("Dynamic.IL.Property.", type.FullName, ".", propertyName);
            }
            Object result;
            if ((result = Engine.Runtime.Cache.Get(key)) != null)
            {
                return (CallPropertyOrFieldDelegate)result;
            }
            CallPropertyOrFieldDelegate gpf = CreateCallPropertyOrFieldProxy(type, value, propertyName);
            Engine.Runtime.Cache.Set(key, gpf);
            return gpf;
        }
        private CallPropertyOrFieldDelegate CreateCallPropertyOrFieldProxy(Type type, Object value, String propertyName)
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
                String.Concat("P_", type.FullName.Replace(".", "_"), "_", propertyName),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(type);//0
            il.Emit(OpCodes.Ldarg_0);
            if (IsValueType(type))
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
            il.Emit(OpCodes.Stloc_0);

            if ((mi = GetMethod(type, String.Concat("get_", propertyName), Type.EmptyTypes)) != null)
            {
                Ldloc(type, il, 0);
                Call(type, il, mi);
                returnType = mi.ReturnType;
            }
            else if (isNumberRegex.Match(propertyName).Success && (mi = GetMethod(type, "get_Item",
               new Type[] {
                                typeof(int)
               })) != null)
            {
                Ldloc(type, il, 0);
                il.Emit(OpCodes.Ldind_I4, int.Parse(propertyName));
                Call(type, il, mi);
                returnType = mi.ReturnType;
            }
            else if ((mi = GetMethod(type, "get_Item",
               new Type[] {
                                stringType
               })) != null)
            {
                Ldloc(type, il, 0);
                il.Emit(OpCodes.Ldstr, propertyName);
                Call(type, il, mi);
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
                Ldloc(type, il, 0);
                il.Emit(OpCodes.Ldfld, fi);
                returnType = fi.FieldType;
            }
#endif
            else
            {
                il.Emit(OpCodes.Ldnull);
                returnType = objectType;
            }
            if (IsValueType(returnType))
            {
                il.Emit(OpCodes.Box, returnType);
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(CallPropertyOrFieldDelegate)) as CallPropertyOrFieldDelegate;
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
        public Object CallMethod(Object container, String methodName, Object[] args)
        {
            if (container != null)
            {
                Type[] types;
                ExcuteMethodDelegate d = CreateExcuteMethodProxy(container, methodName, args, out types);
                if (d != null)
                {
                    if (types != null && types.Length > 0)
                    {
                        for (Int32 i = 0; i < types.Length; i++)
                        {
                            if (types[i] != null && args[i] != null)
                            {
                                args[i] = Convert.ChangeType(args[i], types[i]);
                            }
                        }
                    }
                    return d(container, args);
                }
            }
            return null;
        }
        private ExcuteMethodDelegate CreateExcuteMethodProxy(Object container, String methodName, Object[] args, out Type[] parameterTypes)
        {
            parameterTypes = null;
            Type type = container.GetType();
            String key = String.Concat("Dynamic.IL.Method.", type.FullName, ".", methodName);
            Object value;
            String itemKey;
            ParameterInfo[] pis;
            Dictionary<Int32, Dictionary<String, DynamicMethodInfo>> dic;
            Dictionary<String, DynamicMethodInfo> itemDic = null;
            DynamicMethodInfo d;

            if ((value = Engine.Runtime.Cache.Get(key)) != null)
            {
                dic = (Dictionary<Int32, Dictionary<String, DynamicMethodInfo>>)value;
            }
            else
            {
                dic = new Dictionary<Int32, Dictionary<String, DynamicMethodInfo>>();
                MethodInfo[] mis = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                Type[] types;
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
                        itemDic = new Dictionary<string, DynamicMethodInfo>();
                        dic[types.Length] = itemDic;
                    }
                    //if (!itemDic.TryGetValue(itemKey,out d))
                    itemDic[itemKey] = CreateExcuteMethodProxy(type, mis[i]);
                }
                Engine.Runtime.Cache.Set(key, dic);
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
                    return d.Delegate;
                }
            }

            if (itemDic.Count > 0)
            {
                System.Collections.IEnumerator enumerator = itemDic.Values.GetEnumerator();
                enumerator.MoveNext();
                d = (DynamicMethodInfo)enumerator.Current;
                parameterTypes = new Type[args.Length];
                pis = d.Parameters;
                for (Int32 i = 0; i < args.Length; i++)
                {
                    if (pis[i].ParameterType != args[i].GetType())
                    {
                        parameterTypes[i] = pis[i].ParameterType;
                    }
                }
                return d.Delegate;
            }

            return null;
        }

        private DynamicMethodInfo CreateExcuteMethodProxy(Type type, MethodInfo mi)
        {
            Type objectType = typeof(Object);
            Type[] parameterTypes = {
                objectType,
                typeof(Object[])
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                String.Concat("M_", type.FullName.Replace(".", "_"), "_", mi.Name),
                objectType,
                parameterTypes);

            ILGenerator il = dynamicMethod.GetILGenerator();
            il.DeclareLocal(type);//0

            il.Emit(OpCodes.Ldarg_0);
            if (IsValueType(type))
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
            il.Emit(OpCodes.Stloc_0);

            ParameterInfo[] pis = mi.GetParameters();
            Int32 index = pis.Length;
            for (Int32 i = 0; i < index; i++)
            {
                il.DeclareLocal(pis[i].ParameterType);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldelem_Ref);
                if (IsValueType(pis[i].ParameterType))
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
            Ldloc(type, il, 0);
            for (Int32 i = 0; i < index; i++)
            {
                il.Emit(OpCodes.Ldloc, i + 1);
            }
            Call(type, il, mi);
            switch (mi.ReturnType.FullName)
            {
                case "System.Void":
                    il.Emit(OpCodes.Ldnull);
                    break;
                case "System.Object":
                    break;
                default:
                    if (IsValueType(mi.ReturnType))
                    {
                        il.Emit(OpCodes.Box, mi.ReturnType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, objectType);
                    }
                    break;
            }
            //il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);

            DynamicMethodInfo model = new DynamicMethodInfo();
            model.Delegate = dynamicMethod.CreateDelegate(typeof(ExcuteMethodDelegate)) as ExcuteMethodDelegate;
            model.FullName = String.Concat(type.FullName, "_", mi.Name);
            model.Name = mi.Name;
            model.Parameters = pis;
            return model;

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
            return String.Join("_", values);
        }
        /// <summary>
        /// 获取类型简写
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 加载局部变量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="il"></param>
        /// <param name="index"></param>
        private void Ldloc(Type type, ILGenerator il, Int32 index)
        {
            if (IsValueType(type))
            {
                il.Emit(OpCodes.Ldloca, index);
            }
            else
            {
                il.Emit(OpCodes.Ldloc, index);
            }
        }
        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="il"></param>
        /// <param name="index"></param>
        private void Ldarg(Type type, ILGenerator il, Int32 index)
        {
            if (IsValueType(type))
            {
                il.Emit(OpCodes.Ldarga, index);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, index);
            }
        }
        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="il"></param>
        /// <param name="mi"></param>
        private void Call(Type type, ILGenerator il, MethodInfo mi)
        {
            if (mi.IsStatic || (!mi.IsAbstract && !mi.IsVirtual) || IsValueType(type))
            {
                il.Emit(OpCodes.Call, mi);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, mi);
            }
        }

        private Boolean IsValueType(Type type)
        {
            return type.IsValueType;
        }


        private MethodInfo GetMethod(Type type, string methodName, Type[] argsType)
        {

#if NET20 || NET40
            return type.GetMethod(methodName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase,
                null,
                argsType,
                null);
#else
            return type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
#endif
        }

#endregion

    }

}