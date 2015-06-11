/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// Foreach标签
    /// </summary>
    public class ForeachTag : BaseTag
    {

        private String name;
        /// <summary>
        /// 节点名
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Tag source;
        /// <summary>
        /// 源对象
        /// </summary>
        public Tag Source
        {
            get { return source; }
            set { source = value; }
        }

        private void Excute(Object value, TemplateContext context, System.IO.TextWriter writer)
        {
            IEnumerable enumerable = Common.ReflectionHelpers.ToIEnumerable(value);
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                Int32 i = 0;
                while (ienum.MoveNext())
                {
                    i++;
                    ctx.TempData[this.Name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (Int32 n = 0; n < this.Children.Count; n++)
                    {
                        this.Children[n].Parse(ctx, writer);
                    }
                    
                }
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="writer">writer</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter writer)
        {
            if (this.Source != null)
            {
                Excute(this.Source.Parse(context), context, writer);
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Object Parse(TemplateContext context)
        {
            using (System.IO.StringWriter write = new System.IO.StringWriter())
            {
                Excute(this.Source.Parse(context), context, write);
                return write.ToString();
            }
        }

    }
}