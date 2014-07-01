using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public interface ITagParser
    {
        Tag Parse(TemplateParser parser, Token token);
    }
}
