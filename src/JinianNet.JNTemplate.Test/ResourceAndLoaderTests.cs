using System.Text;
using JinianNet.JNTemplate.Resources;
using StringReader = JinianNet.JNTemplate.Resources.StringReader;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 资源读取器与加载器测试
    /// </summary>
    public class ResourceAndLoaderTests : IDisposable
    {
        private readonly string workDir;
        private readonly string workFile;

        public ResourceAndLoaderTests()
        {
            workDir = Path.Combine(Path.GetTempPath(), "jntemplate-res-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workDir);
            workFile = Path.Combine(workDir, "demo.txt");
            File.WriteAllText(workFile, "demo content", Encoding.UTF8);
        }

        public void Dispose()
        {
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }
        }

        private static TemplateContext CreateContext()
        {
            return new EngineBuilder().Build().CreateContext();
        }

        /// <summary>
        /// 字符串读取器
        /// </summary>
        [Fact]
        public async Task TestStringReader()
        {
            var reader = new StringReader("content");
            var ctx = CreateContext();

            Assert.Equal("content", reader.ReadToEnd(ctx));
            Assert.Equal("content", await reader.ReadToEndAsync(ctx));
            Assert.Equal("content".GetHashCode(), reader.GetHashCode());

            Assert.True(reader.Equals(new StringReader("content")));
            Assert.False(reader.Equals(new StringReader("other")));
            Assert.False(reader.Equals(null));
            Assert.False(reader.Equals("content"));
            Assert.Equal(0, new StringReader(null!).GetHashCode());
        }

        /// <summary>
        /// 空读取器
        /// </summary>
        [Fact]
        public async Task TestNullTextReader()
        {
            var reader = new NullTextReader();
            var ctx = CreateContext();

            Assert.Equal(string.Empty, reader.ReadToEnd(ctx));
            Assert.Equal(string.Empty, reader.ReadToEnd());
            Assert.Equal(0, reader.Read(new char[1], 0, 1));
            Assert.Null(reader.ReadLine());
            Assert.Equal(string.Empty, await reader.ReadToEndAsync(ctx));
        }

        /// <summary>
        /// 资源读取器会缓存读取结果
        /// </summary>
        [Fact]
        public async Task TestResourceReader()
        {
            var ctx = CreateContext();
            var reader = new ResourceReader(workFile);
            var other = new ResourceReader(workFile);

            // 未完成读取时按路径比较
            Assert.True(reader.Equals(other));
            Assert.False(reader.Equals(null));
            Assert.False(reader.Equals("x"));
            Assert.Equal(workFile.GetHashCode(), reader.GetHashCode());

            Assert.Equal("demo content", reader.ReadToEnd(ctx));
            // 读取完成后按内容比较
            Assert.True(reader.Equals(other));

            var otherContent = new ResourceReader(Path.Combine(workDir, "missing.txt"));
            Assert.False(reader.Equals(otherContent));
            Assert.Null(otherContent.ReadToEnd(ctx));

            var asyncReader = new ResourceReader(workFile);
            Assert.Equal("demo content", await asyncReader.ReadToEndAsync(ctx));
            Assert.Equal("demo content", await asyncReader.ReadToEndAsync(ctx));
        }

        /// <summary>
        /// 模板读取器
        /// </summary>
        [Fact]
        public async Task TestTemplateReader()
        {
            var ctx = CreateContext();
            var reader = new TemplateReader(workFile);
            var other = new TemplateReader(workFile);

            Assert.True(reader.Equals(other));
            Assert.False(reader.Equals(null));
            Assert.False(reader.Equals("x"));
            Assert.Equal(workFile.GetHashCode(), reader.GetHashCode());

            Assert.Equal("demo content", reader.ReadToEnd(ctx));
            Assert.True(reader.Equals(other));

            var missing = new TemplateReader(Path.Combine(workDir, "missing.txt"));
            Assert.Null(missing.ReadToEnd(ctx));
            Assert.False(reader.Equals(missing));

            var asyncReader = new TemplateReader(workFile);
            Assert.Equal("demo content", await asyncReader.ReadToEndAsync(ctx));
            Assert.Equal("demo content", await asyncReader.ReadToEndAsync(ctx));
        }

        /// <summary>
        /// 路径判断
        /// </summary>
        [Fact]
        public void TestIsAbsolutePath()
        {
            Assert.True(workFile.IsAbsolutePath());
            Assert.False("demo.txt".IsAbsolutePath());
            Assert.False(Path.Combine("sub", "demo.txt").IsAbsolutePath());
            Assert.True(Path.Combine(workDir, "..", "demo.txt").IsAbsolutePath());
        }

        /// <summary>
        /// 文件加载器
        /// </summary>
        [Fact]
        public async Task TestFileLoader()
        {
            var loader = new FileLoader();
            var ctx = CreateContext();

            // 绝对路径
            var info = loader.Load(ctx, workFile);
            Assert.NotNull(info);
            Assert.Equal("demo content", info!.Content);
            Assert.Equal(workFile, info.FullPath);

            // 不存在的文件
            Assert.Null(loader.Load(ctx, Path.Combine(workDir, "not-exists.txt")));

            // 相对路径：基于 CurrentPath 查找
            ctx.CurrentPath = workDir;
            Assert.Equal(workFile, loader.Find(ctx, "demo.txt"));

            // 相对路径：基于资源目录查找
            ctx.CurrentPath = null;
            ctx.ResourceDirectories.Clear();
            ctx.ResourceDirectories.Add(workDir);
            Assert.Equal(workFile, loader.Find(ctx, "demo.txt"));

            // 带结尾分隔符的资源目录
            ctx.ResourceDirectories.Clear();
            ctx.ResourceDirectories.Add(workDir + Path.DirectorySeparatorChar);
            Assert.Equal(workFile, loader.Find(ctx, "demo.txt"));

            // 非法路径不会命中
            Assert.Null(loader.Find(ctx, "../demo.txt"));

            // 异步加载
            var asyncInfo = await loader.LoadAsync(ctx, workFile);
            Assert.NotNull(asyncInfo);
            Assert.Equal("demo content", asyncInfo!.Content);
            Assert.Null(await loader.LoadAsync(ctx, Path.Combine(workDir, "not-exists.txt")));

            var asyncRelative = await loader.LoadAsync(ctx, "demo.txt");
            Assert.NotNull(asyncRelative);
        }

        /// <summary>
        /// 资源管理器
        /// </summary>
        [Fact]
        public async Task TestResourceManager()
        {
            var env = new Hosting.DefaultHostEnvironment();
            var ctx = new EngineBuilder().Build().CreateContext();
            var manager = env.ResourceManager;

            Assert.False(manager.EnableWatcher);
            Assert.Same(env.ResourceLoader, manager.ResourceLoader);
            Assert.Same(env.TemplateWatcherProvider, manager.WatcherProvider);

            Assert.NotNull(manager.Load(ctx, workFile));
            Assert.Null(manager.Load(ctx, Path.Combine(workDir, "not-exists.txt")));
            Assert.Equal(workFile, manager.Find(ctx, workFile));

            var asyncInfo = await manager.LoadAsync(ctx, workFile);
            Assert.NotNull(asyncInfo);

            // 开启监控后会创建监控器
            var provider = new TestWatcherProvider();
            env.TemplateWatcherProvider = provider;
            manager.EnableWatcher = true;
            Assert.True(provider.Created);

            // 重复设置不会重复创建
            manager.EnableWatcher = true;
            manager.EnableWatcher = false;
            manager.EnableWatcher = false;
            manager.EnableWatcher = true;

            manager.Dispose();
        }

        /// <summary>
        /// 文件监控器
        /// </summary>
        [Fact]
        public void TestFileTemplateWatcher()
        {
            var provider = new FileTemplateWatcherProvider();
            using (var watcher = provider.Create())
            {
                Assert.IsType<FileTemplateWatcher>(watcher);

                Assert.False(watcher.Watch(Path.Combine(workDir, "not-exists.txt")));
                Assert.True(watcher.Watch(workFile));
                // 重复监控同一文件
                Assert.True(watcher.Watch(workFile));
            }
        }

        /// <summary>
        /// 模板事件参数
        /// </summary>
        [Fact]
        public void TestTemplateEventArgs()
        {
            var args = new TemplateEventArgs(workFile, new[] { "a", "b" });
            Assert.Equal(workFile, args.FullPath);
            Assert.Equal(new[] { "a", "b" }, args.Keys);
        }

        /// <summary>
        /// 资源信息
        /// </summary>
        [Fact]
        public void TestResourceInfo()
        {
            var info = new ResourceInfo { Content = "c", FullPath = "p" };
            Assert.Equal("c", info.Content);
            Assert.Equal("p", info.FullPath);
        }
    }
}
