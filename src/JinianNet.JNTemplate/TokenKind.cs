/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

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
        /// 非标签文本
        /// </summary>
        Text,
        /// <summary>
        /// 标签内文本
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
        /// 右圆括号
        /// </summary>
        RightParentheses,
        /// <summary>
        /// 左大括号
        /// </summary>
        LeftBrace,
        /// <summary>
        /// 右大括号
        /// </summary>
        RightBrace,
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
        /// 冒号
        /// </summary>
        Colon,
        /// <summary>
        /// 注释
        /// </summary>
        Comment,
        /// <summary>
        /// 结束
        /// </summary>
        EOF
    }

}