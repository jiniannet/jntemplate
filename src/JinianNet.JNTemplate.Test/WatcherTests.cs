using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 模板监控（<see cref="ResourceManager"/> / <see cref="FileTemplateWatcher"/>）测试
    /// </summary>
    public class WatcherTests : IDisposable
    {
        private readonly string workDir;
        private readonly string workFile;
        private readonly string deepFile;
        private readonly string otherDir;
        private readonly string otherFile;

        public WatcherTests()
        {
            workDir = Path.Combine(Path.GetTempPath(), "jntemplate-watch-" + Guid.NewGuid().ToString("N"));
            var deepDir = Path.Combine(workDir, "sub", "deep");
            Directory.CreateDirectory(deepDir);

            workFile = Path.Combine(workDir, "demo.txt");
            deepFile = Path.Combine(deepDir, "deep.txt");
            File.WriteAllText(workFile, "demo content");
            File.WriteAllText(deepFile, "deep content");

            otherDir = Path.Combine(Path.GetTempPath(), "jntemplate-other-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(otherDir);
            otherFile = Path.Combine(otherDir, "other.txt");
            File.WriteAllText(otherFile, "other content");
        }

        public void Dispose()
        {
            if (Directory.Exists(workDir))
                Directory.Delete(workDir, true);
            if (Directory.Exists(otherDir))
                Directory.Delete(otherDir, true);
        }

        /// <summary>
        /// 开启监控后加载文件会注册监控并维护缓存键
        /// </summary>
        [Fact]
        public void TestResourceManagerWatch()
        {
            var env = new DefaultHostEnvironment();
            var cache = new RecordingTemplateCache();
            var provider = new FakeWatcherProvider();
            env.Cache = cache;
            env.TemplateWatcherProvider = provider;

            var manager = env.ResourceManager;
            Assert.False(manager.EnableWatcher);
            manager.EnableWatcher = true;

            var ctx = new EngineBuilder().Build().CreateContext();
            ctx.Name = "watch-key";

            var info = manager.Load(ctx, workFile);
            Assert.NotNull(info);
            Assert.Contains(workFile, provider.Watcher.Watched);
            Assert.Equal(workFile, manager.Find(ctx, workFile));

            // 文件变更后对应的缓存键被移除
            provider.Watcher.RaiseChanged(workFile);
            Assert.Contains("watch-key", cache.Removed);

            // 未登记的文件不会触发缓存移除
            cache.Removed.Clear();
            provider.Watcher.RaiseChanged(otherFile);
            Assert.Empty(cache.Removed);

            manager.Dispose();
        }

        /// <summary>
        /// 文件监控器：向上合并目录与新建无关目录的监控
        /// </summary>
        [Fact]
        public void TestFileTemplateWatcherDirectories()
        {
            using (var watcher = new FileTemplateWatcher())
            {
                // 深层目录创建监控
                Assert.True(watcher.Watch(deepFile));
                // 父目录中的文件会复用并上移监控目录
                Assert.True(watcher.Watch(workFile));
                // 无关目录会重新创建监控
                Assert.True(watcher.Watch(otherFile));
                // 文件不存在时不监控
                Assert.False(watcher.Watch(Path.Combine(workDir, "missing.txt")));
            }
        }

        /// <summary>
        /// 记录移除操作的模板缓存
        /// </summary>
        private class RecordingTemplateCache : ITemplateCache
        {
            public List<string> Removed { get; } = new List<string>();

            private readonly Dictionary<string, ITemplateResult> items = new Dictionary<string, ITemplateResult>();

            public ITemplateResult GetOrAdd(string key, Func<string, ITemplateResult> valueFactory)
            {
                if (!items.TryGetValue(key, out var value))
                {
                    value = valueFactory(key);
                    items[key] = value;
                }
                return value;
            }

            public bool Remove(string text)
            {
                Removed.Add(text);
                return items.Remove(text);
            }

            public ITemplateResult this[string name]
            {
                get => items.TryGetValue(name, out var value) ? value : null;
                set => items[name] = value;
            }

            public void Clear()
            {
                items.Clear();
            }
        }

        /// <summary>
        /// 可主动触发变更事件的监控提供者
        /// </summary>
        private class FakeWatcherProvider : ITemplateWatcherProvider
        {
            public FakeTemplateWatcher Watcher { get; } = new FakeTemplateWatcher();

            public ITemplateWatcher Create()
            {
                return Watcher;
            }
        }

        private class FakeTemplateWatcher : ITemplateWatcher
        {
            public List<string> Watched { get; } = new List<string>();

            public event EventHandler<FileSystemEventArgs> Changed;

            public bool Watch(string path)
            {
                Watched.Add(path);
                return true;
            }

            public void RaiseChanged(string path)
            {
                Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(path), Path.GetFileName(path)));
            }

            public void Dispose()
            {
                Changed = null;
            }
        }
    }
}
