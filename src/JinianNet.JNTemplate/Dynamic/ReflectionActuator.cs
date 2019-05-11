/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

//#define NEEDFIELD  //如果需要支持字段取值，请取消本行最开始的// 使用BUILD.bat执行生成时忽略本项
//#define USECACHE
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectionActuator : IActuator
    {
        private readonly char[] expressionPartSeparator;
        //private readonly char[] indexExprEndChars;
        //private readonly char[] indexExprStartChars;
        /// <summary>
        /// 反射构造函数
        /// </summary>
        public ReflectionActuator()
        {
            expressionPartSeparator = new char[] { '.' };
            //indexExprEndChars = new char[] { ']', ')' };
            //indexExprStartChars = new char[] { '[', '(' };
        }
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
                PropertyInfo p = DynamicHelpers.GetPropertyInfo(t, propName);
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
        }
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
        /// 调用实例方法
        /// </summary>
        /// <param name="container">实例对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">形参</param>
        /// <returns>object</returns>
        public object CallMethod(object container, string methodName, object[] args)
        {
            Type[] types = new Type[args.Length];
            bool hasNullValue = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null)
                {
                    types[i] = args[i].GetType();
                }
                else
                {
                    hasNullValue = true;
                }
            }

            Type t = container.GetType();
            MethodInfo method = DynamicHelpers.GetMethod(t, methodName, types);

            if (method != null)
            {
                //if (hasParam)
                //{
                //    Array arr;
                //    if (types.Length - 1 == args.Length)
                //    {
                //        arr = null;
                //    }
                //    else
                //    {
                //        arr = Array.CreateInstance(types[types.Length - 1].GetElementType(), args.Length - types.Length + 1);
                //        for (int i = types.Length - 1; i < args.Length; i++)
                //        {
                //            arr.SetValue(args[i], i - (types.Length - 1));
                //        }

                //        object[] newArgs = new object[types.Length];

                //        for (int i = 0; i < newArgs.Length - 1; i++)
                //        {
                //            newArgs[i] = args[i];
                //        }
                //        newArgs[newArgs.Length - 1] = arr;

                //        return method.Invoke(container, newArgs);
                //    }
                //}

                ParameterInfo[] pi = method.GetParameters();
                //处理可选参数
                if (types.Length == 1 && types[0] == typeof(Dictionary<object, object>)
                    && (pi.Length != 1 || !pi[0].ParameterType.IsSubclassOf(typeof(IDictionary))))
                {
                    //实参
                    args = DynamicHelpers.ChangeParameters((Dictionary<object, object>)args[0],pi);
                }
                else if (hasNullValue)
                { 
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == null && pi[i].DefaultValue != null)
                        {
                            args[i] = pi[i].DefaultValue;
                        }
                        //else
                        //{
                        //    args[i] = DefaultForType(pi[i].ParameterType);
                        //}
                    }
                }
                return method.Invoke(container, args);

            }

            return null;
        }

        #endregion

    }
}