using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestTag : Tag, ITag
    {
        public string Document { get; set; }
    }


    public class TestParser : ITagParser
    {
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 2 && tc.First.Text == ":")
            {
                return new TestTag
                {
                    Document = tc[1].Text
                };
            }
            return null;
        }
    }
}
