/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public class TemplateException : System.Exception
    {
        private int errorLine;
        private int errorColumn;
        private string errorCode;
        /// <summary>
        /// The line of the exception.
        /// </summary>
        public int Line
        {
            get { return this.errorLine; }
            set { this.errorLine = value; }
        }
        /// <summary>
        /// The column of the exception.
        /// </summary>
        public int Column
        {
            get { return this.errorColumn; }
            set { this.errorColumn = value; }
        }
        /// <summary>
        /// The error code of the exception.
        /// </summary>
        public string Code
        {
            get { return this.errorCode; }
            set { this.errorCode = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateException"/> class.
        /// </summary>
        public TemplateException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="line">The line on error.</param>
        /// <param name="column">The column on error.</param>
        public TemplateException(string message, int line, int column)
            : base($"Line:{line} Column:{ column}\r\n{message}")
        {
            this.errorColumn = column;
            this.errorLine = line;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public TemplateException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public TemplateException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }
}