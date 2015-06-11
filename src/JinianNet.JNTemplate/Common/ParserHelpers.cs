/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// 分析辅助类
    /// </summary>
    public class ParserHelpers
    {
        /// <summary>
        /// 是否英文字母
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static Boolean IsLetter(Char value)
        {
            return Char.IsLower(value) || Char.IsUpper(value);
        }
        /// <summary>
        /// 是否单词
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static Boolean IsWord(Char value)
        {
            return Char.IsLower(value) || Char.IsUpper(value) || Char.IsNumber(value) || value == '_';
        }
        /// <summary>
        /// 字符串是否相同
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Boolean IsEqual(String x, String y)
        {
            if (x == null || y == null)
                return x == y;
            return String.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}