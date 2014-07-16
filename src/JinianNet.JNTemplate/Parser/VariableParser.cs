using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public class VariableParser : ITagParser
    {

        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, Token token)
        {
            if (token.TokenKind == TokenKind.TextData &&
                (token.Next == null || token.Next.TokenKind == TokenKind.TagEnd || token.Next.TokenKind == TokenKind.EOF)
                )
            {

            }

            return null;
        }

        #endregion
    }

}
