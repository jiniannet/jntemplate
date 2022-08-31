/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Hosting;
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    ///  
    /// </summary>
    public class EngineBuilder
    {
        private bool enableCompile =
#if NF35 || NF20
            false;
#else
            true;
#endif

        /// <summary>
        /// Build a engine.
        /// </summary>
        /// <returns></returns>
        public IEngine Build()
        {
            var environment = new DefaultHostEnvironment();
            environment.Options.Mode = enableCompile ? EngineMode.Compiled : EngineMode.Interpreted;
            return new TemplatingEngine(environment);
        }

        /// <summary>
        /// Enable compilation mode.
        /// </summary>
        /// <returns></returns>
        public EngineBuilder UseCompileEngine()
        {
            this.enableCompile = true;
            return this;
        }

        /// <summary>
        /// Disable compilation mode.
        /// </summary>
        /// <returns></returns>
        public EngineBuilder UseInterpretationEngine()
        {
            this.enableCompile = false;
            return this;
        }
    }
}
