using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 标签分析器
    /// </summary>
    public interface ITagParser
    {
        Tag Parse(TemplateParser parser, TokenCollection tc);
    }

    public class NumberParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1 && tc.First.TokenKind == TokenKind.Number)
            {
                NumberTag tag = new NumberTag();
                if (tc.First.Text.IndexOf('.') == -1)
                {
                    tag.Value = Int32.Parse(tc.First.Text);
                }
                else
                {
                    tag.Value = Double.Parse(tc.First.Text);
                }

                return tag;
            }

            return null;
        }

        #endregion
    }

    public class BooleanParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && (tc.First.Text == "true" || tc.First.Text == "false"))
            {
                BooleanTag tag = new BooleanTag();
                tag.Value = tc.First.Text == "true";
                return tag;
            }

            return null;
        }

        #endregion
    }
    //true false
    #region Word
    //public class WordParser : ITagParser
    //{
    //    #region ITagParser 成员

    //    public Tag Parse(TemplateParser parser, TokenCollection tc)
    //    {
    //        if (tc.Count == 1 &&
    //            tc.First.TokenKind == TokenKind.TextData
    //            && (Field.KEY_ELSE == tc.First.Text
    //            || Field.KEY_ELSEIF == tc.First.Text
    //            || Field.KEY_END == tc.First.Text
    //            || Field.KEY_FOR == tc.First.Text
    //            || Field.KEY_FOREACH == tc.First.Text
    //            || Field.KEY_IF == tc.First.Text
    //            || Field.KEY_IN == tc.First.Text
    //            || Field.KEY_INCLUDE == tc.First.Text
    //            || Field.KEY_LOAD == tc.First.Text
    //            || Field.KEY_SET == tc.First.Text)
    //            )
    //        {
    //            WordTag tag = new WordTag();
    //            tag.Name = tc.First.Text;
    //            return tag;
    //        }

    //        return null;
    //    }
    //    #endregion
    //}
    #endregion

    public class EleseParser : ITagParser
    {
        #region ITagParser 成员
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && tc.First.Text == Field.KEY_ELSE)
            {
                return new ElseTag();
            }

            return null;
        }
        #endregion
    }

    public class EndParser : ITagParser
    {
        #region ITagParser 成员
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 1
                && tc.First.Text == Field.KEY_END)
            {
                return new EndTag();
            }

            return null;
        }
        #endregion
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

    public class StringParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count == 3
                && tc.First.TokenKind == TokenKind.StringStart
                && tc[1].TokenKind == TokenKind.String
                && tc.Last.TokenKind == TokenKind.StringEnd
                )
            {
                StringTag tag = new StringTag();
                tag.Value = tc[1].Text;
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
                    coll.Add(tc, 4, tc.Count - 2);
                    tag.Source = parser.Read(coll);

                    while (parser.MoveNext())
                    {
                        tag.Children.Add(parser.Current);
                        if (parser.Current is EndTag)
                        {
                            return tag;
                        }
                    }

                    throw new Exception.ParseException("未闭合的标签",tc.First.BeginLine,tc.First.BeginColumn);
                }
                else
                {
                    throw new Exception.ParseException("foreach标签语法错误", tc.First.BeginLine, tc.First.BeginColumn);
                }

            }

            return null;
        }


        #endregion
    }

    public class ForParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 3 && tc.First.TokenKind == TokenKind.TextData
                && tc.First.Text == Field.KEY_FOR)
            {
                ForTag tag = new ForTag();
                for (Int32 i = 2; i < tc.Count; i++)
                {
                    //end = i;
                    //switch (tc[i].TokenKind)
                    //{
                    //    case TokenKind.Punctuation:
                    //        if (pos == 0 && tc[i].Text==";")
                    //        {
                    //            TokenCollection coll = new TokenCollection();
                    //            coll.Add(tc, start, end - 1);
                    //            if (coll.Count > 0)
                    //            {
                    //                tag.AddChild(parser.Read(coll));
                    //            }
                    //            start = i + 1;
                    //        }
                    //        break;
                    //    default:
                    //        if (tc[i].TokenKind == TokenKind.LeftParentheses)
                    //        {
                    //            pos++;
                    //        }
                    //        else if (tc[i].TokenKind == TokenKind.RightParentheses)
                    //        {
                    //            pos--;
                    //        }
                    //        if (i == tc.Count - 1)
                    //        {
                    //            TokenCollection coll = new TokenCollection();
                    //            coll.Add(tc, start, end - 1);
                    //            if (coll.Count > 0)
                    //            {
                    //                tag.AddChild(parser.Read(coll));
                    //            }
                    //        }
                    //        break;
                    //}

                }

                return tag;
            }

            return null;
        }

        #endregion
    }

    public class IfParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 3
                && tc.First.TokenKind == TokenKind.TextData
                && tc.First.Text == Field.KEY_IF)
            {
                IfTag tag = new IfTag();

                ElseifTag t = new ElseifTag();
                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 2, tc.Count - 2);
                t.Test = parser.Read(coll);
                t.FirstToken = coll.First;
                //t.LastToken = coll.Last;
                tag.AddChild(t);

                while (parser.MoveNext())
                {
                    if (parser.Current is EndTag)
                    {
                        tag.AddChild(parser.Current);
                        return tag;
                    }
                    else if (parser.Current is ElseifTag
                        || parser.Current is ElseTag)
                    {
                        tag.AddChild(parser.Current);
                    }
                    else
                    {
                        tag.Children[tag.Children.Count - 1].AddChild(parser.Current);
                    }
                }

                return tag;

            }

            return null;
        }

        #endregion
    }

    public class ElseifParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 3
                && tc.First.Text == Field.KEY_ELSEIF)
            {
                ElseifTag tag = new ElseifTag();

                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 2, tc.Count - 2);
                tag.Test = parser.Read(coll);

                return tag;
            }

            return null;
        }

        #endregion
    }

    public class SetParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            //支持写法：简写格式：
            //常规格式：
            if (tc.Count > 5
                && tc.First.TokenKind == TokenKind.TextData
                && tc.First.Text == Field.KEY_SET
                && tc[1].TokenKind == TokenKind.LeftParentheses
                && tc[3].Text == "="
                && tc.Last.TokenKind == TokenKind.RightParentheses)
            {
                SetTag tag = new SetTag();
                tag.Name = tc[2].Text;

                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 4, tc.Count - 2);

                tag.Value = parser.Read(coll);
                return tag;

            }
            else if (tc.Count > 2
                && tc.First.TokenKind == TokenKind.TextData
                && tc[1].Text == "=")
            {
                SetTag tag = new SetTag();
                tag.Name = tc.First.Text;

                TokenCollection coll = new TokenCollection();
                coll.Add(tc, 2, tc.Count - 1);

                tag.Value = parser.Read(coll);
                return tag;
            }

            return null;
        }

        #endregion
    }

    public class LoadParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.First.Text == Field.KEY_LOAD)
            {
                if (tc.Count > 2
                    && (tc[1].TokenKind == TokenKind.LeftParentheses)
                    && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    LoadTag tag = new LoadTag();
                    tag.Path = parser.Read(new TokenCollection(tc, 2, tc.Count - 2));
                    return tag;
                }
            }

            return null;
        }

        #endregion
    }

    public class IncludeParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.First.Text == Field.KEY_INCLUDE)
            {
                if (tc.Count > 2
                    && (tc[1].TokenKind == TokenKind.LeftParentheses)
                    && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    IncludeTag tag = new IncludeTag();
                    tag.Path = parser.Read(new TokenCollection(tc, 2, tc.Count - 2));
                    return tag;
                }
            }

            return null;
        }

        #endregion
    }

    public class ExpressionParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2 && HasOperator(tc))
            {
                Int32 start, end, pos;
                start = end = pos = 0;

                ExpressionTag tag = new ExpressionTag();

                #region

                for (Int32 i = 0; i < tc.Count; i++)
                {
                    end = i;
                    switch (tc[i].TokenKind)
                    {
                        case TokenKind.Operator:
                            if (pos == 0)
                            {
                                if (start != end)
                                {
                                    TokenCollection coll = new TokenCollection();
                                    coll.Add(tc, start, end - 1);
                                    tag.AddChild(parser.Read(coll));
                                }
                                tag.AddChild(new TextTag());
                                tag.Children[tag.Children.Count - 1].FirstToken = tc[i];
                                start = i + 1;
                            }
                            break;
                        default:
                            if (tc[i].TokenKind == TokenKind.LeftParentheses)
                            {
                                pos++;
                            }
                            else if (tc[i].TokenKind == TokenKind.RightParentheses)
                            {
                                pos--;
                            }
                            if (i == tc.Count - 1)
                            {
                                TokenCollection coll = new TokenCollection();
                                if (tc[start].TokenKind == TokenKind.RightBracket)
                                {
                          
                                    coll.Add(tc, start + 1, end - 1);
                                }
                                else
                                {
                                    coll.Add(tc, start, end);
                                }
                                start = i + 1;
                                if (coll.Count > 0)
                                {
                                    tag.AddChild(parser.Read(coll));
                                }
                            }
                            break;
                    }
                }

                #endregion

                if (tag.Children.Count > 0)
                {
                    if (tag.Children.Count == 1)
                    {
                        return tag.Children[0];
                    }
                    return tag;
                }
            }
            return null;
        }

        private Boolean HasOperator(TokenCollection tc)
        {
            for (Int32 i = 0; i < tc.Count; i++)
            {
                if (tc[i].TokenKind == TokenKind.Operator)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    public class ReferenceParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2
                && tc.First.TokenKind == TokenKind.TextData
                && HasDot(tc))
            {
                ReferenceTag tag = new ReferenceTag();
                Int32 start, end, pos;
                start = end = pos = 0;

                for (Int32 i = 0; i < tc.Count; i++)
                {

                    end = i;
                    switch (tc[i].TokenKind)
                    {

                        case TokenKind.Dot:
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                tag.AddChild(parser.Read(coll));
                                start = i + 1;
                            }
                            break;
                        default:
                            if (tc[i].TokenKind == TokenKind.LeftParentheses)
                            {
                                pos++;
                            }
                            else if (tc[i].TokenKind == TokenKind.RightParentheses)
                            {
                                pos--;
                            }
                            if (i == tc.Count - 1)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end);
                                tag.AddChild(parser.Read(coll));
                            }
                            break;
                    }
                }
                if (tag.Children.Count > 0)
                {
                    if (tag.Children.Count == 1)
                    {
                        return tag.Children[0];
                    }
                }
                return tag;
            }

            return null;
        }

        public bool HasDot(TokenCollection tc)
        {
            int pos = 0;
            for (Int32 i = 0; i < tc.Count; i++)
            {
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
                            return true;
                        }
                        break;
                }
            }
            return false;
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
                    start = 2,
                    end;

                for (Int32 i = 2; i < tc.Count; i++)
                {
                    end = i;
                    switch (tc[i].TokenKind)
                    {
                        case TokenKind.Comma:
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                if (coll.Count > 0)
                                {
                                    tag.AddChild(parser.Read(coll));
                                }
                                start = i + 1;
                            }
                            break;
                        default:
                            if (tc[i].TokenKind == TokenKind.LeftParentheses)
                            {
                                pos++;
                            }
                            else if (tc[i].TokenKind == TokenKind.RightParentheses)
                            {
                                pos--;
                            }
                            if (i == tc.Count - 1)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                if (coll.Count > 0)
                                {
                                    tag.AddChild(parser.Read(coll));
                                }
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

#if comlex
    //太过复杂的表达式 ，比如：
    //(num*(value.getNum(x,y)+8)*84).ToString("#0.00") 
    //因为处理太过麻烦 本版本暂不实现

    public class ComplexParser : ITagParser
    {
        /*
        nom.aa(38+24)
        nom.aa(38+24).ToString("#0.00")
        (num*(3+8)-45).ToString("#0.00")
        (num*(value.getNum()+8).toFxi()-45).ToString("#0.00")
        (num*(value.getNum(x,y)+8)*84).ToString("#0.00") 
        (num*(value.getNum()+8).toFxi()-45).ToString("#0.00")
        1. 
        (num*(value.getNum()+8).toFxi()-45)   - 
                                                  num
                                                  (value.getNum()+8).toFxi()
                                                  45
        ToString("#0.00") - 
         */
    #region ITagParser 成员
        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            List<Tag> list = new List<Tag>();
            Int32 start, end, pos;
            start = end = pos = 0;

            if (tc.Count > 0)
            {
                for (Int32 i = 0; i < tc.Count; i++)
                {
                    end = i;
                    switch (tc[i].TokenKind)
                    {
                        case TokenKind.Operator:
                        case TokenKind.Dot:
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                coll.Add(tc, start, end - 1);
                                list.Add(parser.Read(coll));
                                list.Add(new TextTag());
                                list[list.Count - 1].FirstToken = tc[i];
                                start = i + 1;
                            }
                            break;
                        case TokenKind.LeftParentheses:
                            pos++;
                            break;
                        case TokenKind.RightParentheses:
                            pos--;
                            if (pos == 0)
                            {
                                TokenCollection coll = new TokenCollection();
                                if (tc[start].TokenKind == TokenKind.TextData)
                                {
                                    coll.Add(tc, start, end);
                                }
                                else
                                {
                                    coll.Add(tc, start + 1, end - 1);
                                }
                                start = i + 1;
                                list.Add(parser.Read(coll));
                            }
                            break;
                    }
                }
            }

            return null;
        }

    #endregion

    }
#endif
}