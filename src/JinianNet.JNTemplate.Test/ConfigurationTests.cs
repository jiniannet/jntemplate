using System.Text;
using JinianNet.JNTemplate.Configuration;
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Runtime;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 配置相关测试（<see cref="EngineConfig"/>、<see cref="ConfigExtensions"/>、<see cref="RuntimeOptions"/>）
    /// </summary>
    public class ConfigurationTests
    {
        /// <summary>
        /// 默认配置值
        /// </summary>
        [Fact]
        public void TestEngineConfigDefault()
        {
            var config = EngineConfig.CreateDefault();
            Assert.False(config.DisableLogogram);
            Assert.Equal("${", config.TagPrefix);
            Assert.Equal("}", config.TagSuffix);
            Assert.Equal('$', config.TagFlag);
            Assert.NotNull(config.GlobalData);
            Assert.Empty(config.GlobalData);
            Assert.Equal(Encoding.UTF8, config.Encoding);
            Assert.NotNull(config.ResourceDirectories);
            Assert.True(config.EnableCompile);
            Assert.True(config.EnableTemplateCache);
            Assert.True(config.ThrowExceptions);
            Assert.Equal(OutMode.None, config.OutMode);
        }

        /// <summary>
        /// 过时的 DisableeLogogram 属性映射到 DisableLogogram
        /// </summary>
        [Fact]
        public void TestEngineConfigDisableLogogramAlias()
        {
            var config = new EngineConfig();
#pragma warning disable CS0618
            config.DisableeLogogram = true;
            Assert.True(config.DisableLogogram);
            config.DisableLogogram = false;
            Assert.False(config.DisableeLogogram);
#pragma warning restore CS0618
        }

        /// <summary>
        /// 配置转换为字典：只输出基础类型与字符串
        /// </summary>
        [Fact]
        public void TestConfigToDictionary()
        {
            var config = new EngineConfig
            {
                TagFlag = '@',
                OutMode = OutMode.StripWhiteSpace,
                Encoding = Encoding.UTF8,
                TagPrefix = "{{",
                TagSuffix = "}}"
            };

            var dict = config.ToDictionary();

            Assert.Equal("@", dict["TagFlag"]);
            Assert.Equal("{{", dict["TagPrefix"]);
            Assert.Equal("}}", dict["TagSuffix"]);
            Assert.Equal(bool.FalseString, dict["DisableLogogram"]);
            // 集合类型与复杂类型不会被输出
            Assert.False(dict.ContainsKey("GlobalData"));
            Assert.False(dict.ContainsKey("ResourceDirectories"));
        }

        /// <summary>
        /// 环境变量配置特性
        /// </summary>
        [Fact]
        public void TestVariableAttribute()
        {
            var attr = new VariableAttribute();
            Assert.Null(attr.Name);
            Assert.Equal(VariableType.Environment, attr.Type);

            var attr2 = new VariableAttribute("JNT_HOME", VariableType.Environment);
            Assert.Equal("JNT_HOME", attr2.Name);
            Assert.Equal(VariableType.Environment, attr2.Type);

            // 特性只能标注在属性上
            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(VariableAttribute), typeof(AttributeUsageAttribute))!;
            Assert.Equal(AttributeTargets.Property, usage.ValidOn);
            Assert.False(usage.AllowMultiple);
        }

        /// <summary>
        /// 引擎默认选项
        /// </summary>
        [Fact]
        public void TestRuntimeOptionsDefault()
        {
            var env = new DefaultHostEnvironment();
            var options = env.Options;

            Assert.Equal(typeof(RuntimeOptions), options.GetType());
            Assert.Equal(Encoding.UTF8, options.Encoding);
            Assert.Equal('$', options.TagFlag);
            Assert.Equal("${", options.TagPrefix);
            Assert.Equal("}", options.TagSuffix);
            Assert.True(options.ThrowExceptions);
            Assert.False(options.DisableCodeOutput);
            Assert.Equal(OutMode.None, options.OutMode);
            Assert.Equal(EngineMode.Compiled, options.Mode);
            Assert.False(options.DisableLogogram);
            Assert.True(options.EnableCache);
            Assert.Empty(options.ResourceDirectories);
            Assert.Equal(string.Empty, options.ModelName);
            Assert.NotNull(options.Data);

#pragma warning disable CS0618
            options.DisableeLogogram = true;
            Assert.True(options.DisableLogogram);
#pragma warning restore CS0618
        }

        /// <summary>
        /// 默认宿主环境
        /// </summary>
        [Fact]
        public void TestDefaultHostEnvironment()
        {
            var env = new DefaultHostEnvironment();
            Assert.False(string.IsNullOrEmpty(env.ApplicationName));
            Assert.False(string.IsNullOrEmpty(env.RootPath));
            Assert.NotNull(env.Cache);
            Assert.NotNull(env.ScopeProvider);
            Assert.NotNull(env.Resolver);
            Assert.NotNull(env.ResourceManager);
            Assert.NotNull(env.ResourceLoader);
            Assert.NotNull(env.TemplateWatcherProvider);
            Assert.NotNull(env.OutputFormatters);
            Assert.Empty(env.EnvironmentVariable);
#if DEBUG
            Assert.Equal("DEBUG", env.EnvironmentName);
#endif
        }

        /// <summary>
        /// 结果缓存集合
        /// </summary>
        [Fact]
        public void TestResultCollection()
        {
            var collection = new ResultCollection<int>();
            Assert.Equal(0, collection.Count);
            Assert.Equal(0, collection["none"]);

            collection["a"] = 1;
            collection["a"] = 2;
            Assert.Equal(1, collection.Count);
            Assert.Equal(2, collection["a"]);
            Assert.Single(collection.Keys);

            Assert.Equal(3, collection.GetOrAdd("b", key => 3));
            Assert.Equal(3, collection.GetOrAdd("b", key => 4));
            Assert.Equal(2, collection.Count);

            Assert.True(collection.Remove("a"));
            Assert.False(collection.Remove("a"));
            Assert.Equal(0, collection["a"]);

            collection.Clear();
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection.Keys);
        }

        /// <summary>
        /// 模板缓存
        /// </summary>
        [Fact]
        public void TestTemplateCache()
        {
            var cache = new TemplateCache();
            Assert.Equal(0, cache.Count);

            var result = new InterpretResult(new Nodes.TagCollection());
            cache["a"] = result;
            Assert.Equal(1, cache.Count);
            Assert.Same(result, cache["a"]);
            Assert.Same(result, cache.GetOrAdd("a", key => new InterpretResult(new Nodes.TagCollection())));

            Assert.True(cache.Remove("a"));
            Assert.False(cache.Remove("a"));

            cache["b"] = result;
            cache.Clear();
            Assert.Equal(0, cache.Count);
        }

        /// <summary>
        /// 主机构建器工厂
        /// </summary>
        [Fact]
        public void TestHostCreateDefaultBuilder()
        {
            var builder = Host.CreateDefaultBuilder();
            Assert.NotNull(builder);
            var engine = builder.UseCompileEngine().Build();
            Assert.Equal(EngineMode.Compiled, engine.Mode);

            var interpreted = new EngineBuilder().UseInterpretationEngine().Build();
            Assert.Equal(EngineMode.Interpreted, interpreted.Mode);
        }
    }
}
