/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate.Common
{
    /// <summary>
    /// 公用类
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 字符串转布尔
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ToBoolean(string input)
        {
            if ("true".Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否英文字母
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool IsLetter(char value)
        {
            return char.IsLower(value) || char.IsUpper(value);
        }
        /// <summary>
        /// 是否单词
        /// </summary>
        /// <param name="value">字符</param>
        /// <returns></returns>
        public static bool AllowWord(char value)
        {
            return char.IsLower(value) || char.IsUpper(value) || char.IsNumber(value) || value == '_';
        }
        /// <summary>
        /// 字符串是否相同
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsEqual(string x, string y)
        {
            if (x == null || y == null)
                return x == y;
            return string.Equals(x, y, Engine.Runtime.ComparisonIgnoreCase);
        }
    }
}