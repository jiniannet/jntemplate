/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// IL操作类
    /// 注：本类并非最终版本，请勿使用本类
    /// </summary>
    public class ILActuator : IActuator
    {

        private ICache cache;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public ILActuator(ICache provider)
        {
            cache = provider;
        }

        #region 获取属性或索引
        /// <summary>
        /// 获取属性或字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object CallPropertyOrField(object value, string propertyName)
        {
            if (value != null)
            {
                CallPropertyOrFieldDelegate d = CreateCallPropertyOrFieldProxy(value, propertyName);
                return d(value, propertyName);
            }
            return null;
        }

        private CallPropertyOrFieldDelegate CreateCallPropertyOrFieldProxy(object value, string propertyName)
        {
            Type type = value.GetType();
            string key = string.Concat("p.", TypeToName(type), ".", propertyName);
            CallPropertyOrFieldDelegate gpf;
            if ((gpf = cache.Get<CallPropertyOrFieldDelegate>(key)) != null)
            {
                return gpf;
            }
            gpf = CreateCallPropertyOrFieldProxy(type, value, propertyName);
            cache.Set(key, gpf);
            return gpf;
        }

        private string TypeToName(Type type)
        {
            string name = type.IsGenericType ? type.Name : type.FullName;
            return name.Replace(".", "_").Replace(" ", "").Replace("<", "_").Replace(">", "_").Replace("`", "_");
            //<> f__AnonymousType0`1[[System.String, mscorlib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089]]
        }
        private CallPropertyOrFieldDelegate CreateCallPropertyOrFieldProxy(Type type, object value, string propertyName)
        {
            Type objectType = typeof(object);
            Type stringType = typeof(string);
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
                string.Concat("P_", TypeToName(type), "_", propertyName),
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

            if ((mi = GetMethod(type, string.Concat("get_", propertyName), Type.EmptyTypes)) != null)
            {
                Ldloc(type, il, 0);
                Call(type, il, mi);
                returnType = mi.ReturnType;
            }
#if NEEDFIELD
            else if ((fi = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                //Type t;
                //if ((fi.FieldType.IsArray && (fi.FieldType.GetArrayRank() > 1 || (!(t = fi.FieldType.GetElementType()).IsValueType && t != typeof(string) && t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
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

        private CallIndexValueDelegate CreateCallIndexValueProxy(Type type, object value, object index)
        {
            Type objectType = typeof(object);
            Type stringType = typeof(string);
            MethodInfo mi;
            Type returnType;
            Type[] parameterTypes = {
                objectType,
                objectType
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                string.Concat("i_", TypeToName(type), "_", index.GetType().Name),
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

            var indexType = index.GetType();

            if ((mi = GetMethod(type, "get_Item",
               new Type[] {
                   indexType
               })) != null)
            {
                Ldloc(type, il, 0);
                il.Emit(OpCodes.Ldarg_1);
                if (IsValueType(indexType))
                {
                    il.Emit(OpCodes.Unbox_Any, indexType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, indexType);
                }
                Call(type, il, mi);
                returnType = mi.ReturnType;
            }
            if ((mi = GetMethod(type, "get",
              new Type[] {
                   indexType
              })) != null)
            {
                Ldloc(type, il, 0);
                il.Emit(OpCodes.Ldarg_1);
                if (IsValueType(indexType))
                {
                    il.Emit(OpCodes.Unbox_Any, indexType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, indexType);
                }
                Call(type, il, mi);
                returnType = mi.ReturnType;
            }
            else
            {
                return null;
            }

            if (IsValueType(returnType))
            {
                il.Emit(OpCodes.Box, returnType);
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod.CreateDelegate(typeof(CallIndexValueDelegate)) as CallIndexValueDelegate;

        }
        #endregion

        #region 获取索引
        /// <summary>
        /// 动态获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="index">索引</param>
        /// <returns>返回结果</returns>
        public object CallIndexValue(object container, object index)
        {
            if (container != null)
            {
                #region 常见类型做特殊处理
                IList list;
                if (index is int && (list = container as IList) != null)
                {
                    return list[(int)index];
                }
                IDictionary dic;
                if ((dic = container as IDictionary) != null)
                {
                    return dic[index];
                }
                if (index is int && container is string)
                {
                    return ((string)container)[(int)index];
                }
                #endregion

                CallIndexValueDelegate d;
                Type type = container.GetType();
                string key = string.Concat("i.", TypeToName(type), ".", index.GetType().Name);
                if ((d = cache.Get<CallIndexValueDelegate>(key)) == null)
                {
                    d = CreateCallIndexValueProxy(type, container, index);
                    cache.Set(key, d);
                }
                return d(container, index);
            }
            return null;
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
        public object CallMethod(object container, string methodName, object[] args)
        {
            if (container != null)
            {
                var m = GetExcuteMethodProxy(container, methodName, args);
                if (m != null)
                {
                    if (!m.IsMatchParameters)
                    {
                        args = DynamicHelpers.ChangeParameters((Dictionary<object, object>)args[0], m.Parameters);
                    }
                    for (int i = 0; i < m.Parameters.Length; i++)
                    {
                        if (m.Parameters[i] != null && args[i] != null && args[i].GetType() != m.Parameters[i].ParameterType)
                        {
                            args[i] = Convert.ChangeType(args[i], m.Parameters[i].ParameterType);
                        }
                    }
                    return m.Delegate(container, args);
                }
            }
            return null;
        }
        private DynamicMethodInfo[] CreateDynamicMethods(Type type, string methodName)
        {
            MethodInfo[] mis = DynamicHelpers.GetMethods(type, methodName);
            DynamicMethodInfo[] list = new DynamicMethodInfo[mis.Length];
            Type[] types;
            for (int i = 0; i < mis.Length; i++)
            {
                ParameterInfo[] pis = mis[i].GetParameters();
                types = new Type[pis.Length];
                for (int j = 0; j < pis.Length; j++)
                {
                    types[j] = pis[j].ParameterType;
                }
                list[i] = CreateExcuteMethodProxy(type, mis[i]);
            }
            return list;
        }

        private DynamicMethodInfo GetExcuteMethodProxy(object container, string methodName, object[] args)
        {
            Type type = container.GetType();
            string key = string.Concat("m.", TypeToName(type), ".", methodName);

            DynamicMethodInfo[] list;

            if ((list = cache.Get<DynamicMethodInfo[]>(key)) == null)
            {
                list = CreateDynamicMethods(type, methodName);
                cache.Set(key, list);
            }
            if (list != null && list.Length > 0)
            {
                var argsType = new Type[args.Length];
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] != null)
                    {
                        argsType[i] = args[i].GetType();
                    }
                }
                var m = DynamicHelpers.GetDynamicMethod(methodName, list, argsType);
                if (m != null)
                {
                    m.IsMatchParameters = true;
                    return m;
                }


                //可选参数
                if (argsType.Length == 1 && argsType[0] != null && argsType[0] == typeof(Dictionary<object, object>)
         && (list[0].Parameters.Length != 1 || !list[0].Parameters[0].ParameterType.IsSubclassOf(typeof(IDictionary))))
                {
                    m = list[0];
                    m.IsMatchParameters = false;
                    return m;
                }

            }

            return null;
        }

        private DynamicMethodInfo CreateExcuteMethodProxy(Type type, MethodInfo mi)
        {
            Type objectType = typeof(object);
            Type[] parameterTypes = {
                objectType,
                typeof(object[])
            };
            DynamicMethod dynamicMethod = new DynamicMethod(
                string.Concat("M_", TypeToName(type), "_", mi.Name),
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
            int index = pis.Length;
            for (int i = 0; i < index; i++)
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
            for (int i = 0; i < index; i++)
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
            model.FullName = string.Concat(TypeToName(type), "_", mi.Name);
            model.Name = mi.Name;
            model.Parameters = pis;
            return model;

        }

        #endregion

        #region 共用方法 
        /// <summary>
        /// 加载局部变量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="il"></param>
        /// <param name="index"></param>
        private void Ldloc(Type type, ILGenerator il, int index)
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
        private void Ldarg(Type type, ILGenerator il, int index)
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

        private bool IsValueType(Type type)
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