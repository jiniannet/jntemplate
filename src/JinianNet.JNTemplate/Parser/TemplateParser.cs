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
using JinianNet.JNTemplate.Context;

namespace JinianNet.JNTemplate.Parser
{
    public class TemplateParser : IEnumerator<Tag>
    {
        const StringComparison stringComparer = StringComparison.OrdinalIgnoreCase;

        #region private field
        private Tag tag;//当前标签
        private Token[] tokens;//tokens列表
        private Int32 index;//当前索引
        private List<ITagParser> parsers;
        #endregion

        #region
        public List<ITagParser> Parser
        {
            get { return this.parsers; }
        }
        #endregion

        #region ctox
        public TemplateParser(Token[] ts)
        {
            parsers = new List<ITagParser>();
            //parsers.Add(new TestParser());
            this.tokens = ts;
            Reset();
        }
        #endregion

        #region IEnumerator<Tag> 成员

        public Tag Current
        {
            get
            {
                return tag;
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {

        }

        #endregion

        #region IEnumerator 成员


        public bool MoveNext()
        {
            if (this.index < this.tokens.Length)
            {
                Tag t = Read();
                if (t != null)
                {
                    this.tag = t;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            this.index = 0;
            this.tag = null;
        }

        private Tag Read()
        {
            Tag t = null;
            if (IsTagStart())
            {
                Token t1, t2;
                t1 = t2 = GetToken();
                TokenCollection tc = new TokenCollection();

                do
                {
                    this.index++;
                    t2.Next = GetToken();
                    t2 = t2.Next;

                    tc.Add(t2);


                } while (!IsTagEnd());

                tc.Remove(tc.Last);
                this.index++;
                t = Read(tc);
                if (t != null)
                {
                    t.FirstToken = t1;
                    if (t.Children.Count == 0 || t.LastToken==null || t2.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = t2;
                    }
                }
            }
            else
            {
                t = new TextTag();
                t.FirstToken = GetToken();
                t.LastToken = null;
                this.index++;
            }
            return t;
        }

        public Tag Read(TokenCollection tc)
        {
            if (tc == null || tc.Count == 0)
                return null;
            Tag t = null;
            for (Int32 i = 0; i < this.parsers.Count; i++)
            {
                t = this.parsers[i].Parse(this, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;


                    if (t.Children.Count == 0 || tc.Last.CompareTo(t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    break;
                }
            }
            return t;
        }


        private Boolean IsTagEnd()
        {
            return IsTagEnd(GetToken());
        }

        private Boolean IsTagStart()
        {
            return IsTagStart(GetToken());
        }

        private Boolean IsTagEnd(Token t)
        {
            return t == null || t.TokenKind == TokenKind.TagEnd || t.TokenKind == TokenKind.EOF;
        }

        private Boolean IsTagStart(Token t)
        {
            return t.TokenKind == TokenKind.TagStart;
        }

        private Token GetToken()
        {
            return tokens[this.index];
        }

        private Token GetToken(Int32 i)
        {
            return tokens[this.index + 1];
        }

        #endregion

        #region 1.1
#if V1
        private Analyzers analyzer;
        private TemplateLexer lexer;

        private Tag tag;
        private Token token;
        private Token[] tokens;
        private Int32 index;


        public TemplateParser(TemplateLexer lexer, Analyzers analyzer)
        {
            this.lexer = lexer;
            this.tokens = lexer.Parse();
            this.analyzer = analyzer;
        }

        public Tag Current
        {
            get { return tag; }
        }

        public void Dispose()
        {

        }

        public Tag ReadTag(Token token)
        {
            Tag t;
            if (token.TokenKind == TokenKind.TagStart)
            {
                t = this.analyzer.Parse(this, token);
            }
            else
            {
                t = new TextTag();
                t.FirstToken = token;
                t.LastToken = null;
            }
            return t;
        }

        Object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (token == null)
            {
                return false;
            }
            Tag t = ReadTag(this.token);

            if (t != null)
            {
                this.tag = t;
                if (this.tag.LastToken != null)
                    this.token = this.tag.LastToken.Next;
                else
                    this.token = this.tag.FirstToken.Next;
                return true;
            }
            else
            {
                this.token = null;
                return false;
            }


        }

        public void Reset()
        {
            //this.index = 0;
        }

        #region


        public class ForEachAnalyzer : TagAnalyzer
        {
            /*
             * FOREACH ( TEXEDATE IN 
             * 
             * END
             */
            public override Tag Parse(TemplateParser parser, Token token)
            {
                Token t = token;
                if (t.Equals(Field.KEY_FOREACH, stringComparer))
                {
                    ForeachTag tag = new ForeachTag();
                    tag.FirstToken = token;
                    
                    while((t = t.Next).TokenKind == TokenKind.Space || t.TokenKind == TokenKind.LeftBracket)
                    {

                    }

                    if(t.TokenKind != TokenKind.TextData)
                    {
                        throw new Exception("analyzer error……");
                    }

                    tag.Name = t.Text;
                    tag.Source = parser.ReadTag(t.Next.Next);
                    t = (tag.Source.LastToken ?? tag.Source.FirstToken).Next;

                    while ((t = t.Next).TokenKind != TokenKind.EOF && t.TokenKind != TokenKind.TagEnd) 
                    {
                        if (t.TokenKind != TokenKind.Space && t.TokenKind != TokenKind.RightParentheses)
                        {
                            throw new Exception("analyzer error……");
                        }
                    }


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

            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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

            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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
            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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
            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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

            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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
            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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
            public override Tag Parse(TemplateParser parser, Token[] tokens, Int32 line, Int32 col)
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
                    if (tokens[i].TokenKind == TokenKind.LeftParentheses && ((pos.Count == 0 && i > 0 && tokens[i - 1].TokenKind == TokenKind.TextData) || pos.Count > 0))
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

        /// <summary>
        /// 避免处理难度，不支持多余的无意义括号
        /// 比如：(((User.Name))) 应直写做 User.Name
        /// </summary>
        public class Reference : TagAnalyzer
        {
            public override Tag Parse(TemplateParser parser, Token token)
            {
                if (token.TokenKind == TokenKind.TextData && (
                    token.Text.Equals(Field.KEY_ELSE, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_ELSEIF, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_END, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOR, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOREACH, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOREACH_IN, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_IF, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_INCLUDE, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_LOAD, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_SET, StringComparison.OrdinalIgnoreCase)
                    ))
                {
                    WordTag tag = new WordTag();
                    tag.FirstToken = token;
                    tag.LastToken = null;
                    return tag;
                }
                return null;
            }
        }

        public class WordAnalyzer : TagAnalyzer
        {
            public override Tag Parse(TemplateParser parser, Token token)
            {
                if (token.TokenKind == TokenKind.TextData && (
                    token.Text.Equals(Field.KEY_ELSE, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_ELSEIF, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_END, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOR, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOREACH, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_FOREACH_IN, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_IF, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_INCLUDE, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_LOAD, StringComparison.OrdinalIgnoreCase)
                    || token.Text.Equals(Field.KEY_SET, StringComparison.OrdinalIgnoreCase)
                    ))
                {
                    WordTag tag = new WordTag();
                    tag.FirstToken = token;
                    tag.LastToken = null;
                    return tag;
                }
                return null;
            }
           
        }
        #endregion
#endif
        #endregion


        #region IEnumerator 成员

        Object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        #endregion
    }
}