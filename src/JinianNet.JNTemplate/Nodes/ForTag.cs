/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// FOR标签
    /// </summary>
    [Serializable]
    public class ForTag : ComplexTag
    {
        private ITag initial;
        private ITag test;
        private ITag dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public ITag Initial
        {
            get { return this.initial; }
            set { this.initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public ITag Test
        {
            get { return this.test; }
            set { this.test = value; }
        }

        /// <summary>
        /// Do 
        /// </summary>
        public ITag Do
        {
            get { return this.dothing; }
            set { this.dothing = value; }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="writer">writer</param>
        public override void Parse(TemplateContext context, TextWriter writer)
        {
            this.initial.ParseResult(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            bool run;

            if (this.test == null)
            {
                run = false;
            }
            else
            {
                run = Utility.ToBoolean(this.test.ParseResult(context));
            }

            while (run)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Parse(context, writer);
                }
                if (this.dothing != null)
                {
                    //执行计算，不需要输出，比如i++
                    this.dothing.ParseResult(context);
                }
                run = this.test == null ? true : run = Utility.ToBoolean(this.test.ParseResult(context));
            }
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 异步解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="writer">TextWriter</param>
        /// <returns></returns>
        public override async Task ParseAsync(TemplateContext context, TextWriter writer)
        {
            this.initial.ParseResult(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            bool run;

            if (this.test == null)
            {
                run = false;
            }
            else
            {
                run = Utility.ToBoolean(await this.test.ParseResultAsync(context));
            }

            while (run)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    await Children[i].ParseAsync(context, writer);
                }
                if (this.dothing != null)
                {
                    //执行计算，不需要输出，比如i++
                    await this.dothing.ParseResultAsync(context);
                }
                run = this.test == null ? true : run = Utility.ToBoolean(await this.test.ParseResultAsync(context));
            }
        }
#endif
    }
}