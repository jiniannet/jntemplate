/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    public class CompileException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileException"/> class.
        /// </summary>
        /// <param name="exception">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CompileException(System.Exception exception) :
            base(exception.Message, exception)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CompileException(string message) :
            base(message)
        {

        }
    }
}