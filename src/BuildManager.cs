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

            if (path.Length > 3 && Char.IsLetter(path[0]) && path[1] == System.IO.Path.VolumeSeparatorChar && path[2] == System.IO.Path.DirectorySeparatorChar)
                template.TemplateContent = Resources.Load(path, template.Context.Charset);
            else
                template.TemplateContent = Resources.LoadResource(new String[] { template.Context.CurrentPath }, path, template.Context.Charset);
            return template;

        }
    }
}
