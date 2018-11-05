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
        public const string Version = "1.4";
        internal const string KEY_FOREACH = "foreach";
        internal const string KEY_IF = "if";
        internal const string KEY_ELSEIF = "elseif";
        internal const string KEY_ELIF = "elif";
        internal const string KEY_ELSE = "else";
        internal const string KEY_SET = "set";
        internal const string KEY_LOAD = "load";
        internal const string KEY_INCLUDE = "include";
        internal const string KEY_END = "end";
        internal const string KEY_FOR = "for";
        internal const string KEY_IN = "in";
        internal const string KEY_LAYOUT = "layout";
        internal const string KEY_BODY = "BODY";
        /// <summary>
        /// 默认标签解析器
        /// </summary>
        internal static readonly string[] RSEOLVER_TYPES = new string[] {
                "JinianNet.JNTemplate.Parsers.CommentParser",
                "JinianNet.JNTemplate.Parsers.BooleanParser",
                "JinianNet.JNTemplate.Parsers.NumberParser",
                "JinianNet.JNTemplate.Parsers.EleseParser",
                "JinianNet.JNTemplate.Parsers.EndParser",
                "JinianNet.JNTemplate.Parsers.BodyParser",
                "JinianNet.JNTemplate.Parsers.VariableParser",
                "JinianNet.JNTemplate.Parsers.IndexValueParser",
                "JinianNet.JNTemplate.Parsers.StringParser",
                "JinianNet.JNTemplate.Parsers.ForeachParser",
                "JinianNet.JNTemplate.Parsers.ForParser",
                "JinianNet.JNTemplate.Parsers.SetParser",
                "JinianNet.JNTemplate.Parsers.IfParser",
                "JinianNet.JNTemplate.Parsers.ElseifParser",
                "JinianNet.JNTemplate.Parsers.LayoutParser",
                "JinianNet.JNTemplate.Parsers.LoadParser",
                "JinianNet.JNTemplate.Parsers.IncludeParser",
                "JinianNet.JNTemplate.Parsers.FunctionParser",
                "JinianNet.JNTemplate.Parsers.ComplexParser" };
    }


}