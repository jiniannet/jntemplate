/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// INCLUDE标签
    /// </summary>
    public class IncludeTag : ComplexTag
    {
        private ITag _path;
        /// <summary>
        /// 模板路径
        /// </summary>
        public ITag Path
        {
            get { return this._path; }
            set { this._path = value; }
        }

        private string LoadResource(object path, TemplateContext context)
        {
            if (path != null)
            {
                string[] paths = null;
                if (!string.IsNullOrEmpty(context.CurrentPath))
                {
                    paths = new[] { context.CurrentPath };
                }
                ResourceInfo info = Engine.Runtime.Load(path.ToString(), context.Charset, paths);
                if (info != null)
                {
                    return info.Content;
                }
            }
            return null;
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            object path = this._path.ParseResult(context);
            return LoadResource(path, context);
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            write.Write(ParseResult(context));
        }
    }
}