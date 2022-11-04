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
            /* 友情提示：实际开发中，应该注入一个IEngine的单例 ，
             * 或者使用系统默认的Engine类，不要每次请求创建一个IEngine
             * 不然会降低引擎性能，带来不必要的初始化开销
             * 本处是为了单元测试时，某些全局配置不影响其它单元测试使用
             * 并不适合实际开发情况
             */
            var builder = new EngineBuilder();
            if (Engine.Mode == EngineMode.Interpreted) //解释引擎更灵活，适用于访问频率不高但是希望更灵活的场景，比如代码生成器，规则引擎之类  
                return builder.UseInterpretationEngine().Build();
            return builder.UseCompileEngine().Build();// 编译引擎适合访问频高，对性能要求更严重的场景，比如HTML模板引擎
        }

      
    }
}
