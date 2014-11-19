/*****************************************************
   Copyright (c) 2013-2014 jiniannet (http://www.jiniannet.com)

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
