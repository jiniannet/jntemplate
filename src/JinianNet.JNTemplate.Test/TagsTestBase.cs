using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Test
{
    public class TagsTestBase
    {

        public TagsTestBase()
        {
            var conf = Configuration.EngineConfig.CreateDefault();
            //开始严格大小写模式 默认忽略大小写
            //conf.IgnoreCase = false;
            //Engine.EnableCompile = false;
            Engine.Configure(conf);
            //Engine.EnableCompile = false;
        }

        public async Task<string> Excute(ITemplate t)
        {
            string document;

            using (StringWriter writer = new StringWriter())
            {
                t.Render(writer);
                // await t.RenderAsync(writer);
                document = writer.ToString();
            }

            return document;

        }
    }
}
