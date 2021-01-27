/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// Complex标签分析器
    /// </summary>
    public class ComplexParser : ITagParser
    {
        #region
        private static List<Operator> operators = new List<Operator>(new Operator[] { Operator.Equal, Operator.NotEqual, Operator.LessThan, Operator.LessThanOrEqual, Operator.GreaterThan, Operator.GreaterThanOrEqual });
        #endregion

        #region ITagParser 成员
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser">TemplateParser</param>
        /// <param name="tc">Token集合</param>
        /// <returns></returns>
        public ITag Parse(TemplateParser parser, TokenCollection tc)
        {
            if (tc != null
                && parser != null
                && tc.Count > 2)
            {

                List<ITag> tags = new List<ITag>();
                TokenCollection[] tcs = tc.Split(0, tc.Count, TokenKind.Dot, TokenKind.Operator);
                bool isLogical = false;
                if (tcs.Length == 1)
                {
                    return null;
                }
                for (int i = 0; i < tcs.Length; i++)
                {
                    if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Dot)
                    {
                        if (tags.Count == 0 || i == tcs[i].Count - 1 || (tcs[i + 1].Count == 1 && (tcs[i + 1][0].TokenKind == TokenKind.Dot || tcs[i + 1][0].TokenKind == TokenKind.Operator)))
                        {
                            throw new Exception.ParseException(string.Concat("syntax error near .:", tc), tcs[i][0].BeginLine, tcs[i][0].BeginColumn);
                        }
                        if (tags[tags.Count - 1] is ReferenceTag)
                        {
                            tags[tags.Count - 1].AddChild(parser.Read(tcs[i + 1]));
                        }
                        else
                        {
                            ReferenceTag t = new ReferenceTag();
                            t.AddChild(tags[tags.Count - 1]);
                            t.AddChild(parser.Read(tcs[i + 1]));
                            tags[tags.Count - 1] = t;
                        }
                        i++;
                    }
                    else if (tcs[i].Count == 1 && tcs[i][0].TokenKind == TokenKind.Operator)
                    {
                        tags.Add(new OperatorTag(tcs[i][0]));

                        Operator op = Dynamic.OperatorConvert.Parse(tcs[i][0].Text);
                        switch (op)
                        {
                            case Operator.Or:
                            case Operator.And:
                            case Operator.LessThan:
                            case Operator.LessThanOrEqual:
                            case Operator.Equal:
                            case Operator.GreaterThan:
                            case Operator.GreaterThanOrEqual:
                            case Operator.NotEqual:
                                isLogical = true;
                                break;
                        }
                    }
                    else if (tcs[i][0].TokenKind == TokenKind.LeftBracket)
                    {
                        if (tags[tags.Count - 1] is ReferenceTag)
                        {
                            tags[tags.Count - 1].AddChild(parser.Read(tcs[i]));
                        }
                        else
                        {
                            if (tags.Count == 0)
                            {
                                throw new Exception.ParseException(string.Concat("syntax error near [:", tc), tcs[i][0].BeginLine, tcs[i][0].BeginColumn);
                            }
                            ReferenceTag t = new ReferenceTag();
                            t.AddChild(tags[tags.Count - 1]);
                            t.AddChild(parser.Read(tcs[i]));
                            tags[tags.Count - 1] = t;
                        }
                    }
                    else if (tcs[i].Count > 0)
                    {
                        if (tcs[i].First.TokenKind == TokenKind.LeftParentheses && tcs[i].Last.TokenKind == TokenKind.RightParentheses)
                        {
                            tcs[i].RemoveAt(0);
                            tcs[i].RemoveAt(tcs[i].Count - 1);
                        }
                        tags.Add(parser.Read(tcs[i]));
                    }
                }

                if (tags.Count == 1)
                {
                    return tags[0];
                }

                if (tags.Count > 1)
                {
                    var list = new List<List<ITag>>();
                    ITag t;
                    if (isLogical)
                    {
                        t = new LogicTag();
                        var arr = Analysis(tags, new List<Operator>(new Operator[] { Operator.And, Operator.Or }));
                        if (arr.Length == 1)
                        {
                            return arr[0];
                        }
                        AddRange(t, arr);
                    }
                    else
                    {
                        t = new ArithmeticTag();
                        for (int i = 0; i < tags.Count; i++)
                        {
                            t.AddChild(tags[i]);
                        }
                    }
                    tags.Clear();
                    return t;
                }
            }
            return null;
        }

        private ITag[] Analysis(IList<ITag> tags, IList<Operator> opt)
        {
            var result = new List<ITag>();
            var temp = new List<ITag>();

            var isLogical = false;
            for (var i = 0; i < tags.Count; i++)
            {
                var o = tags[i] as OperatorTag;
                if (o != null)
                {
                    if (opt.Contains(o.Value))
                    {
                        result.Add(Analysis(temp, isLogical));
                        result.Add(tags[i]);
                        isLogical = false;
                        temp.Clear();
                    }
                    else
                    {
                        if (operators.Contains(o.Value))
                        {
                            isLogical = true;
                        }
                        temp.Add(tags[i]);
                    }
                    //result.Add(new Tem);
                }
                else
                {
                    temp.Add(tags[i]);
                }
            }

            if (temp.Count > 0)
            {
                result.Add(Analysis(temp, isLogical));
            }

            return result.ToArray();
        }

        private ITag Analysis(IList<ITag> tags, bool isLogical)
        {
            if (tags.Count > 1)
            {
                ITag t;
                if (isLogical)
                {
                    t = new LogicTag();
                    AddRange(t, Analysis(tags, operators));
                }
                else
                {
                    t = new ArithmeticTag();
                    AddRange(t, tags);
                }
                return t;
            }
            else
            {
                return tags[0];
            }
        }


        private void AddRange(ITag tag, IList<ITag> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                tag.AddChild(list[i]);
            }
        }

        #endregion
    }
}