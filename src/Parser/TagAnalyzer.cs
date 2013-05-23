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
    public abstract class TagAnalyzer
    {
        public Token[] CopyTo(Token[] tokens, Int32 start, Int32 count)
        {
            Token[] array = new Token[count];
            for (Int32 i = 0; i < count; i++)
            {
                array[i] = tokens[i + start];
            }

            return array;
        }

        public abstract Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col);
    }
}
