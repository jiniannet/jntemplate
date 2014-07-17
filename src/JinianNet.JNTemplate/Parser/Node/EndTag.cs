/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class EndTag:SimpleTag
    {
        public override Object Parse(TemplateContext context)
        {
            return null;
        }

        public override Object Parse(Object baseValue, TemplateContext context)
        {
            return null;
        }

        public override Boolean ToBoolean(TemplateContext context)
        {
            return false;
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            
        }
    }
}
