/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    public class BuildManager
    {
        private static EngineCollection _engines = new EngineCollection();

        public static EngineCollection Engines
        {
            get {
                return _engines;
            }
        }

        public static ITemplate CreateTemplate(String path)
        {
            if (Engines.Count == 0)
            {
                Engines.Add(new Engine());
            }

            ITemplate template = null;
            foreach (IEngine engine in Engines)
            {
                template = engine.CreateTemplate();
                if (template != null)
                {
                    break;
                }
            }
            if (template == null)
            {
                throw new ArgumentException("ITemplate");
            }

            if (!string.IsNullOrEmpty(path))
            {
                String fullPath = path;
                Int32 index = fullPath.IndexOf(System.IO.Path.VolumeSeparatorChar);
                if (index == -1)
                {
                    if (Resources.FindPath(template.Context.Paths.ToArray(), path, out fullPath) == -1)
                    {
                        return template;
                    }
                }

                template.TemplateContent = Resources.Load(fullPath, template.Context.Charset);
            }

            return template;

        }
    }
}
