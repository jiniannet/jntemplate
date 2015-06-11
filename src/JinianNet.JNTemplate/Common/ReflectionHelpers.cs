/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/
//#define NEEDFIELD  //如果需要支持字段取值，请取消本行最开始的// 使用BUILD.bat执行生成时忽略本项
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Collections;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectionHelpers
    {
        private static readonly Char[] expressionPartSeparator = new Char[] { '.' };
        private static readonly Char[] indexExprEndChars = new Char[] { ']', ')' };
        private static readonly Char[] indexExprStartChars = new Char[] { '[', '(' };

        #region EVAL解析
        #region 4.0版本

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="propIndex">索引名称</param>
        /// <param name="isNumber">索引名称是否数字</param>
        /// <returns></returns>
        private static Object GetIndexedProperty(Object container, Boolean isNumber, Object propIndex)
        {
            IList list;
            if (isNumber && (list = container as IList) != null)
            {
                return list[(Int32)propIndex];
            }
            IDictionary dic;
            if ((dic = container as IDictionary) != null)
            {
                return dic[propIndex];
            }
            PropertyInfo info = container.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { propIndex.GetType() }, null);
            if (info == null)
            {
                return null;
            }
            return info.GetValue(container, new Object[] { propIndex });
        }

        #endregion

        #region Property
        /// <summary>
        /// 获取属性或字段的值
        /// </summary>
        /// <param name="container">原对象</param>
        /// <param name="propName">属性或字段名，有参数属性为参数值</param>
        /// <returns></returns>
        public static Object GetPropertyOrField(Object container, String propName)
        {
            //此处的属性包括有参属性（索引）与无参属性（属性）
            if (propName.IndexOfAny(indexExprStartChars) < 0)
            {
                Type t = container.GetType();
                PropertyInfo p = t.GetProperty(propName);
                //取属性
                if (p != null)
                {
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //取字段
                FieldInfo f = t.GetField(propName);
                if (f != null)
                {
                    return f.GetValue(container);
                }
#endif
            }

            /*
if (propName.IndexOfAny(indexExprStartChars) < 0)
            {
#if NET20
                Type t = container.GetType();
                PropertyInfo p = t.GetProperty(propName);
                //取属性
                if (p != null)
                {
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //取字段
                FieldInfo f = t.GetField(propName);
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
             */

            Int32 index;
            if (Char.IsDigit(propName[0]) && Int32.TryParse(propName, out index))
            {
                return GetIndexedProperty(container, true, index);
            }
            //取索引
            return GetIndexedProperty(container, false, propName);
        }
        #endregion

        #region Index Proerty

        #endregion

        #endregion
        #region EVAL解析
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expression">表达式</param>
        /// <param name="format">格式化对象</param>
        /// <returns></returns>
        public static String Eval(Object container, String expression, String format)
        {
            Object obj = Eval(container, expression);
            if ((obj == null) || (obj == DBNull.Value))
            {
                return String.Empty;
            }
            if (String.IsNullOrEmpty(format))
            {
                return obj.ToString();
            }
            return String.Format(format, obj);
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static Object Eval(Object container, String expression)
        {
            if (expression == null)
            {
                //throw new ArgumentNullException("expression");
                return null;
            }
            expression = expression.Trim();
            if (expression.Length == 0)
            {
                //throw new ArgumentNullException("expression");
                return null;
            }
            if (container == null)
            {
                return null;
            }
            String[] expressionParts = expression.Split(expressionPartSeparator);
            return Eval(container, expressionParts);
        }
        #region
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expressionParts">表达式集合</param>
        /// <returns></returns>
        public static Object Eval(Object container, String[] expressionParts)
        {
            return Eval(container, expressionParts, 0, expressionParts.Length);
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对像</param>
        /// <param name="expressionParts">表达式</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        private static Object Eval(Object container, String[] expressionParts, Int32 start, Int32 end)
        {
            Object property = container;
            for (Int32 i = start; (i < end) && (property != null); i++)
            {
                if (property == null)
                {
                    //throw new Exception("");
                    return null;
                }
                property = GetPropertyOrField(property, expressionParts[i]);
            }
            return property;
        }
        #endregion

        #endregion
        #region Method
        /// <summary>
        /// 根据形参与方法名获取MethodInfo
        /// </summary>
        /// <param name="type">目标TYPE</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">形参</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetMethod(Type type, String methodName, Type[] args)
        {
            if (args == null || Array.LastIndexOf(args, null) == -1) //
            {
                return type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static,
                    null, args, null);
            }
            else
            {
                ParameterInfo[] pi;
                Boolean accord;
                foreach (MethodInfo m in type.GetMembers(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                    {
                        pi = m.GetParameters();
                        if (pi.Length == args.Length)
                        {
                            accord = true;
                            for (Int32 i = 0; i < pi.Length; i++)
                            {
                                if (args[i] != null && args[i] != pi[i].ParameterType)
                                {
                                    accord = false;
                                    break;
                                }
                            }
                            if (accord)
                            {
                                return m;
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 调用实例方法
        /// </summary>
        /// <param name="container">实例对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">形参</param>
        /// <returns>Object</returns>
        public static Object InvokeMethod(Object container, String methodName, Object[] args)
        {
            Type[] types = new Type[args.Length];
            for (Int32 i = 0; i < args.Length; i++)
                types[i] = args[i].GetType();
            Type t = container.GetType();
            MethodInfo method = GetMethod(t, methodName, types);
            if (method == null)
                return null; //throw new Exception(String.Concat("在类型 ", t.Name, " 中未找到方法 ", methodName));

            return method.Invoke(container, args);
        }
        #endregion
        #region ToIEnumerable
        /// <summary>
        /// 将对象转换为IEnumerable
        /// </summary>
        /// <param name="dataSource">源对象</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable ToIEnumerable(Object dataSource)
        {
            IListSource source;
            IEnumerable result;

            if (dataSource == null)
                return null;
            source = dataSource as IListSource;
            if ((source = dataSource as IListSource) != null)
            {
                IList list = source.GetList();
                if (!source.ContainsListCollection)
                {
                    return list;
                }
                if ((list != null) && (list is ITypedList))
                {
                    PropertyDescriptorCollection itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                    if ((itemProperties == null) || (itemProperties.Count == 0))
                    {
                        return null;
                    }
                    PropertyDescriptor descriptor = itemProperties[0];
                    if (descriptor != null)
                    {
                        Object component = list[0];
                        Object value = descriptor.GetValue(component);
                        if ((value != null) && ((result = value as IEnumerable) != null))
                        {
                            return result;
                        }
                    }
                    return null;
                }
            }
            if ((result = dataSource as IEnumerable) != null)
            {
                return result;
            }
            return null;

        }


        #endregion
    }
}