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
using System.Text;


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// INCLUDE标签
    /// </summary>
    public class IncludeTag : BaseTag
    {
        private Tag path;
        /// <summary>
        /// 模板路径
        /// </summary>
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        protected String LoadResource(Object path, TemplateContext context)
        {
            if (path != null)
            {
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    return Resources.LoadResource(context.Config.Paths, path.ToString(), context.Charset);
                }
                else
                {
                    return Resources.LoadResource(
                        Resources.MergerPaths(context.Config.Paths, context.CurrentPath),
                        path.ToString(),
                        context.Charset);
                }
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            Object path = this.Path.Parse(context);
            return LoadResource(path.ToString(), context);
        }

    }
}