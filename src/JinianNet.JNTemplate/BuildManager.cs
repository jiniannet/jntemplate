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
        private readonly static EngineCollection _engines = new EngineCollection();
        /// <summary>
        /// 模板处理引擎
        /// </summary>
        public static EngineCollection Engines
        {
            get {
                return _engines;
            }
        }

        /// <summary>
        /// 创建Template实殃
        /// </summary>
        /// <param name="path">模板路径</param>
        /// <returns></returns>
        public static ITemplate CreateTemplate(String path)
        {
            if (Engines.Count == 0)
            {
                Engines.Add(new Engine());
            }

            ITemplate template = null;
            foreach (IEngine engine in Engines)
            {
                template = engine.CreateTemplate(path);
                if (template != null)
                {
                    return template;
                }
            }

            return null;

        }
    }
}
