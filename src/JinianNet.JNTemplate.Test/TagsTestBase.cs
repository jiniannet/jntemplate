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
