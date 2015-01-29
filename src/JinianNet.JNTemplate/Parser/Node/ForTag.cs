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

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ForTag : BaseTag
    {
        private Tag initial;
        private Tag test;
        private Tag dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public Tag Initial
        {
            get { return initial; }
            set { initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        /// <summary>
        /// Do
        /// </summary>
        public Tag Do
        {
            get { return dothing; }
            set { dothing = value; }
        }

        private void Excute(TemplateContext context, System.IO.TextWriter writer)
        {
            this.Initial.Parse(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            Boolean run;

            if (this.Test == null)
            {
                run = false;
            }
            else
            {
                run = this.Test.ToBoolean(context);
            }

            while (run)
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].Parse(context, writer);
                }
                if (this.Do != null)
                {
                    this.Do.Parse(context);
                }
                run = this.Test == null ? true : this.Test.ToBoolean(context);
            }
        }

        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(context, write);
                return write.ToString();
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            Excute(context, write);
        }
    }
}
