/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  
    /// </summary>
    public class EngineBuilder
    {
        /// <summary>
        /// Build a engine.
        /// </summary>
        /// <returns></returns>
        public IEngine Build()
        {
            return new TemplatingEngine();
        }
    }
}
