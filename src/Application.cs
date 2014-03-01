/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate
{
    public enum ElementType
    {
        None = 0,
        Var = 1,
        Function = 2,
        Object = 3,
        Set = 4,
        Load = 5,
        Include = 6,
        Expression = 7,
        Foreach = 8,
        If = 9,
    }

    public enum ValueType
    {
        None = 0,
        Text = 1,
        Integer = 2,
        Decimal = 3,
        Boolen = 4,
        Punctuation = 5,
        Mark = 6
    }

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

    public delegate Object FuncHandler(params Object[] args);
}
