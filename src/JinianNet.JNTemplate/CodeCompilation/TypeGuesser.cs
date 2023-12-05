/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Exceptions;
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Type Guess
    /// </summary>
    public class TypeGuesser
    { 
        /// <summary>
        /// Gets the child <see cref="Type"/> with the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Thpe <see cref="Type"/>.</param>
        /// <returns></returns>
        public static Type[] InferChildType(Type type)
        {
            if (type.IsArray)
            {
                var methd = type.GetMethodInfo("Get", new Type[] { typeof(int) });
                if (methd != null)
                {
                    return new Type[] { methd.ReturnType };
                }
            }
            var gType = type.GetIEnumerableGenericType();
            if (gType != null)
            {
                return gType.GetGenericArguments();
            }

            if (type == typeof(System.Data.DataTable))
            {
                return new Type[] { typeof(System.Data.DataRow) };
            }

            var ms = type.GetMethods();
            var objType = typeof(object);
            foreach (var m in ms)
            {
                if (m.Name.Equals("get_Item", StringComparison.CurrentCultureIgnoreCase) &&
                    m.ReturnType != objType)
                    return new Type[] { m.ReturnType };
            }
            return new Type[] { typeof(object) };
        }

        /// <summary>
        /// Get priority
        /// </summary>
        /// <param name="type">TYPE</param>
        /// <returns></returns>
        private static int GetTypeLevel(Type type)
        {
            switch (type.FullName)
            {
                case "System.Object":
                    return 13;
                case "System.String":
                    return 12;
                case "System.Boolean":
                    return 11;
                case "System.Decimal":
                    return 10;
                case "System.Double":
                    return 9;
                case "System.Single":
                    return 8;
                case "System.Int64":
                    return 7;
                case "System.UInt64":
                    return 6;
                case "System.Int32":
                    return 5;
                case "System.UInt32":
                    return 4;
                case "System.Int16":
                    return 3;
                case "System.UInt16":
                    return 2;
                case "System.SByte":
                    return 1;
                case "System.Byte":
                    return 0;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Whether the type is unsigned
        /// </summary>
        /// <param name="type">TYPE</param>
        /// <returns></returns>
        private static bool IsMinZero(Type type)
        {
            switch (type.FullName)
            {
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Byte":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The unsigned type is converted to the corresponding signed type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type ChangeType(Type type)
        {
            switch (type.FullName)
            {
                case "System.UInt16":
                    return typeof(Int16);
                case "System.UInt32":
                    return typeof(Int32);
                case "System.UInt64":
                    return typeof(Int64);
                case "System.SByte":
                    return typeof(Byte);
                default:
                    return type;
            }
        }

        /// <summary>
        /// Choose the best type
        /// </summary>
        /// <param name="x">The first type.</param>
        /// <param name="y">The second type.</param>
        /// <returns></returns>
        public static Type ChoiceType(Type x, Type y)
        {
            if ((x.FullName == "System.Single" && (y.FullName == "System.Int64" || y.FullName == "System.UInt64"))
                || (y.FullName == "System.Single" && (x.FullName == "System.Int64" || x.FullName == "System.UInt64")))
            {
                return typeof(double);
            }
            int x1 = GetTypeLevel(x);
            int y1 = GetTypeLevel(y);
            if (IsNumber(x) && IsNumber(y))
            {
                if (x1 > y1)
                {
                    if (IsMinZero(x) && !IsMinZero(x))
                    {
                        return ChangeType(x);
                    }
                    return x;
                }
                if (x1 < y1)
                {
                    if (!IsMinZero(x) && IsMinZero(x))
                    {
                        return ChangeType(y);
                    }
                    return y;
                }
            }
            else
            {
                if (x1 < y1)
                {
                    return y;
                }
            }
            return x;
        }

        /// <summary>
        /// Find the type that matches best.
        /// </summary>
        /// <param name="types">type array</param>
        /// <returns></returns>
        public static Type FindBestType(Type[] types)
        {
            Type type = types[0];
            for (var i = 1; i < types.Length; i++)
            {
                if (types[i] == null)
                {
                    continue;
                }
                type = ChoiceType(type, types[i]);
            }
            return type;
        }

        /// <summary>
        /// Gets a value indicating whether the  <see cref="Type"/> is one of the primitive types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the <see cref="Type"/> is one of the primitive types; otherwise, false.</returns>
        public static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanUseEqual(Type type)
        {
            if (type.FullName == "System.Decimal")
                return false;
            return IsNumber(type)
                 || type.FullName == "System.Boolean"
                 || type.IsEnum;
        }
        /// <summary>
        /// Indicates whether the specified type is categorized as a number.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the <see cref="Type"/> is one of the number types; otherwise, false.</returns>
        public static bool IsNumber(Type type)
        {
            var name = type.FullName;
            if (name == "System.Decimal")
            {
                return true;
            }
            if (!IsPrimitive(type))
            {
                return false;
            }
            switch (name)
            {
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                case "System.Int64":
                case "System.UInt64":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int16":
                case "System.UInt16":
                case "System.SByte":
                case "System.Byte":
                    return true;
            }
            return false;
        }
         
    }
}
