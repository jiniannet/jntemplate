/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Exception
{
    /// <summary>
    /// 编译错误
    /// </summary>
    public class CompileException : System.Exception
    {
        /// <summary>
        /// CompileException
        /// </summary>
        /// <param name="exception">innexception</param>
        public CompileException(System.Exception exception) :
            base(exception.Message, exception)
        {

        }

        /// <summary>
        /// CompileException
        /// </summary>
        /// <param name="message">message</param>
        public CompileException(string message) :
            base(message)
        {

        }
    }
}