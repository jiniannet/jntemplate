using JinianNet.JNTemplate.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 表达式访问器
    /// </summary>
    public class ExpressionActuator : IActuator
    {
        /// <summary>
        /// 使用Expression访问对象
        /// </summary>
        public ExpressionActuator()
        {

        }
        /// <summary>
        /// 调用索引值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public object CallIndexValue(object value, object index)
        {
            //MemoryCache
            throw new NotImplementedException();
        }

        /// <summary>
        /// 调用方法（警告，不支持重载）
        /// </summary>
        /// <param name="container">原始对象容器</param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallMethod(object container, string methodName, object[] args)
        {
            if (container == null)
            {
                return null;
            }
            //为了提高处理效率，不考虑重载
            Type t = container.GetType();
            var cacheKey = string.Concat(t.FullName, ".", methodName); 
            LambdaExpression func;
            if ((func = CacheHelpers.Get<LambdaExpression>(cacheKey)) == null)
            {
                var ms = DynamicHelpers.GetMethods(t, methodName);
                if (ms.Length == 0)
                {
                    return null;
                }
                if (ms.Length > 1)
                {
                    throw new AmbiguousMatchException(string.Concat("Ambiguous match found. name:",methodName));
                }
                var m = ms[0];
                var pe = new List<ParameterExpression>();
            }
            //ParameterExpression expA = Expression.Parameter(typeof(string)); //参数a
            //var expCall = Expression.Call(Expression.Constant(t), m, expA); //Math.Sin(a)
            //LambdaExpression exp = Expression.Lambda(expCall, "test", new ParameterExpression[] { expA });
            //var r = exp.Compile();

            return null;
        }
        /// <summary>
        /// 调用属性或者索引
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object CallPropertyOrField(object value, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}