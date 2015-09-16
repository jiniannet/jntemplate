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

        private String _name;
        private Tag _source;

        /// <summary>
        /// 节点名
        /// </summary>
        public String Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// 源对象
        /// </summary>
        public Tag Source
        {
            get { return this._source; }
            set { this._source = value; }
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
                    ctx.TempData[this._name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (Int32 n = 0; n < Children.Count; n++)
                    {
                        Children[n].Parse(ctx, writer);
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
            if (Source != null)
            {
                Excute(Source.Parse(context), context, writer);
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
                Excute(Source.Parse(context), context, write);
                return write.ToString();
            }
        }

    }
}