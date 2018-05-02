/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

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
            Column = column;
            Line = line;
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