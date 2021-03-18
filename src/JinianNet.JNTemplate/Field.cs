/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// System fields.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Varsion
        /// </summary>
        public const string Version = "2.0.1";
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
        /// default parsers
        /// </summary>
        internal static readonly string[] RSEOLVER_TYPES = new string[] {
                "JinianNet.JNTemplate.Parsers.CommentParser",
                "JinianNet.JNTemplate.Parsers.BooleanParser",
                "JinianNet.JNTemplate.Parsers.NumberParser",
                "JinianNet.JNTemplate.Parsers.EleseParser",
                "JinianNet.JNTemplate.Parsers.EndParser",
                "JinianNet.JNTemplate.Parsers.BodyParser",
                "JinianNet.JNTemplate.Parsers.NullParser",
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
                "JinianNet.JNTemplate.Parsers.JsonParser",
                "JinianNet.JNTemplate.Parsers.ComplexParser" };
    }


}