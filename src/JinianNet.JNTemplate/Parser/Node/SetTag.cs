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
    public class SetTag : BaseTag
    {
        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Tag _value;
        public Tag Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public override Object Parse(TemplateContext context)
        {
            Object value = this.Value.Parse(context);
            if (!context.TempData.SetValue(this.Name,value))
            {
                context.TempData.Push(this.Name, value);
            }
            return null;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            Parse(context);
        }
    }
}