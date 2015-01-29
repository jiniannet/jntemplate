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
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎基类
    /// </summary>
    public interface IEngine
    {
        //void Render(TemplateContext context, TextWriter writer);

        /// <summary>
        /// 创建Template实现
        /// </summary>
        /// <returns></returns>
        ITemplate CreateTemplate(String path);
    }
}
