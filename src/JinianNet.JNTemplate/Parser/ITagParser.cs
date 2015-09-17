/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate.Parser
{
    /// <summary>
    /// 标签分析器
    /// </summary>
    public interface ITagParser
    {
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        Tag Parse(TemplateParser parser, TokenCollection tc);
    }

    #region Word
    //public class WordParser : ITagParser
    //{
    //    #region ITagParser 成员

    //    public Tag Parse(TemplateParser parser, TokenCollection tc)
    //    {
    //        if (tc.Count == 1 &&
    //            tc.First.TokenKind == TokenKind.TextData
    //            && (Field.KEY_ELSE == tc.First.Text
    //            ||Field.KEY_ELSEIF == tc.First.Text
    //            ||Field.KEY_END == tc.First.Text
    //            ||Field.KEY_FOR == tc.First.Text
    //            ||Field.KEY_FOREACH == tc.First.Text
    //            ||Field.KEY_IF == tc.First.Text
    //            ||Field.KEY_IN == tc.First.Text
    //            ||Field.KEY_INCLUDE == tc.First.Text
    //            ||Field.KEY_LOAD == tc.First.Text
    //            ||Field.KEY_SET == tc.First.Text)
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



#if V1_2_0_0
    public class ExpressionParser : ITagParser
    {
        #region ITagParser 成员

        public Tag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc.Count > 2 && HasOperator(tc))
            {
                Int32 start, end, pos;
                start = end = pos = 0;

                #region 去括号
                //(8+2) ==》 8+2
                //(8+2) * (10-5) ==》(8+2) * (10-5)
                if (tc.First.TokenKind == TokenKind.LeftParentheses && tc.Last.TokenKind == TokenKind.RightParentheses)
                {
                    for (Int32 i = 1; i < tc.Count - 1; i++)
                    {
                        switch (tc[i].TokenKind)
                        {
                            case TokenKind.LeftParentheses:
                                pos++;
                                break;
                            case TokenKind.RightParentheses:
                                if (pos > 0)
                                {
                                    pos--;
                                }
                                break;
                        }
                    }
                    if (pos == 0)
                    {
                        tc = new TokenCollection(tc, 1, tc.Count - 2);
                    }
                    else
                    {
                        pos = 0;
                    }
                }
                #endregion

                ExpressionTag tag = new ExpressionTag();

                #region 执行表达式折分

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
                                if (tc[start].TokenKind == TokenKind.RightParentheses)
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

        public Boolean HasDot(TokenCollection tc)
        {
            Int32 pos = 0;
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
#endif

}