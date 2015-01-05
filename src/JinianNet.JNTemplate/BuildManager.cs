/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    public class BuildManager
    {
        private readonly static EngineCollection _engines = new EngineCollection();
        /// <summary>
        /// 模板处理引擎
        /// </summary>
        public static EngineCollection Engines
        {
            get {
                return _engines;
            }
        }

        /// <summary>
        /// 创建Template实殃
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
                template = engine.CreateTemplate(path);
                if (template != null)
                {
                    return template;
                }
            }

            return null;

        }
    }
}
