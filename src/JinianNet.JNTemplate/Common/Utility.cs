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
        public static Boolean ToBoolean(String input)
        {
            if ("true".Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}
