using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    public interface ITagParser
    {
        Tag Parse(TemplateParser parser, TokenCollection tc);
    }


    public class VariableParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1 &&
                tc.First.TokenKind == TokenKind.TextData)
            {
                VariableTag tag = new VariableTag();
                tag.Name = tc.First.Text;
                return tag;
            }

            return null;
        }

        #endregion
    }

    public class FunctionParser : ITagParser
    {

        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        { 
            if (tc.First.TokenKind == TokenKind.TextData
                && tc.Count > 2
                && (tc[1].TokenKind == TokenKind.LeftParentheses)
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                FunctaionTag tag = new FunctaionTag();

                tag.Name = tc.First.Text;

                Int32 pos = 0,
                    start = 1,
                    end = 1;

                for (Int32 i = 1; i < tc.Count; i++)
                {
                    end = i;
                    switch (tc[i].TokenKind)
                    {
                        case TokenKind.LeftParentheses:
                            pos++;
                            break;
                        case TokenKind.RightParentheses:
                            pos--;
                            break;
                        case TokenKind.Dot:
                            if (pos == 0)
                            {
                                return null;
                            }
                            else
                            {
                                break;
                            }
                        case TokenKind.Comma:
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                tag.AddChild(parser.Read(coll));
                                start = i+1;
                            }
                            break;
                        default:
                            if (i == tc.Count - 1)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                tag.AddChild(parser.Read(coll));
                            }
                            break;
                    }
                   
                }

                return tag;

            }

            return null;
        }

        #endregion
    }

    public class ForeachParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (Field.KEY_FOREACH == tc.First.Text)
            {
                if (tc.Count > 5
                    && tc[1].TokenKind == TokenKind.LeftParentheses
                    && tc[2].TokenKind == TokenKind.TextData
                    && Field.KEY_IN.Equals(tc[3].Text, StringComparison.OrdinalIgnoreCase)
                    && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    ForeachTag tag = new ForeachTag();
                    tag.Name = tc[2].Text;
                    TokenCollection coll = new TokenCollection();
                    coll.Add(tc,5,tc.Count-1);
                    tag.Source = parser.Read(coll);

                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
                        if (parser.Current is WordTag && Field.KEY_END == parser.Current.ToString())
                        {
                            return tag;
                        }
                    }

                    throw new Exception("can not find #end");
                }
                else
                {
                    throw new Exception("-");
                }

            }

            return null;
        }


        #endregion
    }

    public class WordParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1 &&
                tc.First.TokenKind == TokenKind.TextData
                && (Field.KEY_ELSE == tc.First.Text
                || Field.KEY_ELSEIF == tc.First.Text
                || Field.KEY_END == tc.First.Text
                || Field.KEY_FOR == tc.First.Text
                || Field.KEY_FOREACH == tc.First.Text
                || Field.KEY_IF == tc.First.Text
                || Field.KEY_IN == tc.First.Text
                || Field.KEY_INCLUDE == tc.First.Text
                || Field.KEY_LOAD == tc.First.Text
                || Field.KEY_SET == tc.First.Text)
                )
            {
                WordTag tag = new WordTag();
                tag.Name = tc.First.Text;
                return tag;
            }

            return null;
        }
        #endregion
    }


}
