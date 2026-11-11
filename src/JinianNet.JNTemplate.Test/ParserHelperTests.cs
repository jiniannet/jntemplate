using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="CharScanner"/> 测试
    /// </summary>
    public class CharScannerTests
    {
        /// <summary>
        /// 索引与前后移动
        /// </summary>
        [Fact]
        public void TestIndexAndMove()
        {
            var scanner = new CharScanner("abc");
            Assert.Equal(0, scanner.Index);
            Assert.False(scanner.IsEnd());

            Assert.True(scanner.Next());
            Assert.Equal(1, scanner.Index);
            Assert.True(scanner.Next(2));
            Assert.Equal(3, scanner.Index);
            Assert.True(scanner.IsEnd());

            // 越界移动返回 false，且不改变下标
            Assert.False(scanner.Next());
            Assert.Equal(3, scanner.Index);

            Assert.True(scanner.Back());
            Assert.Equal(2, scanner.Index);
            Assert.True(scanner.Back(2));
            Assert.Equal(0, scanner.Index);
            Assert.False(scanner.Back());
            Assert.Equal(0, scanner.Index);

            // null 文本按空字符串处理
            var empty = new CharScanner(null!);
            Assert.True(empty.IsEnd());
            Assert.Equal('\0', empty.Read());
        }

        /// <summary>
        /// 读取字符
        /// </summary>
        [Fact]
        public void TestRead()
        {
            var scanner = new CharScanner("abc");
            Assert.Equal('a', scanner.Read());
            Assert.Equal('c', scanner.Read(2));
            Assert.Equal('\0', scanner.Read(3));
            Assert.Equal('\0', scanner.Read(10));

            scanner.Next();
            Assert.Equal('b', scanner.Read());
        }

        /// <summary>
        /// 字符匹配
        /// </summary>
        [Fact]
        public void TestIsMatch()
        {
            var scanner = new CharScanner("hello");
            Assert.True(scanner.IsMatch(new[] { 'h', 'e' }));
            Assert.False(scanner.IsMatch(new[] { 'h', 'a' }));
            // 超出范围
            Assert.False(scanner.IsMatch(new[] { 'h', 'e', 'l', 'l', 'o', '!' }));

            scanner.Next(2);
            Assert.True(scanner.IsMatch(new[] { 'l' }, 0));
            Assert.True(scanner.IsMatch(new[] { 'h' }, -2));
            Assert.False(scanner.IsMatch(new[] { 'x' }, 2));
        }

        /// <summary>
        /// 字符串截取
        /// </summary>
        [Fact]
        public void TestGetString()
        {
            var scanner = new CharScanner("hello");
            scanner.Next(2);
            Assert.Equal("he", scanner.GetString());
            // 截取后起点后移
            scanner.Next(3);
            Assert.Equal("llo", scanner.GetString());
            Assert.Equal("ll", scanner.GetString(2, 4));

            Assert.Null(scanner.GetString(2, 1));
            Assert.Null(scanner.GetString());
        }

        /// <summary>
        /// 转义字符串
        /// </summary>
        [Fact]
        public void TestGetEscapeString()
        {
            Assert.Equal("\0", new CharScanner(@"\0").GetEscapeString(0, 2));
            Assert.Equal("\"", new CharScanner("\\\"").GetEscapeString(0, 2));
            Assert.Equal("\\", new CharScanner(@"\\").GetEscapeString(0, 2));
            Assert.Equal("\a", new CharScanner(@"\a").GetEscapeString(0, 2));
            Assert.Equal("\b", new CharScanner(@"\b").GetEscapeString(0, 2));
            Assert.Equal("\f", new CharScanner(@"\f").GetEscapeString(0, 2));
            Assert.Equal("\n", new CharScanner(@"\n").GetEscapeString(0, 2));
            Assert.Equal("\r", new CharScanner(@"\r").GetEscapeString(0, 2));
            Assert.Equal("\t", new CharScanner(@"\t").GetEscapeString(0, 2));
            Assert.Equal("\v", new CharScanner(@"\v").GetEscapeString(0, 2));
            // 未知转义保留反斜杠与原字符
            Assert.Equal(@"\z", new CharScanner(@"\z").GetEscapeString(0, 2));

            var scanner = new CharScanner(@"a\nb");
            scanner.Next(4);
            Assert.Equal("a\nb", scanner.GetEscapeString());

            // 空区间返回 null
            Assert.Null(new CharScanner("abc").GetEscapeString(1, 1));
        }
    }

    /// <summary>
    /// <see cref="TokenCollectionExtensions"/> 测试
    /// </summary>
    public class TokenCollectionExtensionsTests
    {
        private static TokenCollection Build(params TokenKind[] kinds)
        {
            var tokens = new Token[kinds.Length];
            for (var i = 0; i < kinds.Length; i++)
            {
                tokens[i] = new Token(kinds[i], kinds[i].ToString());
            }
            return new TokenCollection(tokens);
        }

        /// <summary>
        /// 判断整个集合是否被括号包裹
        /// </summary>
        [Fact]
        public void TestInsideParentheses()
        {
            Assert.False(TokenCollectionExtensions.InsideParentheses(null!, 0, 0));
            Assert.False(Build(TokenKind.LeftParentheses, TokenKind.RightParentheses).InsideParentheses(0, 2));
            Assert.False(Build(TokenKind.TextData, TokenKind.TextData, TokenKind.TextData).InsideParentheses(0, 3));
            Assert.True(Build(TokenKind.LeftParentheses, TokenKind.TextData, TokenKind.RightParentheses).InsideParentheses(0, 3));

            // 括号不完整
            Assert.False(Build(TokenKind.LeftParentheses, TokenKind.LeftParentheses, TokenKind.RightParentheses).InsideParentheses(0, 3));
            // 括号提前闭合
            Assert.False(Build(
                TokenKind.LeftParentheses,
                TokenKind.RightParentheses,
                TokenKind.TextData,
                TokenKind.RightParentheses).InsideParentheses(0, 4));
        }

        /// <summary>
        /// 去除外层括号
        /// </summary>
        [Fact]
        public void TestTrimParentheses()
        {
            Assert.Null(TokenCollectionExtensions.TrimParentheses(null!));

            var noParentheses = Build(TokenKind.TextData, TokenKind.TextData, TokenKind.TextData);
            Assert.Equal(3, noParentheses.TrimParentheses().Count);

            var once = Build(TokenKind.LeftParentheses, TokenKind.TextData, TokenKind.RightParentheses);
            var trimmed = once.TrimParentheses();
            Assert.Single(trimmed);
            Assert.Equal(TokenKind.TextData, trimmed[0].TokenKind);

            var twice = Build(
                TokenKind.LeftParentheses,
                TokenKind.LeftParentheses,
                TokenKind.TextData,
                TokenKind.RightParentheses,
                TokenKind.RightParentheses);
            Assert.Single(twice.TrimParentheses());
        }

        /// <summary>
        /// 按分隔符拆分
        /// </summary>
        [Fact]
        public void TestSplit()
        {
            var coll = Build(TokenKind.TextData, TokenKind.Comma, TokenKind.TextData);
            var result = coll.Split(TokenKind.Comma);
            Assert.Equal(3, result.Length);
            Assert.Equal(TokenKind.TextData, result[0][0].TokenKind);
            Assert.Equal(TokenKind.Comma, result[1][0].TokenKind);
            Assert.Equal(TokenKind.TextData, result[2][0].TokenKind);

            // 空 kinds 时只有一个分组
            var single = coll.Split(Array.Empty<TokenKind>());
            Assert.Single(single);

            // 结束位置超过集合长度时自动截断
            var rangeSplit = coll.Split(0, 100, TokenKind.Comma);
            Assert.Equal(3, rangeSplit.Length);

            // 带括号的内容不会被拆分
            var withParentheses = Build(
                TokenKind.LeftParentheses,
                TokenKind.TextData,
                TokenKind.Comma,
                TokenKind.RightParentheses);
            Assert.Single(withParentheses.Split(TokenKind.Comma));
        }

        /// <summary>
        /// 括号不匹配时抛出解析异常
        /// </summary>
        [Fact]
        public void TestSplitSyntaxError()
        {
            var coll = Build(TokenKind.TextData, TokenKind.RightParentheses);
            Assert.Throws<ParseException>(() => coll.Split(TokenKind.Comma));

            var bracket = Build(TokenKind.LeftBracket, TokenKind.TextData, TokenKind.RightParentheses);
            Assert.Throws<ParseException>(() => bracket.Split(TokenKind.Comma));
        }
    }
}
