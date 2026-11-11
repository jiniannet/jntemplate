namespace JinianNet.JNTemplate.Test
{
    public class TagsTestBase : IClassFixture<EngineFixture>
    {
        /// <summary>
        /// 是否测试异步方法
        /// </summary>
        static public bool IsTestAsync = true;

        public TagsTestBase()
        {

        }

        public static void TestCreateRunDoubleEngine(string content, string render)
         => TestCreateRunDoubleEngine(content, render, null, null);

        public static void TestCreateRunDoubleEngine(string content, string render, Action<ITemplate> templateConfig)
            => TestCreateRunDoubleEngine(content, render, null, templateConfig);

        public static void TestCreateRunDoubleEngine(string content, string render, Action<IEngine> engineConfig, Action<ITemplate> templateConfig)
            => TestCreateRunDoubleEngine(content, engineConfig, templateConfig, (result) => Assert.Equal(render, result));
        public static void TestCreateRunDoubleEngine(string content, Action<IEngine> engineConfig, Action<ITemplate> templateConfig, Action<string> assert)
        {
            var comResult = TestCreateRunAndReturn(content, true, engineConfig, templateConfig);
            assert(comResult);

            var inResult = TestCreateRunAndReturn(content, false, engineConfig, templateConfig);
            assert(inResult);
        }


        public static string TestCreateRunAndReturn(string content, bool isCompile)
            => TestCreateRunAndReturn(content, isCompile, null, null);

        public static string TestCreateRunAndReturn(string content, bool isCompile, Action<ITemplate> templateConfig)
            => TestCreateRunAndReturn(content, isCompile, null, templateConfig);

        public static string TestCreateRunAndReturn(string content, bool isCompile, Action<IEngine> engineConfig, Action<ITemplate> templateConfig)
        {
            var engine = CreateEngine(isCompile);
            engineConfig?.Invoke(engine);
            var template = engine.CreateTemplate(content);
            templateConfig?.Invoke(template);
            return template.Render();
        }


        public static void TestLoadRunDoubleEngine(string path, string render)
            => TestLoadRunDoubleEngine(path, render, null, null);

        public static void TestLoadRunDoubleEngine(string path, string render, Action<ITemplate> templateConfig)
            => TestLoadRunDoubleEngine(path, render, null, templateConfig);

        public static void TestLoadRunDoubleEngine(string path, string render, Action<IEngine> engineConfig, Action<ITemplate> templateConfig)
                        => TestLoadRunDoubleEngine(path, engineConfig, templateConfig, result => Assert.Equal(render, result));
        public static void TestLoadRunDoubleEngine(string path, Action<IEngine> engineConfig, Action<ITemplate> templateConfig, Action<string> assert)
        {
            var comResult = TestLoadRunAndReturn(path, true, engineConfig, templateConfig);
            assert(comResult);

            var inResult = TestLoadRunAndReturn(path, false, engineConfig, templateConfig);
            assert(inResult);
        }


        public static string TestLoadRunAndReturn(string path, bool isCompile)
            => TestLoadRunAndReturn(path, isCompile, null, null);

        public static string TestLoadRunAndReturn(string path, bool isCompile, Action<ITemplate> templateConfig)
            => TestLoadRunAndReturn(path, isCompile, null, templateConfig);
        public static string TestLoadRunAndReturn(string path, bool isCompile, Action<IEngine> engineConfig, Action<ITemplate> templateConfig)
        {
            var engine = CreateEngine(isCompile);
            engineConfig?.Invoke(engine);
            var template = engine.LoadTemplate(path);
            templateConfig?.Invoke(template);
            return template.Render();
        }


        public static IEngine CreateEngine(bool isCompile)
        {
            var builder = new EngineBuilder();
            if (isCompile)
                builder.UseCompileEngine();
            else
                builder.UseInterpretationEngine();
            return builder.Build();
        }

    }
}
