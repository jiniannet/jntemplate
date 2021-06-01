/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Exceptions;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// A template for Render
    /// </summary>
    public class TemplateRender : TemplateBase
    {
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        public virtual void Render(System.IO.TextWriter writer)
        {
            var text = this.TemplateContent;
            var tags = ReadAll(text);
            Render(writer, tags);
        }

        /// <summary>
        /// Performs the render for a tags.
        /// </summary>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        /// <param name="collection">The tags collection.</param>
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
                        var tagResult = TagExecutor.Execute(collection[i], this.Context);
                        if (tagResult != null)
                        {
                            writer.Write(tagResult.ToString());
                        }
                    }
                    catch (TemplateException e)
                    {
                        ThrowException(e, collection[i], writer);
                    }
                    catch (System.Exception e)
                    {
                        var baseException = e.GetBaseException();
                        ThrowException(new ParseException(collection[i], baseException), collection[i], writer);
                    }
                }
            }
        }



        /// <summary>
        /// Rreads the contents of the text into a tag array.
        /// </summary>
        /// <param name="text">The text for reading.</param>
        /// <returns></returns>
        public ITag[] ReadAll(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new ITag[0];
            }
            var findOnCache = this.Context.EnableTemplateCache
                && !string.IsNullOrEmpty(this.TemplateKey);

            ITag[] tags;
            if (findOnCache && (tags = Context.Cache.Get<ITag[]>(this.TemplateKey)) != null)
            {
                return tags;
            }

            var lexer = Context.CreateTemplateLexer(text);
            var ts = lexer.Execute(); 
            var parser = Context.CreateTemplateParser(ts);
            tags = parser.Execute();

            if (findOnCache)
            {
                Context.Cache.Set(this.TemplateKey, tags);
            }

            return tags;
        }

        /// <summary>
        /// Throw exception.
        /// </summary>
        /// <param name="e">Represents errors that occur during application execution.</param>
        /// <param name="tag">Represents errors tag.</param>
        /// <param name="writer">See the <see cref="System.IO.TextWriter"/>.</param>
        private void ThrowException(TemplateException e, ITag tag, System.IO.TextWriter writer)
        {
            this.Context.AddError(e);
            if (!this.Context.ThrowExceptions)
            {
                writer.Write(tag.ToSource());
            }
        } 

    }
}