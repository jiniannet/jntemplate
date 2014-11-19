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
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ForeachTag : SimpleTag
    {

        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Tag source;
        public Tag Source
        {
            get { return source; }
            set { source = value; }
        }

        private void Excute(Object value, TemplateContext context, System.IO.TextWriter writer)
        {
            IEnumerable enumerable = Common.ReflectionHelpers.ToIEnumerable(value);
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                Int32 i = 0;
                while (ienum.MoveNext())
                {
                    i++;
                    ctx.TempData[this.Name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (Int32 n = 0; n < this.Children.Count; n++)
                    {
                        this.Children[n].Parse(ctx, writer);
                    }
                    
                }
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            if (this.Source != null)
            {
                Excute(this.Source.Parse(context), context, writer);
            }
        }

        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(this.Source.Parse(context), context, write);
                return write.ToString();
            }
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(this.Source.Parse(baseValue, context), context, write);
                return write.ToString();
            }
        }
    }
}