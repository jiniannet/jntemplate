/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
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