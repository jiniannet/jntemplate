/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using System;

namespace JinianNet.JNTemplate.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public class TemplateException : Exception
    {
        /// <summary>
        /// The line of the exception.
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// The column of the exception.
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// The error code of the exception.
        /// </summary>
        public string Code { get; set; }
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
            : base($"{message} [line:{line},col:{column}]")
        {
            this.Column = column;
            this.Line = line;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateException"/> class.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public TemplateException(ITag tag, string message, System.Exception exception) :
            base(ToString(tag, message), exception)
        {
            if (tag != null && tag.FirstToken != null)
            {
                this.Column = tag.FirstToken.BeginColumn;
                this.Line = tag.FirstToken.BeginLine;
            }
        }



        private static string ToString(ITag tag, string message)
        {
            if (tag != null)
            {
                return $"{message} on {tag.ToSource()} [line:{tag.FirstToken?.BeginLine},col:{tag.FirstToken?.BeginColumn}]";
            }
            return message;
        }
    }
}