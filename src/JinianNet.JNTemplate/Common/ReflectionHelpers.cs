/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
//#define NEEDFIELD  //如果需要支持字段取值，请取消本行最开始的// 使用BUILD.bat执行生成时忽略本项
//#define USECACHE
using System;
using System.ComponentModel;
using System.Reflection;
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
            Type t = container.GetType();
            //string cacheKey = String.Concat(t.FullName, "[", propIndex.ToString(), "]");
            PropertyInfo info = null;// Utils.GetCacheItem<PropertyInfo>(cacheKey);
            if (info == null)
            {
                info = t.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { propIndex.GetType() }, null);
                if (info == null)
                {
                    return null;
                }
#if USECACHE
                Utils.InsertCache(cacheKey, info);
#endif
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
            Type t = container.GetType();
#if USECACHE
            //是否有缓存
            String cacheKey = String.Concat(t.FullName, ".", propName);
            MemberInfo mi = Utils.GetCacheItem<MemberInfo>(cacheKey);
            if (mi != null)
            {
                switch (mi.MemberType.ToString())
                {
                    case "Property":
                        return ((PropertyInfo)mi).GetValue(container, null);
                    case "Field":
                        return ((FieldInfo)mi).GetValue(container);
                }

            }

            if ((mi = Utils.GetCacheItem<MemberInfo>(String.Concat(t.FullName, "[", propName, "]"))) != null)
            {
                return ((PropertyInfo)mi).GetValue(container, null);
            }
#endif
            //此处的属性包括有参属性（索引）与无参属性（属性）
            if (propName.IndexOfAny(indexExprStartChars) < 0)
            {
                PropertyInfo p = t.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
                //取属性
                if (p != null)
                {
#if USECACHE
                    Utils.InsertCache(cacheKey, p);
#endif
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //取字段
                FieldInfo f = t.GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.BindIgnoreCase);
                if (f != null)
                {
#if USECACHE
                    Utils.InsertCache(cacheKey, p);
#endif
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
        /// <param name="hasParam">是否有params参数</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetMethod(Type type, String methodName, ref Type[] args, out Boolean hasParam)
        {
            hasParam = false;

            MethodInfo method;
            //根据具体形参获取方法名，以处理重载
            if (args == null || Array.LastIndexOf(args, null) == -1)
            {
                method = type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | Engine.Runtime.BindIgnoreCase,
                    null, args, null);
                if (method != null)
                {
                    return method;
                }
            }

            //如果参数中存在空值，无法获取正常的参数类型，则进行智能判断

            ParameterInfo[] pi;
            Boolean accord;

            MethodInfo[] ms = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);

            foreach (MethodInfo m in ms)
            {

                if (m.Name.Equals(methodName, Engine.Runtime.ComparisonIgnoreCase))
                {
                    pi = m.GetParameters();

                    if (pi.Length < pi.Length - 1)
                    {
                        continue;
                    }

                    hasParam = System.Attribute.IsDefined(pi[pi.Length - 1], typeof(ParamArrayAttribute));

                    //参数个数一致或者形参中含有 param 参数
                    if (pi.Length == args.Length || hasParam)
                    {
                        accord = true;
                        for (Int32 i = 0; i < pi.Length - 1; i++)
                        {
                            if (args[i] != null && args[i] != pi[i].ParameterType && !args[i].IsSubclassOf(pi[i].ParameterType))
                            {
                                accord = false;
                                break;
                            }
                        }
                        if (accord)
                        {
                            if (hasParam)
                            {
                                if (args.Length != pi.Length - 1)
                                {
                                    Type arrType = pi[pi.Length - 1].ParameterType.GetElementType();
                                    for (Int32 j = pi.Length - 1; j < args.Length; j++)
                                    {
                                        if (args[j] != null && args[j] != arrType && !args[j].IsSubclassOf(arrType))
                                        {
                                            accord = false;
                                            break;
                                        }
                                    }
                                }

                                if (accord)
                                {
                                    args = new Type[pi.Length];
                                    for (Int32 i = 0; i < pi.Length; i++)
                                    {
                                        args[i] = pi[i].ParameterType;

                                    }
                                    return m;
                                }
                            }
                            else
                            {
                                if (args[args.Length - 1] == pi[pi.Length - 1].ParameterType)
                                {
                                    return m;
                                }
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
            {
                if (args[i] != null)
                {
                    types[i] = args[i].GetType();
                }
            }

            Type t = container.GetType();

            Boolean hasParam;
            MethodInfo method = GetMethod(t, methodName, ref types, out hasParam);
            if (method != null)
            {
                if (hasParam)
                {
                    Array arr;
                    if (types.Length - 1 == args.Length)
                    {
                        arr = null;
                    }
                    else
                    {
                        arr = Array.CreateInstance(types[types.Length - 1].GetElementType(), args.Length - types.Length + 1);
                        for (Int32 i = types.Length - 1; i < args.Length; i++)
                        {
                            arr.SetValue(args[i], i - (types.Length - 1));
                        }

                        Object[] newArgs = new Object[types.Length];

                        for (Int32 i = 0; i < newArgs.Length - 1; i++)
                        {
                            newArgs[i] = args[i];
                        }
                        newArgs[newArgs.Length - 1] = arr;

                        return method.Invoke(container, newArgs);
                    }
                }
                else
                {
                    return method.Invoke(container, args);
                }
            }

            return null;
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