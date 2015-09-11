/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Text;
using System.IO;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// LOAD标签
    /// </summary>
    public class LoadTag : BlockTag
    {
        private Tag path;
        /// <summary>
        /// 路径
        /// </summary>
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            Object path = this.Path.Parse(context);
            LoadResource(path, context);
            return base.Parse(context);
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            Object path = this.Path.Parse(context);
            LoadResource(path, context);
            base.Parse(context, write);
        }

        private void LoadResource(Object path, TemplateContext context)
        {
            if (path != null)
            {
                if (String.IsNullOrEmpty(context.CurrentPath))
                {
                    this.TemplateContent = Resources.LoadResource(path.ToString(),context.Charset);
                }
                else
                {
                    this.TemplateContent = Resources.LoadResource(
                        Resources.MergerPaths(Resources.Paths, context.CurrentPath), 
                        path.ToString(), 
                        context.Charset);
                }
            }
        }
    }
}