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


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// 变量标签
    /// </summary>
    public class VariableTag : SimpleTag
    {

        private String name;
        /// <summary>
        /// 变量名
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            return context.TempData[this.Name];
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="baseValue">baseValue</param>
        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return Common.ReflectionHelpers.Eval(baseValue, this.Name);
        }

    }
}