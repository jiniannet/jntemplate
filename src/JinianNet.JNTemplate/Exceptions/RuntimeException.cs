/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public class RuntimeException : TemplateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileException"/> class.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public RuntimeException(ITag tag, string message, System.Exception exception) :
            base(tag, message, exception)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CompileException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public RuntimeException(string message, System.Exception exception) :
            base(null, message, null)
        {
        }
    }
}
