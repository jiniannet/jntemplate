/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// IL帮助类
    /// </summary>
    public class ILHelpers
    {

        private delegate Object GetPropertyOrFieldDelegate(Object model, String propertyName);
        private delegate Object ExcuteMethodDelegate(Object container, Object[] args);
        private static Regex isNumberRegex = new Regex("[0-9]+", RegexOptions.Compiled);


        #region 获取属性或索引
        /// <summary>
        /// 获取属性或字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Object GetPropertyOrField(Object value, String propertyName)
        {
            if (value != null)
            {
                GetPropertyOrFieldDelegate d = CreateGetPropertyOrFieldProxy(value, propertyName);
                return d(value, propertyName);
            }
            return null;
        }
        private static GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Object value, String propertyName)
        {
            Type type = value.GetType(); ;
            GetPropertyOrFieldDelegate gpf = CreateGetPropertyOrFieldProxy(type, value, propertyName);
            return gpf;
        }

        private static GetPropertyOrFieldDelegate CreateGetPropertyOrFieldProxy(Type type, Object value, String propertyName)
        {
            Type objectType = typeof(Object);
            Type stringType= typeof(String);
            MethodInfo mi;
            FieldInfo fi;
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
        public static Object ExcuteMethod(Object container, String methodName, Object[] args)
        {
            if (container != null)
            {
                ExcuteMethodDelegate d = CreateExcuteMethodProxy(container, methodName, args);
                return d(container, args);
            }
            return null;
        }
        private static ExcuteMethodDelegate CreateExcuteMethodProxy(Object container, String methodName, Object[] args)
        {
            Type type = container.GetType();
            String key = String.Concat("Dynamic.IL.ExcuteMethod.", type.FullName);
            Object value;
            Dictionary<int, Dictionary<string,MemberInfo>> dic;
            Dictionary<string, MemberInfo> itemDic;
            if ((value = CacheHelprs.Get(key)) != null)
            {
                dic = (Dictionary<int, Dictionary<string, MemberInfo>>)value;
            }
            else
            {
                dic = new Dictionary<int, Dictionary<string, MemberInfo>>();
                CacheHelprs.Set(key, dic);
            }

            if(!dic.TryGetValue(args.Length,out itemDic))
            {
                dic[args.Length] = new Dictionary<string, MemberInfo>();
            }


            //ReflectionHelpers.GetMethod(type,methodName,ref )

            //String key1 = String.Concat(key,".", args.Length);


            //Object value;
            //if ((value = CacheHelprs.Get(key)) != null)
            //{
            //    return (CreateEntityDelegate<T>)value;
            //}
            //CreateEntityDelegate<T> ce = CreateEntityProxy<T>(type);
            //CacheHelprs.Set(key, ce);
            //return ce;

            //ExcuteMethodDelegate gpf = CreateExcuteMethodProxy(type, container, methodName, args);
            //return gpf;

            return null;
                ;
        }

        private static ExcuteMethodDelegate CreateExcuteMethodProxy(Type type, MethodInfo mi)
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
        #endregion

    }
}
