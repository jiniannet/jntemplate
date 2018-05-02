/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 系统常用字段
    /// </summary>
    public class Field
    {
        /// <summary>
        /// 当前程序版本
        /// </summary>
        public const String Version = "1.4";
        internal const String KEY_FOREACH = "foreach";
        internal const String KEY_IF = "if";
        internal const String KEY_ELSEIF = "elseif";
        internal const String KEY_ELIF = "elif";
        internal const String KEY_ELSE = "else";
        internal const String KEY_SET = "set";
        internal const String KEY_LOAD = "load";
        internal const String KEY_INCLUDE = "include";
        internal const String KEY_END = "end";
        internal const String KEY_FOR = "for";
        internal const String KEY_IN = "in";
        /// <summary>
        /// 默认标签解析器
        /// </summary>
        internal static readonly String[] RSEOLVER_TYPES = new String[] {
                "JinianNet.JNTemplate.Parsers.BooleanParser",
                "JinianNet.JNTemplate.Parsers.NumberParser",
                "JinianNet.JNTemplate.Parsers.EleseParser",
                "JinianNet.JNTemplate.Parsers.EndParser",
                "JinianNet.JNTemplate.Parsers.VariableParser",
                "JinianNet.JNTemplate.Parsers.StringParser",
                "JinianNet.JNTemplate.Parsers.ForeachParser",
                "JinianNet.JNTemplate.Parsers.ForParser",
                "JinianNet.JNTemplate.Parsers.SetParser",
                "JinianNet.JNTemplate.Parsers.IfParser",
                "JinianNet.JNTemplate.Parsers.ElseifParser",
                "JinianNet.JNTemplate.Parsers.LoadParser",
                "JinianNet.JNTemplate.Parsers.IncludeParser",
                "JinianNet.JNTemplate.Parsers.FunctionParser",
                "JinianNet.JNTemplate.Parsers.ComplexParser" };
    }


}