/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 表示词法分析模式的枚举值。
    /// </summary>
    /// <remarks></remarks>
    public enum LexerMode
    {
        /// <summary>
        /// 未定义状态。
        /// </summary>
        None = 0,

        /// <summary>
        /// 进入标签。
        /// </summary>
        EnterLabel,

        /// <summary>
        /// 脱离标签。
        /// </summary>
        LeaveLabel,

    }
}