using System.Text;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 用于 Parse 测试的模型类型
    /// </summary>
    public class ApiModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 引擎实例 API 测试
    /// </summary>
    public class EngineApiTests : IDisposable
    {
        private readonly string workDir;

        public EngineApiTests()
        {
            workDir = Path.Combine(Path.GetTempPath(), "jntemplate-api-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }
        }

        private string CreateFile(string name, string content)
        {
            var path = Path.Combine(workDir, name);
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        private static TemplatingEngine CreateEngine()
        {
            return (TemplatingEngine)new EngineBuilder().UseCompileEngine().Build();
        }

        /// <summary>
        /// 解析文本并渲染
        /// </summary>
        [Fact]
        public void TestParseText()
        {
            var engine = CreateEngine();
            Assert.Equal("hello", engine.Parse("hello", (Action<TemplateContext>)null!));

            Assert.Equal("3", engine.Parse("${a+b}", ctx =>
            {
                ctx.Set("a", 1, typeof(int));
                ctx.Set("b", 2, typeof(int));
            }));

            Assert.Throws<ArgumentNullException>(() => engine.Parse((string)null!, (Action<TemplateContext>)null!));
        }

        /// <summary>
        /// 解析文件并渲染
        /// </summary>
        [Fact]
        public void TestParseFile()
        {
            var engine = CreateEngine();
            var path = CreateFile("parse.txt", "hello,${name}");
            var file = new FileInfo(path);

            Assert.Equal("hello,jntemplate", engine.Parse(file, ctx => ctx.Set("name", "jntemplate", typeof(string))));
            Assert.Throws<ArgumentNullException>(() => engine.Parse((FileInfo)null!, (Action<TemplateContext>)null!));
        }

        /// <summary>
        /// 使用字典数据渲染
        /// </summary>
        [Fact]
        public void TestParseDictionary()
        {
            var engine = CreateEngine();
            var data = new Dictionary<string, object>
            {
                { "name", "jntemplate" },
                { "year", 2020 }
            };

            Assert.Equal("jntemplate:2020", engine.Parse("${name}:${year}", data));

            // 非泛型字典使用 object 作为默认类型
            var table = new System.Collections.Hashtable { { "name", "hash" } };
            Assert.Equal("hash", engine.Parse("${name}", table));
        }

        /// <summary>
        /// 使用匿名对象渲染
        /// </summary>
        [Fact]
        public void TestParseAnonymous()
        {
            var engine = CreateEngine();
            var result = engine.Parse("${name}", new { name = "anonymous" });
            Assert.Equal("anonymous", result);
        }

        /// <summary>
        /// 使用模型渲染
        /// </summary>
        [Fact]
        public void TestParseModel()
        {
            var engine = CreateEngine();
            var model = new ApiModel { Id = 7, Name = "model" };
            Assert.Equal("7-model", engine.Parse("${ApiModel.Id}-${ApiModel.Name}", model));

            // 模型名称可配置
            engine.Configure(o => o.ModelName = "m");
            Assert.Equal("model", engine.Parse("${m.Name}", model));

            // 枚举统一使用模型名称作为键
            Assert.Equal("Tuesday", engine.Parse("${m}", DayOfWeek.Tuesday));
        }

        /// <summary>
        /// 空数据时按普通模板处理
        /// </summary>
        [Fact]
        public void TestParseWithDefaultData()
        {
            var engine = CreateEngine();
            Assert.Equal("ok", engine.Parse<string>("ok", null!));
            Assert.Equal("ok", engine.Parse<int>("ok", 0));
        }

        /// <summary>
        /// 编译文本与文件
        /// </summary>
        [Fact]
        public void TestCompile()
        {
            var engine = CreateEngine();
            var result = engine.Compile("compile-test", "hello");
            Assert.NotNull(result);

            using (var writer = new StringWriter())
            {
                result.Render(writer, engine.CreateContext());
                Assert.Equal("hello", writer.ToString());
            }

            // 名称为空时自动生成
            var autoName = engine.Compile(null!, "auto name");
            Assert.NotNull(autoName);

            Assert.Throws<ArgumentNullException>(() => engine.Compile("x", null!));

            // 编译文件
            var path = CreateFile("compile.txt", "file");
            var fileResult = engine.CompileFile("compile-file", path);
            Assert.NotNull(fileResult);

            using (var writer = new StringWriter())
            {
                fileResult.Render(writer, engine.CreateContext());
                Assert.Equal("file", writer.ToString());
            }

            // 批量编译文件
            var second = CreateFile("compile2.txt", "second");
            engine.CompileFile(new[] { new FileInfo(path), new FileInfo(second) });
            Assert.NotNull(engine.HostEnvironment.Cache[path]);
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        [Fact]
        public void TestCreateTemplate()
        {
            var engine = CreateEngine();
            var template = engine.CreateTemplate("hello,${name}");
            Assert.True(template.IsCompileMode);
            Assert.False(string.IsNullOrEmpty(template.TemplateKey));
            template.Set("name", "jntemplate");
            Assert.Equal("hello,jntemplate", template.Render());

            var named = engine.CreateTemplate("my-template", "${v}");
            Assert.Equal("my-template", named.TemplateKey);

            // 空名称自动生成
            var auto = engine.CreateTemplate(null!, "content");
            Assert.False(string.IsNullOrEmpty(auto.TemplateKey));
        }

        /// <summary>
        /// 加载模板文件
        /// </summary>
        [Fact]
        public void TestLoadTemplate()
        {
            var engine = CreateEngine();
            var path = CreateFile("load.txt", "load,${name}");

            var template = engine.LoadTemplate(path);
            template.Set("name", "jntemplate");
            Assert.Equal("load,jntemplate", template.Render());
            Assert.Equal(path, template.TemplateKey);

            var named = engine.LoadTemplate("load-name", path);
            Assert.Equal("load-name", named.TemplateKey);

            // 相对路径找不到时会抛出模板异常
            Assert.Throws<TemplateException>(() => engine.LoadTemplate("not-exists-" + Guid.NewGuid().ToString("N") + ".txt"));
            Assert.Throws<ArgumentNullException>(() => engine.CompileFile("x", null!));
        }

        /// <summary>
        /// 资源目录
        /// </summary>
        [Fact]
        public void TestAppendResourcePath()
        {
            var engine = CreateEngine();
            engine.AppendResourcePath(workDir);
            engine.AppendResourcePath(workDir);
            Assert.Single(engine.HostEnvironment.Options.ResourceDirectories);

            CreateFile("resource.txt", "resource");
            var template = engine.LoadTemplate("resource.txt");
            Assert.Equal("resource", template.Render());
        }

        /// <summary>
        /// 环境变量读写
        /// </summary>
        [Fact]
        public void TestEnvironmentVariable()
        {
            var engine = CreateEngine();
            engine.SetEnvironmentVariable("key", "value");
            Assert.Equal("value", engine.GetEnvironmentVariable("key"));
            Assert.Null(engine.GetEnvironmentVariable("not-exists"));

            engine.SetEnvironmentVariable("key", null!);
            Assert.Null(engine.GetEnvironmentVariable("key"));

            Assert.Throws<ArgumentNullException>(() => engine.GetEnvironmentVariable(null!));
            Assert.Throws<ArgumentNullException>(() => engine.SetEnvironmentVariable(null!, "v"));
        }

        /// <summary>
        /// 自定义资源加载器
        /// </summary>
        [Fact]
        public void TestUseLoader()
        {
            var engine = CreateEngine();
            Assert.Throws<ArgumentNullException>(() => engine.UseLoader(null!));

            engine.UseLoader(new MemoryLoader("${name}", "/memory/template"));
            var template = engine.LoadTemplate("/memory/template");
            template.Set("name", "memory");
            Assert.Equal("memory", template.Render());
        }

        /// <summary>
        /// 自定义作用域提供者
        /// </summary>
        [Fact]
        public void TestUseScopeProvider()
        {
            var engine = CreateEngine();
            Assert.Throws<ArgumentNullException>(() => engine.UseScopeProvider(null!));

            engine.HostEnvironment.Options.Data.Set("global", "g", typeof(string));
            var provider = new CountingScopeProvider();
            engine.UseScopeProvider(provider);
            Assert.True(provider.Created > 0);

            // 由引擎生成的作用域会继承全局数据
            var scope = engine.HostEnvironment.CreateVariableScope();
            Assert.Equal("g", scope["global"]);
        }

        /// <summary>
        /// 自定义缓存
        /// </summary>
        [Fact]
        public void TestUseCache()
        {
            var engine = CreateEngine();
            Assert.Throws<ArgumentNullException>(() => engine.UseCache(null!));

            var cache = new TemplateCache();
            engine.UseCache(cache);
            Assert.Same(cache, engine.HostEnvironment.Cache);

            engine.Compile("cache-key", "cache");
            Assert.Equal(1, cache.Count);
        }

        /// <summary>
        /// 选项替换
        /// </summary>
        [Fact]
        public void TestUseOptions()
        {
            var engine = CreateEngine();
            var options = new DefaultHostEnvironment().Options;
            options.Mode = EngineMode.Compiled;
            engine.UseOptions(options);
            Assert.Same(options, engine.HostEnvironment.Options);

            engine.UseDefaultOptions();
            Assert.NotSame(options, engine.HostEnvironment.Options);
            Assert.Equal(EngineMode.Compiled, engine.Mode);

            // 通过 IConfig 配置
            var config = new EngineConfig { EnableCompile = true, TagPrefix = "<%", TagSuffix = "%>" };
            engine.Configure(config);
            Assert.Equal("<%", engine.HostEnvironment.Options.TagPrefix);
            Assert.Equal("%>", engine.HostEnvironment.Options.TagSuffix);

            // 配置模式切换会重新初始化解析器
            var interpreted = CreateEngine();
            interpreted.Configure(c => c.Mode = EngineMode.Interpreted);
            Assert.Equal(EngineMode.Interpreted, interpreted.Mode);
            Assert.True(interpreted.HostEnvironment.Resolver.Count > 0);
        }

        /// <summary>
        /// 通过 Use 定制宿主环境
        /// </summary>
        [Fact]
        public void TestUse()
        {
            var engine = CreateEngine();
            var visited = false;
            engine.Use(env =>
            {
                visited = true;
                env.EnvironmentVariable["custom"] = "1";
            });
            Assert.True(visited);
            Assert.Equal("1", engine.HostEnvironment.EnvironmentVariable["custom"]);
        }

        /// <summary>
        /// 注册自定义标签
        /// </summary>
        [Fact]
        public void TestRegister()
        {
            var engine = CreateEngine();
            var parser = new TestParser();
            var visitor = new DynamicVisitor(typeof(TestTag), parser.Parse, null!, null!);
            engine.Register(visitor);

            Assert.True(engine.HostEnvironment.Resolver.Count > 0);
            Assert.Equal("TestTag", engine.HostEnvironment.Resolver[0]!.Name);
            Assert.Equal("TestTag", visitor.Name);
        }

        /// <summary>
        /// 文件监控开关
        /// </summary>
        [Fact]
        public void TestFileWatcher()
        {
            var engine = CreateEngine();
            Assert.False(engine.HostEnvironment.ResourceManager.EnableWatcher);

            engine.EnableFileWatcher();
            Assert.True(engine.HostEnvironment.ResourceManager.EnableWatcher);

            engine.DisableFileWatcher();
            Assert.False(engine.HostEnvironment.ResourceManager.EnableWatcher);

            // 自定义监控提供者
            var provider = new TestWatcherProvider();
            engine.UseWatcherProvider(provider);
            engine.EnableFileWatcher();
            Assert.True(provider.Created);
            engine.DisableFileWatcher();
        }

        /// <summary>
        /// 清理缓存与释放
        /// </summary>
        [Fact]
        public void TestCleanAndDispose()
        {
            var engine = CreateEngine();
            var cache = new TemplateCache();
            engine.UseCache(cache);

            engine.Compile("clean-key", "clean");
            Assert.Equal(1, cache.Count);

            engine.Clean();
            Assert.Equal(0, cache.Count);

            engine.Compile("clean-key", "clean");
            engine.Dispose();
            Assert.Equal(0, cache.Count);
        }

        /// <summary>
        /// 使用解释引擎模式
        /// </summary>
        [Fact]
        public void TestUseInterpretationEngine()
        {
            var engine = CreateEngine();
            Assert.Equal(EngineMode.Compiled, engine.Mode);

            engine.UseInterpretationEngine();
            Assert.Equal(EngineMode.Interpreted, engine.Mode);

            var template = engine.CreateTemplate("hello,${name}");
            Assert.False(template.IsCompileMode);
            template.Set("name", "interpreted");
            Assert.Equal("hello,interpreted", template.Render());

            engine.UseCompileEngine();
            Assert.Equal(EngineMode.Compiled, engine.Mode);
        }

        /// <summary>
        /// 全局 Engine 静态入口
        /// </summary>
        [Fact]
        public async Task TestStaticEngineApi()
        {
            Assert.NotNull(Engine.Current);
            Assert.Same(Engine.Current, Engine.Current);
            Assert.False(string.IsNullOrEmpty(Engine.Version));
            Assert.Equal(Const.Version, Engine.Version);
            Assert.Equal(Engine.Current.Mode, Engine.Mode);
            Assert.Equal(EngineMode.Compiled, Engine.UseCompileEngine().Mode);

            var ctx = Engine.CreateContext();
            Assert.NotNull(ctx);
            Assert.True(ctx.IsCompileMode);

            var template = Engine.CreateTemplate("static,${name}");
            template.Set("name", "engine");
            Assert.Equal("static,engine", template.Render());
            Assert.Equal("static,engine", await template.RenderAsync());

            var named = Engine.CreateTemplate("static-name", "${name}");
            Assert.Equal("static-name", named.TemplateKey);

            Assert.Equal("1", Engine.Parse("${v}", new { v = 1 }));

            var result = Engine.Compile("static-compile", "static compile");
            Assert.NotNull(result);

            Engine.Clean();
        }
    }

    /// <summary>
    /// 内存资源加载器（用于替换默认文件加载器）
    /// </summary>
    public class MemoryLoader : IResourceLoader
    {
        private readonly Dictionary<string, string> resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public MemoryLoader(string content, params string[] paths)
        {
            foreach (var path in paths)
            {
                resources[path] = content;
            }
        }

        public ResourceInfo Load(ITemplateContext ctx, string filename)
        {
            if (filename == null || !resources.TryGetValue(filename, out var content))
            {
                return null;
            }
            return new ResourceInfo { Content = content, FullPath = filename };
        }

        public string Find(ITemplateContext ctx, string filename)
        {
            return filename != null && resources.ContainsKey(filename) ? filename : null;
        }

        public Task<ResourceInfo> LoadAsync(ITemplateContext ctx, string filename)
        {
            return Task.FromResult(Load(ctx, filename));
        }
    }

    /// <summary>
    /// 统计创建次数的作用域提供者
    /// </summary>
    public class CountingScopeProvider : IScopeProvider
    {
        public int Created { get; private set; }

        public IVariableScope CreateScope()
        {
            Created++;
            return new VariableScope();
        }
    }

    /// <summary>
    /// 测试用监控提供者
    /// </summary>
    public class TestWatcherProvider : ITemplateWatcherProvider
    {
        public bool Created { get; private set; }

        public ITemplateWatcher Create()
        {
            Created = true;
            return new TestWatcher();
        }

        private class TestWatcher : ITemplateWatcher
        {
            public event EventHandler<FileSystemEventArgs> Changed;

            public bool Watch(string path)
            {
                Changed?.Invoke(this, null!);
                return true;
            }

            public void Dispose()
            {
                Changed = null;
            }
        }
    }
}
