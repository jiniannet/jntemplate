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
    public class ParserHelpers
    {
        public static bool IsLetter(Char value)
        {
            return Char.IsLower(value) || Char.IsUpper(value);
        }
    }
}
