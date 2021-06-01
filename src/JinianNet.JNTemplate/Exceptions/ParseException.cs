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
    public class ParseException : TemplateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        public ParseException()
            : base()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="line">The line on error.</param>
        /// <param name="column">The column on error.</param>
        public ParseException(string message, int line, int column)
            : base(message, line, column)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ParseException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ParseException(ITag tag, System.Exception innerException)
            : base(tag, innerException.Message, innerException)
        {

        }
    }
}