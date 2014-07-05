/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class LoadTag : SimpleTag
    {
        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].Parse(context, writer);
                }

                return writer.ToString();
            }
        }

        public override Object Parse(object baseValue, TemplateContext context)
        {
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    writer.Write(this.Children[i].Parse(baseValue, context));
                }

                return writer.ToString();
            }
        }
    }
}