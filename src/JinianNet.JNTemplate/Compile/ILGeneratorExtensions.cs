/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JinianNet.JNTemplate.Compile
{
    /// <summary>
    /// 
    /// </summary>
    public static class ILGeneratorExtensions
    {
        /// <summary>
        /// object to type
        /// </summary>
        /// <param name="il">ILGenerator</param>
        /// <param name="type">目标类型</param>
        public static void ObjectTo(this ILGenerator il, Type type)
        {
            var toString = DynamicHelpers.GetMethod(typeof(object), "ToString", Type.EmptyTypes);
            switch (type.FullName)
            {
                case "System.Int32":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Int32).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Int64":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Int64).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Int16":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Int16).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Decimal":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Decimal).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Single":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Single).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Double":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Double).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.Boolean":
                    il.Emit(OpCodes.Callvirt, toString);
                    il.Emit(OpCodes.Call, typeof(Boolean).GetMethod("Parse", new[] { typeof(string) }));
                    break;
                case "System.String":
                    il.Emit(OpCodes.Callvirt, toString);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, type);
                    }
                    break;
            }
        }


        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="il"></param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        public static void Call(this ILGenerator il, Type type, MethodInfo method)
        {
            OpCode op;
            if (method.IsStatic || (method.DeclaringType.IsValueType))
            {
                op = OpCodes.Call;
            }
            else
            {
                if (method.IsVirtual && type.IsValueType)
                {
                    il.Emit(OpCodes.Constrained, type);
                }
                op = OpCodes.Callvirt;
            }
            il.Emit(op, method);
        }

        /// <summary>
        /// 加载数组
        /// </summary>
        /// <param name="il"></param>
        /// <param name="type"></param> 
        public static void Ldelem(this ILGenerator il, Type type)
        {
            switch (type.FullName)
            {
                case "System.Double":
                    il.Emit(OpCodes.Ldelem_R8);
                    break;
                case "System.Single":
                    il.Emit(OpCodes.Ldelem_R4);
                    break;
                case "System.Int64":
                    il.Emit(OpCodes.Ldelem_I8);
                    break;
                case "System.Int32":
                    il.Emit(OpCodes.Ldelem_I4);
                    break;
                case "System.UInt32":
                    il.Emit(OpCodes.Ldelem_U4);
                    break;
                case "System.Int16":
                    il.Emit(OpCodes.Ldelem_I2);
                    break;
                case "System.UInt16":
                case "System.Char":
                    il.Emit(OpCodes.Ldelem_U2);
                    break;
                case "System.Byte":
                    il.Emit(OpCodes.Ldelem_U1);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Ldelem, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldelem_Ref);
                    }
                    break;
            }
        }
    }
}
