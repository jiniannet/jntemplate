/*****************************************************
   Copyright (c) 2013-2014 翅膀的初衷  (http://www.jiniannet.com)

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

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// 分析异常类
    /// </summary>
    public class ParseException : TemplateException
    {
        /// <summary>
        /// 模板错误
        /// </summary>
        public ParseException()
            : base()
        {
        }


        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="line">行</param>
        /// <param name="column">字符</param>
        public ParseException(String message, Int32 line, Int32 column)
            : base(message, line,column)
        {
            this.Column = column;
            this.Line = line;
        }

        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public ParseException(String message)
            : base(message)
        {

        }

        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">基础信息</param>
        public ParseException(String message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
