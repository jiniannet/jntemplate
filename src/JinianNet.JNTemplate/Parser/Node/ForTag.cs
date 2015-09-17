/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// FOR标签
    /// </summary>
    public class ForTag : BaseTag
    {
        private Tag _initial;
        private Tag _test;
        private Tag _dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public Tag Initial
        {
            get { return this._initial; }
            set { this._initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public Tag Test
        {
            get { return this._test; }
            set { this._test = value; }
        }

        /// <summary>
        /// Do 
        /// </summary>
        public Tag Do
        {
            get { return this._dothing; }
            set { this._dothing = value; }
        }

        private void Excute(TemplateContext context, System.IO.TextWriter writer)
        {
            this._initial.Parse(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            Boolean run;

            if (this._test == null)
            {
                run = false;
            }
            else
            {
                run = this._test.ToBoolean(context);
            }

            while (run)
            {
                for (Int32 i = 0; i < Children.Count; i++)
                {
                    Children[i].Parse(context, writer);
                }
                if (this._dothing != null)
                {
                    this._dothing.Parse(context);
                }
                run = this._test == null ? true : this._test.ToBoolean(context);
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
                Excute(context, write);
                return write.ToString();
            }
        }
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="write">write</param>
        public override void Parse(TemplateContext context, System.IO.TextWriter write)
        {
            Excute(context, write);
        }
    }
}