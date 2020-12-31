/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Nodes;
#if !NET20
using System.Threading.Tasks;
#endif

using JinianNet.JNTemplate.Caching;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 基本模板呈现
    /// </summary>
    public class TemplateRender
    {
        private TemplateContext context;
        private string content;
        private string key;

        /// <summary>
        /// 模板KEY(用于缓存，默认为文路径)
        /// </summary>
        public string TemplateKey
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// 模板上下文
        /// </summary>
        public TemplateContext Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent
        {
            get { return this.content; }
            set { this.content = value; }
        }

        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public void Render(System.IO.TextWriter writer)
        {
            Render(writer, ReadTags());
        }
#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        public async Task RenderAsync(System.IO.TextWriter writer)
        {
            ITag[] ts = await ReadTagsAsync();
            await RenderAsync(writer, ts);
        }


        /// <summary>
        /// 呈现内容
        /// </summary>
        /// <param name="writer">TextWriter</param>
        /// <param name="collection">Tags</param>
        public virtual async Task RenderAsync(System.IO.TextWriter writer, ITag[] collection)
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
                        var tagResult = await collection[i].ParseResultAsync(this.context);
                        if (tagResult != null)
                        {
                            await writer.WriteAsync(tagResult.ToString());
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
#endif

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
                        var tagResult = collection[i].ParseResult(this.context);
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
        /// read all tags
        /// </summary>
        /// <returns></returns>
        public ITag[] ReadTags()
        {
            ITag[] tags = GetCacheTags();
            if (tags != null)
            {
                return tags;
            }
            var lexer = new TemplateLexer(this.content);
            var ts = lexer.Execute();

            var parser = new TemplateParser(this.Context.TagParser, ts);
            tags = parser.Execute();
            SetCacheTags(tags);
            return tags;
        }

        private ITag[] GetCacheTags()
        {
            if (string.IsNullOrEmpty(this.content))
            {
                return new ITag[0];
            }
            if (context.EnableTemplateCache && !string.IsNullOrEmpty(this.key) && Context.Cache != null)
            {
                return Context.Cache.Get<ITag[]>(this.key);
            }
            return null;
        }

        private void SetCacheTags(ITag[] tags)
        {
            if (tags != null && context.EnableTemplateCache && !string.IsNullOrEmpty(this.key) && Context.Cache != null)
            {
                Context.Cache.Set(this.key, tags);
            }
        }

#if NETCOREAPP || NETSTANDARD

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ITag[]> ReadTagsAsync()
        {
            ITag[] tags = GetCacheTags();
            if (tags != null)
            {
                return tags;
            }
            var lexer = new TemplateLexer(this.content);
            var ts = await lexer.ExecuteAsync();
            var parser = new TemplateParser(this.Context.TagParser, ts);
            tags = await parser.ExecuteAsync();
            SetCacheTags(tags);
            return tags;
        }
#endif

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e">异常信息</param>
        /// <param name="tag">影响标签</param>
        /// <param name="writer">TextWriter</param>
        private void ThrowException(Exception.TemplateException e, ITag tag, System.IO.TextWriter writer)
        {
            if (this.context.ThrowExceptions)
            {
                throw e;
            }
            else
            {
                this.context.AddError(e);
                writer.Write(tag.ToString());
            }
        }
    }
}