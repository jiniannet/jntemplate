/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using JinianNet.JNTemplate.Parser;
using JinianNet.JNTemplate.Parser.Node;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 模板实例类
    /// </summary>
    public class Template : BlockTag, ITemplate
    {
        private TemplateContext _context;
        /// <summary>
        /// TemplateContext
        /// </summary>
        public TemplateContext Context
        {
            get
            {
                return _context;
            }
            set { _context = value; }
        }

        /// <summary>
        /// Template
        /// </summary>
        public Template()
            : this(null)
        {

        }

        /// <summary>
        /// Template
        /// </summary>
        /// <param name="text">模板内容</param>
        public Template(String text)
            : this(new TemplateContext(), text)
        {

        }

        /// <summary>
        /// Template
        /// </summary>
        /// <param name="context">TemplateContext 对象</param>
        /// <param name="text">模板内容</param>
        public Template(TemplateContext context, String text)
        {
            this._context = context;
            this.TemplateContent = text;
        }

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Render(TextWriter writer)
        {
            base.Render(this.Context, writer);
        }

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <returns></returns>
        public String Render()
        {
            String document;

            using (StringWriter writer = new StringWriter())
            {
                Render(writer);
                document = writer.ToString();
            }

            return document;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(String key, Object value)
        {
            Context.TempData[key] = value;
        }

        /// <summary>
        /// 批量设置数据
        /// </summary>
        /// <param name="dic">字典</param>
        public void Set(Dictionary<String, Object> dic)
        {
            foreach (KeyValuePair<String, Object> value in dic)
            {
                Set(value.Key, value.Value);
            }
        }

        /// <summary>
        /// 从指定的文件加载 Template
        /// </summary>
        /// <param name="filename">完整的本地文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static Template FromFile(String filename, Encoding encoding)
        {
            TemplateContext ctx = new TemplateContext();
            ctx.Charset = encoding;
            ctx.CurrentPath = System.IO.Path.GetExtension(filename);

            Template template = new Template(ctx, Resources.Load(filename, encoding));

            return template;
        }

    }
}
