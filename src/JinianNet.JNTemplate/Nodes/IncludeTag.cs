/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// INCLUDE标签
    /// </summary>
    public class IncludeTag : TagBase
    {
        private Tag _path;
        /// <summary>
        /// 模板路径
        /// </summary>
        public Tag Path
        {
            get { return this._path; }
            set { this._path = value; }
        }

        private String LoadResource(Object path, TemplateContext context)
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
        public override Object Parse(TemplateContext context)
        {
            Object path = this._path.Parse(context);
            return LoadResource(path, context);
        }

    }
}