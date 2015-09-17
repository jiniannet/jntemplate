/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    /// <summary>
    /// FOR标签
    /// </summary>
    public class ForTag : BaseTag
    {
        private Tag initial;
        private Tag test;
        private Tag dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public Tag Initial
        {
            get { return initial; }
            set { initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        /// <summary>
        /// Do 
        /// </summary>
        public Tag Do
        {
            get { return dothing; }
            set { dothing = value; }
        }

        private void Excute(TemplateContext context, System.IO.TextWriter writer)
        {
            this.Initial.Parse(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            Boolean run;

            if (this.Test == null)
            {
                run = false;
            }
            else
            {
                run = this.Test.ToBoolean(context);
            }

            while (run)
            {
                for (Int32 i = 0; i < this.Children.Count; i++)
                {
                    this.Children[i].Parse(context, writer);
                }
                if (this.Do != null)
                {
                    this.Do.Parse(context);
                }
                run = this.Test == null ? true : this.Test.ToBoolean(context);
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