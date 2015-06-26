using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 标记模式
    /// </summary>
    public enum FlagMode
    {
        /// <summary>
        /// 无。
        /// </summary>
        None = 0,

        /// <summary>
        /// 简写
        /// </summary>
        Logogram,

        /// <summary>
        /// 完整
        /// </summary>
        Full,

        /// <summary>
        /// 注释
        /// </summary>
        Comment
    }
}
