using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 变量元素
    /// </summary>
    public class VariableElement<T>
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }
    }

}
