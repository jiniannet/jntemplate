/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class SimpleTag : Tag
    {
        public override String ToString()
        {
            if (this.LastToken != null && this.FirstToken != this.LastToken)
            {
                StringBuilder sb = new StringBuilder();
                Token t = this.FirstToken;
                sb.Append(t.ToString());
                while ((t = t.Next) != null && t != this.LastToken)
                {
                    sb.Append(t.ToString());
                }
                sb.Append(this.LastToken.ToString());
                return sb.ToString();
            }
            else
            {
                return this.FirstToken.ToString();
            }
        }

        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            write.Write(Parse(context));
        }
    }
}
