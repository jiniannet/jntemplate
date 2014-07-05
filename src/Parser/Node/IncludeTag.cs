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
    public class IncludeTag : SimpleTag
    {
        private Tag path;
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            throw new NotImplementedException();
        }

        public override Object Parse(object baseValue, TemplateContext context)
        {
            return Parse(context);
        }
    }
}