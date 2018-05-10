/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

//#define NEEDFIELD  //如果需要支持字段取值，请取消本行最开始的// 使用BUILD.bat执行生成时忽略本项
//#define USECACHE
using System;
using System.Reflection;
using System.Collections;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectionCallProxy : ICallProxy
    {
        private readonly char[] expressionPartSeparator;
        //private readonly char[] indexExprEndChars;
        //private readonly char[] indexExprStartChars;
        /// <summary>
        /// 反射构造函数
        /// </summary>
        public ReflectionCallProxy()
        {
            expressionPartSeparator = new char[] { '.' };
            //indexExprEndChars = new char[] { ']', ')' };
            //indexExprStartChars = new char[] { '[', '(' };
        }
        #region EVAL解析
        #region 4.0版本

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="propIndex">索引名称</param>
        /// <param name="isNumber">索引名称是否数字</param>
        /// <returns></returns>
        private object GetIndexedProperty(object container, bool isNumber, object propIndex)
        {
            IList list;
            if (isNumber && (list = container as IList) != null)
            {
                return list[(int)propIndex];
            }
            IDictionary dic;
            if ((dic = container as IDictionary) != null)
            {
                return dic[propIndex];
            }
            Type t = container.GetType();
#if NET20 || NET40
            PropertyInfo info = t.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { propIndex.GetType() }, null);
            if (info != null)
            {
                return info.GetValue(container, new object[] { propIndex });
            }
#elif NETSTANDARD
            var info = t.GetRuntimeMethod("get_Item", new Type[] { propIndex.GetType() });
            if (info != null)
            {
                return info.Invoke(container, new object[] { propIndex });
            }
#else
            var info = t.GetMethod("get_Item", new Type[] { propIndex.GetType() });
            if (info != null)
            {
                return info.Invoke(container, new object[] { propIndex });
            }
#endif
            return null;
        }

        #endregion

        #region Property
        /// <summary>
        /// 获取属性或字段的值
        /// </summary>
        /// <param name="container">原对象</param>
        /// <param name="propName">属性或字段名，有参数属性为参数值</param>
        /// <returns></returns>
        public object CallPropertyOrField(object container, string propName)
        {
            Type t = container.GetType();
            //此处的属性包括有参属性（索引）与无参属性（属性）
            //if (propName.IndexOfAny(indexExprStartChars) < 0)
            //因属性与字段均不可能以数字开头，如第一个字符为数字则直接跳过属性判断以加快处理速度
            if (!char.IsDigit(propName[0]))
            {
#if !NET20_NOTUSER
                PropertyInfo p =
#if NETSTANDARD
                    t.GetRuntimeProperty(propName);
#else
                    t.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
#endif
                //取属性
                if (p != null)
                {
                    return p.GetValue(container, null);
                }
#if NEEDFIELD
                //取字段
                FieldInfo f = t.GetField(propName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |_bindingIgnoreCase);
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

            return null;
        }
        #endregion

        #region Index Proerty
        /// <summary>
        /// 动态获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="propIndex">索引</param>
        /// <returns>返回结果</returns>
        public object CallIndexValue(object container, object propIndex)
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
#if NET20 || NET40
            PropertyInfo info = t.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { propIndex.GetType() }, null);
            if (info != null)
            {
                return info.GetValue(container, new object[] { propIndex });
            }
#elif NETSTANDARD
            var info = t.GetRuntimeMethod("get_Item", new Type[] { propIndex.GetType() });
            if (info != null)
            {
                return info.Invoke(container, new object[] { propIndex });
            }
#else
            var info = t.GetMethod("get_Item", new Type[] { propIndex.GetType() });
            if (info != null)
            {
                return info.Invoke(container, new object[] { propIndex });
            }
#endif
            return null;

            //int index;
            //if (int.TryParse(propName, out index))
            //{
            //    return GetIndexedProperty(container, true, index);
            //}
            ////取索引
            //return GetIndexedProperty(container, false, propName);
        }
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
        public string Eval(object container, string expression, string format)
        {
            object obj = Eval(container, expression);
            if ((obj == null)
#if NET20 || NET40
                || (obj == DBNull.Value)
#endif
                )
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(format))
            {
                return obj.ToString();
            }
            return string.Format(format, obj);
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public object Eval(object container, string expression)
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
            string[] expressionParts = expression.Split(expressionPartSeparator);
            return Eval(container, expressionParts);
        }
        #region
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expressionParts">表达式集合</param>
        /// <returns></returns>
        public object Eval(object container, string[] expressionParts)
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
        private object Eval(object container, string[] expressionParts, int start, int end)
        {
            object property = container;
            for (int i = start; (i < end) && (property != null); i++)
            {
                if (property == null)
                {
                    //throw new Exception("");
                    return null;
                }
                property = CallPropertyOrField(property, expressionParts[i]);
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
        public MethodInfo GetMethod(Type type, string methodName, ref Type[] args, out bool hasParam)
        {
            hasParam = false;

            MethodInfo method;
            //根据具体形参获取方法名，以处理重载
            if (args == null || Array.LastIndexOf(args, null) == -1)
            {
#if NET20 || NET40
                method = type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | Engine.Runtime.BindIgnoreCase,
                    null, args, null);
#else

                method = type.GetMethod(methodName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
#endif
                if (method != null)
                {
                    return method;
                }
            }


            //如果参数中存在空值，无法获取正常的参数类型，则进行智能判断

            ParameterInfo[] pi;
            bool accord;
            System.Collections.Generic.IEnumerable<MethodInfo> ms = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | Engine.Runtime.BindIgnoreCase);
            foreach (MethodInfo m in ms)
            {

                if (m.Name.Equals(methodName, Engine.Runtime.ComparisonIgnoreCase))
                {
                    pi = m.GetParameters();

                    if (pi.Length < pi.Length - 1)
                    {
                        continue;
                    }
#if NET20 || NET40
                    hasParam = System.Attribute.IsDefined(pi[pi.Length - 1], typeof(ParamArrayAttribute));
#endif
                    //参数个数一致或者形参中含有 param 参数
                    if (pi.Length == args.Length || hasParam)
                    {
                        accord = true;
                        for (int i = 0; i < pi.Length - 1; i++)
                        {
                            if (args[i] != null
                                && args[i] != pi[i].ParameterType
                                && !args[i].IsSubclassOf(pi[i].ParameterType))
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
                                    for (int j = pi.Length - 1; j < args.Length; j++)
                                    {
                                        if (args[j] != null && args[j] != arrType
                                            && !args[j].IsSubclassOf(arrType))
                                        {
                                            accord = false;
                                            break;
                                        }
                                    }
                                }

                                if (accord)
                                {
                                    args = new Type[pi.Length];
                                    for (int i = 0; i < pi.Length; i++)
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
        /// <returns>object</returns>
        public object CallMethod(object container, string methodName, object[] args)
        {
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    types[i] = args[i].GetType();
                }
            }

            Type t = container.GetType();

            bool hasParam;
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
                        for (int i = types.Length - 1; i < args.Length; i++)
                        {
                            arr.SetValue(args[i], i - (types.Length - 1));
                        }

                        object[] newArgs = new object[types.Length];

                        for (int i = 0; i < newArgs.Length - 1; i++)
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

    }
}