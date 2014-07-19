using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace JinianNet.JNTemplate.Parser
{
    public class ParserHelpers
    {
        public static bool IsLetter(Char value)
        {
            return Char.IsLower(value) || Char.IsUpper(value);
        }
    }
}
