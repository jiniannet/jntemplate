using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// 分析异常类
    /// </summary>
    public class ParseException : System.Exception
    {
        private Int32 errorLine;
        private Int32 errorColumn;
        /// <summary>
        /// 所在行
        /// </summary>
        public Int32 Line
        {
            get { return errorLine; }
            set { errorLine = value; }
        }
        /// <summary>
        /// 所在字符
        /// </summary>
        public Int32 Column
        {
            get { return errorColumn; }
            set { errorColumn = value; }
        }

        /// <summary>
        /// ParseException
        /// </summary>
        public ParseException()
            : base()
        {

        }
        /// <summary>
        /// ParseException
        /// </summary>
        /// <param name="message">异常信息</param>
        public ParseException(String message)
            : base(message)
        {

        }
        /// <summary>
        /// ParseException
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="line">行</param>
        /// <param name="column">字符</param>
        public ParseException(String message,Int32 line,Int32 column)
            : base(message)
        {
            this.errorColumn = column;
            this.errorLine = line;
        }
        /// <summary>
        /// ParseException
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">基础异常</param>
        public ParseException(String message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return String.Concat("Line:",
                this.Line.ToString(),
                " Column:",
                this.Column.ToString(),
                "\r\n",
                base.ToString());
        }
    }
}
