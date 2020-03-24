/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// 常规性错误
    /// </summary>
    public class TemplateException : System.Exception
    {
        private int errorLine;
        private int errorColumn;
        private string errorCode;
        /// <summary>
        /// 所在行
        /// </summary>
        public int Line
        {
            get { return this.errorLine; }
            set { this.errorLine = value; }
        }
        /// <summary>
        /// 所在字符
        /// </summary>
        public int Column
        {
            get { return this.errorColumn; }
            set { this.errorColumn = value; }
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Code
        {
            get { return this.errorCode; }
            set { this.errorCode = value; }
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        public TemplateException()
            : base()
        {

        }

        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="line">行</param>
        /// <param name="column">字符</param>
        public TemplateException(string message, int line, int column)
            : base(string.Concat("Line:",
                line.ToString(),
                " Column:",
                column.ToString(),
                "\r\n",
                message))
        {
            this.errorColumn = column;
            this.errorLine = line;
        }

        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public TemplateException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">基础信息</param>
        public TemplateException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}