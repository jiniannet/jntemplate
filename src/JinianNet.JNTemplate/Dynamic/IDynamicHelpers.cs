using System;

namespace JinianNet.JNTemplate.Dynamic
{
    /// <summary>
    /// 动态帮助类
    /// </summary>
    public interface IDynamicHelpers
    {
        /// <summary>
        /// 动态执行方法
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">实参</param>
        /// <returns>执行结果（Void返回NULL）</returns>
        Object ExcuteMethod(Object container, String methodName, Object[] args);
        /// <summary>
        /// 动态获取属性或字段
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="propertyName">属性或字段名</param>
        /// <returns>返回结果</returns>
        Object GetPropertyOrField(Object value, String propertyName);
    }
}
