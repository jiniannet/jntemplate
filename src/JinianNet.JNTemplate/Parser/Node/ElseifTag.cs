/*****************************************************
   Copyright (c) 2013-2014 jiniannet (http://www.jiniannet.com)

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
    public class ElseifTag : SimpleTag
    {
        private Tag test;
        public virtual Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            if (this.Children.Count == 1)
            {
                return this.Children[0].Parse(context);
            }
            else
            {
                using (System.IO.StringWriter write = new System.IO.StringWriter())
                {
                    for (Int32 i = 0; i < this.Children.Count; i++)
                    {
                        this.Children[i].Parse(context, write);
                    }
                    return write.ToString();
                }
            }

        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {

            if (this.Children.Count == 1)
            {
                return this.Children[0].Parse(context, context);
            }
            else
            {
                using (System.IO.StringWriter write = new System.IO.StringWriter())
                {
                    for (Int32 i = 0; i < this.Children.Count; i++)
                    {
                        write.Write(this.Children[i].Parse(baseValue, context));
                    }
                    return write.ToString();
                }
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {

            for (Int32 i = 0; i < this.Children.Count; i++)
            {
                this.Children[0].Parse(context, write);
            }

        }


        public override Boolean ToBoolean(TemplateContext context)
        {
            return this.Test.ToBoolean(context);
        }

    }
}
