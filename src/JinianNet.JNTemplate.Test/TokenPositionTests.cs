using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 词法分析阶段 token 位置（行号/列号）测试，
    /// 异常信息中的 [line,col] 来源于 token 的 BeginLine/BeginColumn
    /// </summary>
    public class TokenPositionTests
    {
        /// <summary>
        /// 使用与引擎一致的默认符号（$ 前缀、} 后缀）分析模板文本
        /// </summary>
        private static Token[] Lex(string text)
        {
            var lexer = new TemplateLexer(text, "$", "}", '$', false);
            return lexer.Execute();
        }

        /// <summary>
        /// 查找指定类型且文本包含关键字的第一个 token
        /// </summary>
        private static Token Find(Token[] tokens, TokenKind kind, string contains)
        {
            var token = tokens.FirstOrDefault(t => t.TokenKind == kind && t.Text != null && t.Text.IndexOf(contains, StringComparison.Ordinal) >= 0);
            Assert.NotNull(token);
            return token;
        }

        /// <summary>
        /// 测试跨行 token 的起始位置不被结束位置覆盖
        /// </summary>
        [Fact]
        public void TestMultiLineTokenPosition()
        {
            // 第 3 行的 bar 与第 2 行的 foo 属于同一个 TextData token：foo\nbar
            var tokens = Lex("a\nb ${foo\nbar}\nc");

            var tagStart = Find(tokens, TokenKind.TagStart, "$");
            Assert.Equal(2, tagStart.BeginLine);
            Assert.Equal(4, tagStart.BeginColumn);
            Assert.Equal(2, tagStart.EndLine);
            Assert.Equal(5, tagStart.EndColumn);

            var data = Find(tokens, TokenKind.TextData, "foo");
            Assert.Equal("foo\nbar", data.Text);
            Assert.Equal(2, data.BeginLine);//开始于第 2 行
            Assert.Equal(6, data.BeginColumn);
            Assert.Equal(3, data.EndLine);//结束于第 3 行
            Assert.Equal(5, data.EndColumn);
            Assert.True(data.EndLine > data.BeginLine, "跨行 token 的结束行号应大于开始行号");
        }

        /// <summary>
        /// 测试 EndLine 属性被正确赋值（此前始终为 0）
        /// </summary>
        [Fact]
        public void TestEndLineShouldBeAssigned()
        {
            var tokens = Lex("line1\r\nline2 ${\"x\ny\" + 1}\r\nlast");
            Assert.NotEmpty(tokens);
            foreach (var token in tokens)
            {
                Assert.True(token.EndLine > 0,
                    $"token[{token.Text}] 的 EndLine 未赋值");
                Assert.True(token.EndLine >= token.BeginLine,
                    $"token[{token.Text}] 的 EndLine({token.EndLine}) 不应小于 BeginLine({token.BeginLine})");
                Assert.True(token.BeginLine > 0,
                    $"token[{token.Text}] 的 BeginLine 未赋值");
            }

            //字符串字面量内部换行：x\ny 开始于第 2 行，结束于第 3 行
            var text = Find(tokens, TokenKind.Text, "line1");
            Assert.Equal(1, text.BeginLine);
            Assert.Equal(2, text.EndLine);

            var str = Find(tokens, TokenKind.String, "x");
            Assert.Equal(2, str.BeginLine);
            Assert.Equal(3, str.EndLine);
        }

        /// <summary>
        /// 测试编译引擎下异常报告的行号是标签起始位置而不是标签结束位置
        /// </summary>
        [Fact]
        public void TestCompileExceptionShouldReportBeginLine()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var template = engine.CreateTemplate("a\nb $if(foo\nbar)\nc\n$end\nd");
            var ex = Assert.ThrowsAny<TemplateException>(() => template.Render());

            //foo\nbar 这个 token 从第 2 行第 8 列开始，到第 3 行结束
            Assert.Equal(2, ex.Line);
            Assert.Equal(8, ex.Column);
            Assert.Contains("[line:2,col:8]", ex.Message);
        }

        /// <summary>
        /// 测试异常定位到出错标签所在的行（非首行、非末行）
        /// </summary>
        [Fact]
        public void TestParseExceptionShouldReportBeginLine()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            var template = engine.CreateTemplate("line1\r\nline2 $foo bar\r\nlast");
            var ex = Assert.ThrowsAny<TemplateException>(() => template.Render());

            Assert.Equal(2, ex.Line);
            Assert.Equal(8, ex.Column);
        }

        /// <summary>
        /// 测试位置信息不影响正常模板的渲染结果
        /// </summary>
        [Fact]
        public void TestRenderResultShouldNotBeAffected()
        {
            var engine = new EngineBuilder().UseCompileEngine().Build();
            Assert.Equal("18", engine.CreateTemplate("${8+2*5}").Render());
            Assert.Equal("2180", engine.CreateTemplate("${3000-1000--200-20}").Render());

            var interpretEngine = new EngineBuilder().UseInterpretationEngine().Build();
            Assert.Equal("18", interpretEngine.CreateTemplate("${8+2*5}").Render());
        }
    }
}
