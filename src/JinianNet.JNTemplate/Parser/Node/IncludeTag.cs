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
    public class IncludeTag : SimpleTag
    {
        private Tag path;
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        private String LoadResource(Object path, TemplateContext context)
        {
            if (path != null)
            {
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    return Resources.LoadResource(path.ToString(), context.Charset);
                }
                else
                {
                    return Resources.LoadResource(new String[] { context.CurrentPath }, path.ToString(), context.Charset);
                }
            }
            return null;
        }

        public override Object Parse(TemplateContext context)
        {
            Object path = this.Path.Parse(context);
            return LoadResource(path.ToString(), context);

        }

        public override Object Parse(object baseValue, TemplateContext context)
        {
            Object path = this.Path.Parse(baseValue, context);
            return LoadResource(path.ToString(), context);
        }
    }
}