/*****************************************************
   Copyright (c) 2013-2014 翅膀的初衷  (http://www.jiniannet.com)

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


namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// IF标题
    /// </summary>
    public class IfTag : SimpleTag
    {

        public override Object Parse(TemplateContext context)
        {
            for (Int32 i = 0; i < this.Children.Count-1; i++) //最后面一个子对象为EndTag
            {
                if (this.Children[i].ToBoolean(context))
                {
                    return this.Children[i].Parse(context);
                }
            }
            return null;
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            for (Int32 i = 0; i < this.Children.Count - 1; i++)
            {
                if (this.Children[i].ToBoolean(context))
                {
                    return this.Children[i].Parse(baseValue,context);
                }
            }
            return null;
        }
    }
}