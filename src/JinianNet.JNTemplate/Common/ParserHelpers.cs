/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
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

        public static Boolean IsEqual(String x, String y)
        {
            if (x == null || y == null)
                return x == y;
            return String.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
