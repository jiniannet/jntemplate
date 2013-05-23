/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 /*****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{

    public class Analyzers : List<TagAnalyzer>
    {
        public Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
        {
            Tag tag = null;
            for (int i = 0; i < this.Count; i++)
            {
                tag = this[i].Parse(parser, tokens, line, col);
                if (tag != null)
                {
                    break;
                }
            }
            return tag;
        }
    }
}
