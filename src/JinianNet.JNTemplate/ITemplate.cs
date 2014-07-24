/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Template 基类
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// TemplateContext
        /// </summary>
        TemplateContext Context { get;set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        String TemplateContent { get;set; }
        /// <summary>
        /// 结果呈现
        /// </summary>
        /// <param name="writer"></param>
        void Render(TextWriter writer);
    }
}
