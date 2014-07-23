using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public class Parser
    {
        private readonly static List<ITagParser> collection;

        static Parser()
        {
            collection = new List<ITagParser>();
            Reset();
        }

        public static Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            Tag t;
            for (Int32 i = 0; i < collection.Count; i++)
            {
                t = collection[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 || tc.Last.CompareTo(t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }

        public static void Add(ITagParser item)
        {
            collection.Add(item);
        }

        public static void Insert(Int32 index, ITagParser item)
        {
            collection.Insert(index, item);
        }

        public static void Clear()
        {
            collection.Clear();
        }

        public static void Reset()
        {
            collection.Clear();
            collection.Add(new BooleanParser());
            collection.Add(new NumberParser());
            collection.Add(new EleseParser());
            collection.Add(new EndParser());
            collection.Add(new VariableParser());
            collection.Add(new StringParser());
            collection.Add(new ForeachParser());
            collection.Add(new SetParser());
            collection.Add(new IfParser());
            collection.Add(new ElseifParser());
            collection.Add(new LoadParser());
            collection.Add(new IncludeParser());
            collection.Add(new ExpressionParser());
            collection.Add(new ReferenceParser());
            collection.Add(new FunctionParser());
        }
    }
}
