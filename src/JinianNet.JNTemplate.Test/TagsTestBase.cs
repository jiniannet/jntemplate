using JinianNet.JNTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JinianNet.JNTemplate.Test
{
    public class TagsTestBase
    {
        public string Excute(ITemplate t)
        {
            string document;

            using (StringWriter writer = new StringWriter())
            {
#if NETCOREAPP || NETSTANDARD
                t.RenderAsync(writer).GetAwaiter().GetResult();
#else
                t.Render(writer);
#endif
                document = writer.ToString();
            }

            return document;

        }
    }
}
