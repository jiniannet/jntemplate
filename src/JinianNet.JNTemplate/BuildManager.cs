/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 构建器
    /// </summary>
    public class BuildManager
    {
        private readonly static EngineCollection engines = new EngineCollection();
        /// <summary>
        /// 模板处理引擎
        /// </summary>
        public static EngineCollection Engines
        {
            get
            {
                return engines;
            }
        }

        /// <summary>
        /// 创建Template实例
        /// </summary>
        /// <param name="path">模板路径</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate(String path)
        {
            if (Engines.Count == 0)
            {
                Engines.Add(new Engine());
            }

            ITemplate template = null;
            foreach (IEngine engine in Engines)
            {
                if (engine != null)
                {
                    template = engine.CreateTemplate(path);
                    if (template != null)
                    {
                        return template;
                    }
                }
            }

            return null;

        }
    }
}