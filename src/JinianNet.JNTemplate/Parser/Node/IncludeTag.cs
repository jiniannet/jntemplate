/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// INCLUDE标签
    /// </summary>
    public class IncludeTag : BaseTag
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
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    return Resources.LoadResource(path.ToString(), context.Charset);
                }
                else
                {
                    return Resources.LoadResource(
                        Resources.MergerPaths(Engine.ResourceDirectories, context.CurrentPath),
                        path.ToString(),
                        context.Charset);
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