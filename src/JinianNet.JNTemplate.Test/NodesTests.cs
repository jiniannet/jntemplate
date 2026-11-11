using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="Token"/> 测试
    /// </summary>
    public class TokenTests
    {
        /// <summary>
        /// 基本属性与 ToString
        /// </summary>
        [Fact]
        public void TestTokenProperties()
        {
            var token = new Token(TokenKind.TextData, "hello")
            {
                BeginLine = 1,
                BeginColumn = 2,
                EndLine = 3,
                EndColumn = 4
            };

            Assert.Equal("hello", token.Text);
            Assert.Equal(TokenKind.TextData, token.TokenKind);
            Assert.Equal(1, token.BeginLine);
            Assert.Equal(2, token.BeginColumn);
            Assert.Equal(3, token.EndLine);
            Assert.Equal(4, token.EndColumn);
            Assert.Equal("hello", token.ToString());
            Assert.Null(token.Next);

            var next = new Token(TokenKind.TagStart, "$");
            token.Next = next;
            Assert.Same(next, token.Next);
        }

        /// <summary>
        /// 相等性与比较
        /// </summary>
        [Fact]
        public void TestTokenEquals()
        {
            var a = new Token(TokenKind.TextData, "a") { BeginLine = 1, BeginColumn = 1 };
            var b = new Token(TokenKind.TextData, "a") { BeginLine = 1, BeginColumn = 1 };
            var c = new Token(TokenKind.TextData, "a") { BeginLine = 2, BeginColumn = 1 };
            var d = new Token(TokenKind.TagStart, "a") { BeginLine = 1, BeginColumn = 1 };
            var e = new Token(TokenKind.TextData, "a") { BeginLine = 1, BeginColumn = 5 };

            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.False(a.Equals((object)null!));
            Assert.False(a.Equals((object)"a"));
            Assert.False(a.Equals(c));
            Assert.False(a.Equals(d));
            Assert.False(a.Equals(e));

            Assert.Equal(1, c.CompareTo(a));
            Assert.Equal(-1, a.CompareTo(c));
            Assert.Equal(1, e.CompareTo(a));
            Assert.Equal(-1, a.CompareTo(e));
            Assert.Equal(0, a.CompareTo(b));

            Assert.True(a == b);
            Assert.False(a != b);
            Assert.True((Token)null! == (Token)null!);
            Assert.False(a == (Token)null!);
            Assert.False((Token)null! == a);
            Assert.True(a != (Token)null!);

            Assert.True(c > a);
            Assert.True(a < c);
            Assert.True(a >= b);
            Assert.True(a <= b);
            Assert.False(a > null!);
            Assert.False(a < null!);
            Assert.True((Token)null! >= null!);
            Assert.True((Token)null! <= null!);
        }

        /// <summary>
        /// 哈希与比较器接口
        /// </summary>
        [Fact]
        public void TestTokenHashCode()
        {
            var a = new Token(TokenKind.TextData, "a");
            Assert.Equal("a".GetHashCode(), a.GetHashCode());
            Assert.Equal(0, new Token(TokenKind.TextData, null!).GetHashCode());

            var comparer = (System.Collections.Generic.IEqualityComparer<Token>)a;
            Assert.Equal(a.GetHashCode(), comparer.GetHashCode(a));
            Assert.True(comparer.Equals(a, new Token(TokenKind.TextData, "a")));
            Assert.False(comparer.Equals(a, null!));
            Assert.True(comparer.Equals(null!, null!));
        }
    }

    /// <summary>
    /// <see cref="TokenCollection"/> 测试
    /// </summary>
    public class TokenCollectionTests
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        [Fact]
        public void TestConstructors()
        {
            var empty = new TokenCollection();
            Assert.Empty(empty);
            Assert.False(empty.IsReadOnly);

            var withCapacity = new TokenCollection(10);
            Assert.Empty(withCapacity);

            var fromEnumerable = new TokenCollection(new[]
            {
                new Token(TokenKind.TextData, "a"),
                new Token(TokenKind.TextData, "b"),
                new Token(TokenKind.TextData, "c")
            });
            Assert.Equal(3, fromEnumerable.Count);
            Assert.Equal("a", fromEnumerable.First.Text);
            Assert.Equal("c", fromEnumerable.Last.Text);

            var range = new TokenCollection(fromEnumerable, 1, 2);
            Assert.Equal(2, range.Count);
            Assert.Equal("b", range[0].Text);
            Assert.Equal("c", range[1].Text);

            // 越界的结束位置不会抛异常
            var beyond = new TokenCollection(fromEnumerable, 0, 10);
            Assert.Equal(3, beyond.Count);
        }

        /// <summary>
        /// 空白 token 不会被加入集合
        /// </summary>
        [Fact]
        public void TestIgnoreSpaceToken()
        {
            var tc = new TokenCollection();
            tc.Add(new Token(TokenKind.TextData, "a"));
            tc.Add(new Token(TokenKind.Space, " "));
            Assert.Single(tc);

            tc.Insert(0, new Token(TokenKind.Space, " "));
            Assert.Single(tc);

            tc[0] = new Token(TokenKind.Space, " ");
            Assert.Equal("a", tc[0].Text);

            tc[0] = new Token(TokenKind.TextData, "b");
            Assert.Equal("b", tc[0].Text);
        }

        /// <summary>
        /// 索引器支持负数下标与区间取值
        /// </summary>
        [Fact]
        public void TestIndexer()
        {
            var tc = new TokenCollection(new[]
            {
                new Token(TokenKind.TextData, "a"),
                new Token(TokenKind.TextData, "b"),
                new Token(TokenKind.TextData, "c")
            });

            Assert.Equal("c", tc[-1].Text);
            Assert.Equal("b", tc[-2].Text);

            var range = tc[1, 3];
            Assert.Equal(2, range.Count);
            Assert.Equal("b", range[0].Text);
            Assert.Equal("c", range[1].Text);

            var negativeRange = tc[-2, -1];
            Assert.Single(negativeRange);

            var emptyRange = tc[2, 2];
            Assert.Empty(emptyRange);
        }

        /// <summary>
        /// 集合操作与缓存文本
        /// </summary>
        [Fact]
        public void TestCollectionOperations()
        {
            var a = new Token(TokenKind.TextData, "a");
            var b = new Token(TokenKind.TextData, "b");
            var tc = new TokenCollection(new[] { a, b });

            Assert.Equal("ab", tc.ToString());
            Assert.Equal(tc.ToString().GetHashCode(), tc.GetHashCode());

            Assert.True(tc.Any(TokenKind.TextData));
            Assert.False(tc.Any(TokenKind.TagStart));
            Assert.Equal(0, tc.IndexOf(a));
            Assert.Contains(b, tc);

            var array = new Token[2];
            tc.CopyTo(array, 0);
            Assert.Same(a, array[0]);
            Assert.Same(b, array[1]);

            Assert.True(tc.Remove(a));
            Assert.False(tc.Remove(a));
            Assert.Single(tc);

            tc.RemoveAt(0);
            Assert.Empty(tc);

            tc.Add(a);
            tc.Clear();
            Assert.Empty(tc);
            Assert.Equal(string.Empty, tc.ToString());
        }

        /// <summary>
        /// 相等性
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var tc1 = new TokenCollection(new[] { new Token(TokenKind.TextData, "a") });
            var tc2 = new TokenCollection(new[] { new Token(TokenKind.TextData, "a") });
            var tc3 = new TokenCollection(new[] { new Token(TokenKind.TextData, "b") });
            var tc4 = new TokenCollection(new[]
            {
                new Token(TokenKind.TextData, "a"),
                new Token(TokenKind.TextData, "b")
            });

            Assert.True(tc1.Equals(tc2));
            Assert.True(tc1.Equals((object)tc2));
            Assert.False(tc1.Equals(tc3));
            Assert.False(tc1.Equals(tc4));
            Assert.False(tc1.Equals((object)null!));
            Assert.False(tc1.Equals((object)"a"));

            var comparer = (System.Collections.Generic.IEqualityComparer<TokenCollection>)tc1;
            Assert.True(comparer.Equals(tc1, tc2));
            Assert.False(comparer.Equals(tc1, tc3));
            Assert.True(comparer.Equals(null!, null!));
            Assert.False(comparer.Equals(tc1, null!));
            Assert.Equal(tc1.GetHashCode(), comparer.GetHashCode(tc1));

            Assert.NotEmpty(tc1);
            Assert.Single(tc1);
        }
    }

    /// <summary>
    /// <see cref="TagCollection"/> 测试
    /// </summary>
    public class TagCollectionTests
    {
        /// <summary>
        /// 添加标签会维护前后标记
        /// </summary>
        [Fact]
        public void TestAdd()
        {
            var collection = new TagCollection();
            Assert.Empty(collection);
            Assert.False(collection.IsReadOnly);

            collection.Add(null!);
            Assert.Empty(collection);

            var t1 = new TextTag();
            var t2 = new TextTag();
            collection.Add(t1);
            collection.Add(t2);

            Assert.Equal(2, collection.Count);
            Assert.True(t1.Next);
            Assert.True(t2.Previous);
        }

        /// <summary>
        /// 索引器赋值会重建链式关系
        /// </summary>
        [Fact]
        public void TestIndexerSet()
        {
            var collection = new TagCollection();
            var t1 = new TextTag();
            var t2 = new TextTag();
            var t3 = new TextTag();
            collection.Add(t1);
            collection.Add(t2);
            collection.Add(t3);

            var replaced = new TextTag();
            collection[1] = replaced;
            Assert.Same(replaced, collection[1]);

            var last = new TextTag();
            collection[2] = last;
            Assert.Same(last, collection[2]);
        }

        /// <summary>
        /// 删除与插入
        /// </summary>
        [Fact]
        public void TestRemoveAndInsert()
        {
            var collection = new TagCollection();
            var t1 = new TextTag();
            var t2 = new TextTag();
            var t3 = new TextTag();
            collection.Add(t1);
            collection.Add(t2);
            collection.Add(t3);

            Assert.Equal(1, collection.IndexOf(t2));
            Assert.Contains(t2, collection);
            Assert.True(collection.Remove(t2));
            Assert.False(collection.Remove(t2));
            Assert.Equal(2, collection.Count);

            collection.Insert(0, new TextTag());
            Assert.Equal(3, collection.Count);

            collection.RemoveAt(0);
            Assert.Equal(2, collection.Count);
            collection.RemoveAt(1);
            Assert.Single(collection);

            Assert.NotNull(collection.ToArray());
            Assert.Single(collection.ToArray());

            collection.Clear();
            Assert.Empty(collection);
        }
    }

    /// <summary>
    /// 标签扩展方法测试（<see cref="TagExtensions"/>）
    /// </summary>
    public class TagExtensionsTests
    {
        /// <summary>
        /// ToSource
        /// </summary>
        [Fact]
        public void TestToSource()
        {
            Assert.Null(TagExtensions.ToSource(null!));

            // 没有 token 时返回类型名
            Assert.Equal(nameof(TextTag), new TextTag().ToSource());

            // 只有首 token
            var single = new TextTag();
            single.FirstToken = new Token(TokenKind.TextData, "abc");
            Assert.Equal("abc", single.ToSource());

            // 首尾 token 之间按 Next 链拼接
            var start = new Token(TokenKind.TagStart, "$");
            var name = new Token(TokenKind.TextData, "name");
            start.Next = name;
            name.Next = new Token(TokenKind.TagEnd, "}");

            var tag = new TextTag();
            tag.FirstToken = start;
            tag.LastToken = name;
            Assert.Equal("$name", tag.ToSource());

            // Next 链中断时不会死循环
            var broken = new TextTag();
            broken.FirstToken = new Token(TokenKind.TagStart, "$");
            broken.LastToken = new Token(TokenKind.TagEnd, "}");
            Assert.Equal("$", broken.ToSource());
        }

        /// <summary>
        /// 文本标签在不同输出模式下的处理
        /// </summary>
        [Fact]
        public void TestTextTagToString()
        {
            // 未设置 token 时文本为空字符串（会被缓存）
            Assert.Equal(string.Empty, new TextTag().Text);
            Assert.Null(TagExtensions.ToString(new TextTag(), OutMode.None));
            Assert.Null(TagExtensions.ToString(null!, OutMode.None));

            var tag = new TextTag();
            tag.FirstToken = new Token(TokenKind.TextData, "  hello  ");
            Assert.Equal("  hello  ", tag.OriginalText);
            Assert.Equal("  hello  ", tag.Text);
            Assert.Equal("  hello  ", tag.ToString());
            Assert.Equal("  hello  ", TagExtensions.ToString(tag, OutMode.None));
            Assert.Equal("hello", TagExtensions.ToString(tag, OutMode.StripWhiteSpace));

            // Auto 模式：首标签会去掉行首换行
            var first = new TextTag();
            first.FirstToken = new Token(TokenKind.TextData, "\r\nhello");
            Assert.Equal("hello", TagExtensions.ToString(first, OutMode.Auto));

            var firstLf = new TextTag();
            firstLf.FirstToken = new Token(TokenKind.TextData, "\nhello");
            Assert.Equal("hello", TagExtensions.ToString(firstLf, OutMode.Auto));

            // 纯换行文本在 Auto 模式下被丢弃
            var onlyNewLine = new TextTag();
            onlyNewLine.FirstToken = new Token(TokenKind.TextData, "\n");
            Assert.Equal(string.Empty, TagExtensions.ToString(onlyNewLine, OutMode.Auto));

            // 非首标签保留换行
            var notFirst = new TextTag { Previous = true };
            notFirst.FirstToken = new Token(TokenKind.TextData, "\nhello");
            Assert.Equal("\nhello", TagExtensions.ToString(notFirst, OutMode.Auto));

            // 尾标签去掉结尾多余的空格（保留换行）
            var last = new TextTag { Next = false };
            last.FirstToken = new Token(TokenKind.TextData, "hello \n ");
            Assert.Equal("hello \n", TagExtensions.ToString(last, OutMode.Auto));

            var last2 = new TextTag { Next = false };
            last2.FirstToken = new Token(TokenKind.TextData, "hello ");
            Assert.Equal("hello ", TagExtensions.ToString(last2, OutMode.Auto));
        }

        /// <summary>
        /// 特殊标签
        /// </summary>
        [Fact]
        public void TestSpecialTags()
        {
            Assert.True(new NullTag().IsPrimitive);
            Assert.True(new NullTag().IsSimple);
            Assert.Equal(string.Empty, new NullTag().ToString());

            Assert.False(new EndTag().Out);
            Assert.True(new EndTag().IsPrimitive);

            var comment = new CommentTag();
            comment.FirstToken = new Token(TokenKind.Comment, "# c");
            Assert.False(comment.Out);
            Assert.Equal("# c", comment.ToString());

            var json = new JsonTag();
            Assert.Empty(json.Dict);
            Assert.True(json.IsSimple);

            // TypeTag 的显式接口实现
            var number = new NumberTag { Value = 5 };
            var typeTag = (ITypeTag)number;
            Assert.Equal(5, typeTag.Value);
            typeTag.Value = 6;
            Assert.Equal(6, number.Value);
        }
    }
}
