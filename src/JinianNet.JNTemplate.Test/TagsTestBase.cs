using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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

        /// <summary>
        /// 构建一个新引擎
        /// </summary>
        /// <returns></returns>
        public IEngine BuildEngine()
        {
            var builder = new EngineBuilder();
            if (Engine.Mode == EngineMode.Interpreted)
                return builder.DisableCompile().Build();
            return builder.EnableCompile().Build();
        }

      
    }
}
