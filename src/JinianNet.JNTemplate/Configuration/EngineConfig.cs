/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// The default config of the engine.
    /// </summary>
    public class EngineConfig : Runtime.RuntimeOptions, IConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineConfig"/> class
        /// </summary>
        public EngineConfig() :
            base()
        {

        }

        /// <summary>
        /// Created the default config.
        /// </summary>
        /// <returns></returns>
        public static EngineConfig CreateDefault()
        {
            return new EngineConfig();
        }
    }
}