using JinianNet.JNTemplate.Test.Model;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// <see cref="Engine"/> 静态入口测试（仅验证无副作用的调用）
    /// </summary>
    public class EngineStaticTests : IDisposable
    {
        private readonly string workDir;
        private readonly string workFile;
        private readonly string parseFile;

        public EngineStaticTests()
        {
            workDir = Path.Combine(Path.GetTempPath(), "jntemplate-engine-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(workDir);

            // 编译模式需要在编译时已知变量类型，因此第一个文件不包含变量
            workFile = Path.Combine(workDir, "demo.txt");
            File.WriteAllText(workFile, "hello world");

            parseFile = Path.Combine(workDir, "parse.txt");
            File.WriteAllText(parseFile, "hello ${Entity.Name}");
        }

        public void Dispose()
        {
            if (Directory.Exists(workDir))
                Directory.Delete(workDir, true);
        }

        /// <summary>
        /// 配置引擎
        /// </summary>
        [Fact]
        public void TestConfigure()
        {
            var engine = Engine.Configure(options => { });
            Assert.Same(Engine.Current, engine);
        }

        /// <summary>
        /// 批量编译文件
        /// </summary>
        [Fact]
        public void TestCompileFileCollection()
        {
            var files = new[] { new FileInfo(workFile) };
            Engine.CompileFile(files);

            var template = Engine.LoadTemplate(workFile);
            Assert.Equal("hello world", template.Render());
        }

        /// <summary>
        /// 基于文件解析模板
        /// </summary>
        [Fact]
        public void TestParseFile()
        {
            var result = Engine.Parse(new FileInfo(parseFile), new Entity { Name = "file" });
            Assert.Equal("hello file", result);
        }

        /// <summary>
        /// 文件监控开关
        /// </summary>
        [Fact]
        public void TestFileWatcherSwitch()
        {
            var engine = Engine.EnableFileWatcher();
            Assert.Same(Engine.Current, engine);
            Assert.Same(Engine.Current, Engine.DisableFileWatcher());
        }
    }
}
