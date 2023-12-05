/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 表示资源管理器，其可在运行时提供对于模板资源的便利访问
    /// </summary>
    public class ResourceManager : List<string>, ICollection<string>, IEnumerable<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool EnableWatch { get; set; }
        //private List<string> resources;
        private IHostEnvironment environment;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public ResourceManager(IHostEnvironment e)
        {
            environment = e;
        }

    }
}
