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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Token种类
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 文本数据
        /// </summary>
        TextData,
        /// <summary>
        /// 标签
        /// </summary>
        Tag,
        /// <summary>
        /// 标签开始标记
        /// </summary>
        TagStart,
        /// <summary>
        /// 标签结束标记
        /// </summary>
        TagEnd,
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// 左中括号
        /// </summary>
        LeftBracket,
        /// <summary>
        /// 右中括号
        /// </summary>
        RightBracket,
        /// <summary>
        /// 左圆括号
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// 右员括号
        /// </summary>
        RightParentheses,
        /// <summary>
        /// 新行（换行符）
        /// </summary>
        NewLine,
        /// <summary>
        /// 点
        /// </summary>
        Dot,
        /// <summary>
        /// 字符串开始
        /// </summary>
        StringStart,
        /// <summary>
        /// 字符串结束
        /// </summary>
        StringEnd,
        /// <summary>
        /// 空格
        /// </summary>
        Space,
        /// <summary>
        /// 标点
        /// </summary>
        Punctuation,
        /// <summary>
        /// 运算符
        /// </summary>
        Operator,
        /// <summary>
        /// 逗号
        /// </summary>
        Comma,
        /// <summary>
        /// 结束
        /// </summary>
        EOF
    }

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

    /// <summary>
    /// 方法标签委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate Object FuncHandler(params Object[] args);
}
