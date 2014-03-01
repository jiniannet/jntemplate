/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Collections.Generic;
using JinianNet.JNTemplate.Parser.Node;
using JinianNet.JNTemplate.Parser;

namespace JinianNet.JNTemplate.Parser
{
    public class TemplateParser : IEnumerator<Tag>
    {
        const StringComparison stringComparer = StringComparison.OrdinalIgnoreCase;

        private Analyzers analyzer;

        private Token[] collection;
        private Int32 index;
        private Tag tag;

        public TemplateParser(Token[] tokens, Analyzers analyzer)
        {
            this.collection = tokens;
            this.analyzer = analyzer;
        }

        private Tag ReadTag()
        {
            Int32 line = this.collection[this.index].Line;
            Int32 col = this.collection[this.index].Column;
            List<Token> list = new List<Token>();
            while (this.index < this.collection.Length)
            {
                Token token = this.collection[this.index];
                this.index++;

                if (token.TokenKind == TokenKind.TagEnd)
                    break;
                if (token.TokenKind != TokenKind.Space && token.TokenKind != TokenKind.StringStart && token.TokenKind != TokenKind.StringEnd)
                {
                    list.Add(token);
                }
            }

            return Parse(list.ToArray(), line, col);
        }

        public Tag Parse(Token[] tokens)
        {
            if (tokens.Length > 0)
            {
                return Parse(tokens, tokens[0].Line, tokens[0].Column);
            }
            return null;
        }

        public Tag Parse(Token[] tokens, int line, int col)
        {
            return this.analyzer.Parse(this, tokens, line, col);
        }

        public Tag Current
        {
            get { return tag; }
        }

        public void Dispose()
        {

        }

        Object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            while (this.index < this.collection.Length)
            {
                Token token = this.collection[this.index];
                this.index++;
                switch (token.TokenKind)
                {
                    default:
                        this.tag = new ValueTag(ValueType.Text, token.Text, token.Line, token.Column);
                        return true;
                    case TokenKind.TagStart:
                        this.tag = ReadTag();
                        return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            this.index = 0;
        }

        #region


        public class ForEachAnalyzer : TagAnalyzer
        {

            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 5 && tokens[0].Text.Equals(Field.KEY_FOREACH, stringComparer) && tokens[1].Text.Equals("(", stringComparer) && tokens[3].Text.Equals(Field.KEY_FOREACH_IN, stringComparer) && tokens[tokens.Length - 1].Text.Equals(")", stringComparer))
                {
                    ForeachTag tag = new ForeachTag(line, col);
                    tag.Name = tokens[2].Text;
                    tag.Source = parser.Parse(CopyTo(tokens, 4, tokens.Length - 1 - 4), line, col);

                    while (true)
                    {
                        Tag next = parser.MoveNext() ? parser.Current : null;

                        if (next != null)
                        {
                            if (next.ElementType == ElementType.Object && Field.KEY_END.Equals(((ValueTag)next).Value.ToString(), stringComparer))
                            {
                                return tag;
                            }
                            else
                            {
                                tag.Value.Add(next);
                            }
                        }
                        else
                        {
                            throw new Exception(String.Concat("Foreach 未能找到结束标签! line ", tag.Line.ToString(), ", column ", tag.Column.ToString(), "."));
                        }
                    }
                }

                return null;
            }
        }

        public class IfAnalyzer : TagAnalyzer
        {

            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 3 && tokens[0].Text.Equals(Field.KEY_IF, stringComparer) && tokens[1].Text.Equals("(", stringComparer) && tokens[tokens.Length - 1].Text.Equals(")", stringComparer))
                {
                    IfTag tag = new IfTag(line, col);
                    tag.Test.Add(parser.Parse(CopyTo(tokens, 2, tokens.Length - 3), line, col));
                    TagCollection ec = new TagCollection();

                    while (true)
                    {
                        Tag next = parser.MoveNext() ? parser.Current : null;
                        if (next != null)
                        {
                            if (next.ElementType == ElementType.Object)
                            {
                                ValueTag vt = (ValueTag)next;

                                if (vt.Kind == ValueType.Mark)
                                {
                                    if (vt.Value.ToString().Equals(Field.KEY_END, stringComparer))
                                    {
                                        tag.Value.Add(ec);
                                        return tag;
                                    }
                                    else if (vt.Value.ToString().Equals(Field.KEY_ELSE, stringComparer))
                                    {
                                        tag.Value.Add(ec);
                                        ec = new TagCollection();
                                    }
                                    else
                                    {
                                        throw new Exception(vt.Value.ToString());
                                    }

                                    //tag.Test.Add((next as ElseIfTag).Test);
                                    //tag.Value.Add(ec);
                                    //ec = new TagCollection();
                                }
                                else
                                {
                                    ec.Add(next);
                                }
                            }
                            else
                            {
                                ec.Add(next);
                            }
                        }
                        else
                        {
                            throw new Exception(String.Concat("If 未能找到结束标签! line ", tag.Line.ToString(), ", column ", tag.Column.ToString(), "."));
                        }
                    }
                }

                return null;
            }

        }

        public class LoadAnalyzer : TagAnalyzer
        {
            private TemplateContext context;
            public LoadAnalyzer(TemplateContext ctx)
                : base()
            {
                this.context = ctx;
            }

            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 3 && tokens[0].Text.Equals(Field.KEY_LOAD, stringComparer) && tokens[1].Text.Equals("(", stringComparer) && tokens[tokens.Length - 1].Text.Equals(")", stringComparer))
                {
                    LoadTag tag = new LoadTag(line, col);
                    tag.Path = parser.Parse(CopyTo(tokens, 2, tokens.Length - 1 - 2), line, col);
                    return tag;
                }

                return null;
            }
        }

        public class SetAnalyzer : TagAnalyzer
        {
            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 5 && tokens[0].Text.Equals(Field.KEY_SET, stringComparer) && tokens[1].Text.Equals("(", stringComparer) && tokens[3].Text.Equals("=", stringComparer) && tokens[tokens.Length - 1].Text.Equals(")", stringComparer))
                {
                    SetTag tag = new SetTag(line, col);
                    tag.Name = tokens[2].Text;
                    tag.Value = parser.Parse(CopyTo(tokens, 4, tokens.Length - 5), line, col);
                    return tag;
                }

                return null;
            }
        }

        public class IncludeAnalyzer : TagAnalyzer
        {
            private TemplateContext context;
            public IncludeAnalyzer(TemplateContext ctx)
                : base()
            {
                this.context = ctx;
            }
            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 3 && tokens[0].Text.Equals(Field.KEY_INCLUDE, stringComparer) && tokens[1].Text.Equals("(", stringComparer) && tokens[tokens.Length - 1].Text.Equals(")", stringComparer))
                {
                    IncludeTag tag = new IncludeTag(line, col);
                    tag.Path = parser.Parse(CopyTo(tokens, 2, tokens.Length - 1 - 2), line, col);
                    return tag;
                }

                return null;
            }
        }

        public class FunctionAnalyzer : TagAnalyzer
        {

            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length > 2 && tokens[1].TokenKind == TokenKind.LeftParentheses && tokens[tokens.Length - 1].TokenKind == TokenKind.RightParentheses)
                {
                    FunctaionTag tag = new FunctaionTag(tokens[0].Text, line, col);
                    List<Token> list = new List<Token>();
                    Int32 pos = 0;
                    for (Int32 i = 2; i < tokens.Length - 1; i++)
                    {
                        if (tokens[i].TokenKind == TokenKind.Comma && pos == 0)
                        {
                            tag.Args.Add(parser.Parse(list.ToArray(), list[0].Line, list[0].Column));
                            list = new List<Token>();
                        }
                        else
                        {
                            if (tokens[i].TokenKind == TokenKind.LeftParentheses)
                                pos++;
                            else if (tokens[i].TokenKind == TokenKind.RightParentheses && pos > 0)
                                pos--;
                            list.Add(tokens[i]);
                        }
                    }

                    if (list.Count > 0)
                    {
                        tag.Args.Add(parser.Parse(list.ToArray(), list[0].Line, list[0].Column));
                    }

                    return tag;
                }

                return null;
            }
        }

        public class VariableAnalyzer : TagAnalyzer
        {
            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                if (tokens.Length == 1)
                    switch (tokens[0].TokenKind)
                    {
                        case TokenKind.TextData:
                            if (tokens[0].Text.Equals("true", stringComparer) || tokens[0].Text.Equals("false", stringComparer))
                            {
                                return new ValueTag(ValueType.Boolen, tokens[0].Text.Equals("true", stringComparer), line, col);
                            }
                            else if (tokens[0].Text.Equals(Field.KEY_END, stringComparer))
                            {
                                return new ValueTag(ValueType.Mark, Field.KEY_END, line, col);
                            }
                            else if (tokens[0].Text.Equals(Field.KEY_ELSE, stringComparer))
                            {
                                return new ValueTag(ValueType.Mark, Field.KEY_ELSE, line, col);
                            }
                            return new VariableTag(tokens[0].Text, line, col);
                        case TokenKind.Number:
                            if (tokens[0].Text.IndexOf('.') < 0)
                                return new ValueTag(ValueType.Integer, Int32.Parse(tokens[0].Text), line, col);
                            else
                                return new ValueTag(ValueType.Decimal, Single.Parse(tokens[0].Text), line, col);
                        case TokenKind.Operator:
                        case TokenKind.RightParentheses:
                        case TokenKind.LeftParentheses:
                            return new ValueTag(ValueType.Punctuation, tokens[0].Text, line, col);
                        case TokenKind.String:
                            return new ValueTag(ValueType.Text, tokens[0].Text, line, col);
                        default:
                            throw new Exception(String.Concat(tokens[0].Text, "at line ", tokens[0].Line.ToString(), ", column ", tokens[0].Column.ToString(), "."));
                    }

                return null;
            }
        }

        public class ExpressionAnalyzer : TagAnalyzer
        {
            public override Tag Parse(TemplateParser parser, Token[] tokens, int line, int col)
            {
                ExpressionTag tag = new ExpressionTag(line, col);
                /* 注意事项：避免将FunctionTag拆分成表达式
                 * 遇到左括号，如果是开头或上一个token是标点或操作符号，则为表达式
                 * 否则为方法
                 */
                List<Token> array = new List<Token>();
                Stack<TokenKind> pos = new Stack<TokenKind>();
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i].TokenKind == TokenKind.LeftParentheses && ((pos.Count == 0 && i > 0 && tokens[i - 1].TokenKind == TokenKind.TextData) || pos.Count>0))
                    {
                        pos.Push(TokenKind.LeftParentheses);
                    }
                    else if (pos.Count > 0 && tokens[i].TokenKind == TokenKind.RightParentheses)
                    {
                        pos.Pop();
                    }
                  
                    array.Add(tokens[i]);
                    if (tokens[i].TokenKind == TokenKind.TextData && i < tokens.Length - 1 && tokens[i + 1].TokenKind == TokenKind.LeftParentheses)
                    {
                        continue;
                    }
                    if (pos.Count == 0)
                    {
                        tag.Value.Add(parser.Parse(array.ToArray(), line, col));
                        array = new List<Token>();
                    }
                }
                return tag;
            }
        }
        #endregion

    }
}