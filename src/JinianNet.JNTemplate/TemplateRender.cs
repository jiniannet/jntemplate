/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Dynamic; 

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 基本模板呈现
    /// </summary>
    public class TemplateRender
    {
        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey { get; set; }

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext Context { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent { get; set; }
        /// <summary>
        /// /模板文件地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            var text = ReadTemplateContent();
            var tags = ReadAll(text);
            Render(writer, tags);
        }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        /// <param name="collection">Tags</param>
        public virtual void Render(System.IO.TextWriter writer, ITag[] collection)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("\"writer\" cannot be null.");
            }

            if (collection != null && collection.Length > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    try
                    {
                        var tagResult = Executor.Exec(collection[i], this.Context);
                        if (tagResult != null)
                        {
                            writer.Write(tagResult.ToString());
                        }
                    }
                    catch (Exception.TemplateException e)
                    {
                        ThrowException(e, collection[i], writer);
                    }
                    catch (System.Exception e)
                    {
                        System.Exception baseException = e.GetBaseException();
                        Exception.ParseException ex = new Exception.ParseException(baseException.Message, baseException);
                        ThrowException(ex, collection[i], writer);
                    }
                }
            }
        }

        /// <summary>
        /// 读取模板内容
        /// </summary>
        /// <returns></returns>
        protected string ReadTemplateContent()
        {
            if (!string.IsNullOrWhiteSpace(this.TemplateContent))
            {
                return this.TemplateContent;
            }
            if (!string.IsNullOrWhiteSpace(this.Path))
            {
                if (this.Context == null)
                {
                    throw new System.ArgumentNullException(nameof(Context));
                }
                var res = this.Context.Load(this.Path);
                if (res == null)
                {
                    throw new Exception.TemplateException($"Path:\"{this.Path}\", the file could not be found.");
                }
                return res.Content;
            }
            return null;
        }


        /// <summary>
        /// read all tags
        /// </summary>
        /// <param name="text">text content</param>
        /// <returns></returns>
        public ITag[] ReadAll(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new ITag[0];
            }
            var findOnCache = this.Context.EnableTemplateCache
                && !string.IsNullOrEmpty(this.TemplateKey)
                && !Engine.EnableCompile;

            ITag[] tags;
            if (findOnCache && (tags = Runtime.Cache.Get<ITag[]>(this.TemplateKey)) != null)
            {
                return tags;
            }

            var lexer = new TemplateLexer(text);
            var ts = lexer.Execute();

            var parser = new TemplateParser(ts);
            tags = parser.Execute();

            if (findOnCache)
            {
                Runtime.Cache.Set(this.TemplateKey, tags);
            }

            return tags;
        } 

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">异常信息</param>
        /// <param name="tag">影响标签</param>
        /// <param name="writer">TextWriter</param>
        private void ThrowException(Exception.TemplateException e, ITag tag, System.IO.TextWriter writer)
        {
            if (this.Context.ThrowExceptions)
            {
                throw e;
            }
            else
            {
                this.Context.AddError(e);
                writer.Write(tag.ToString());
            }
        }



        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set<T>(string key, T value)
        {
            Context.TempData.Set<T>(key, value);
        }

        /// <summary>
        /// 设置静态对象
        /// </summary>
        /// <param name="key">对象名</param>
        /// <param name="type">类型</param>
        public void SetStaticType(string key, Type type)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = type.Name;
            }
            Context.TempData.Set(key, null, type);
        }
    }
}