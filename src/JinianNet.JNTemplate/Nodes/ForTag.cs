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
        private ITag condition;
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
        public ITag Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
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
        public override object ParseResult(TemplateContext context)
        {
            this.initial.ParseResult(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            bool run;

            if (this.Condition == null)
            {
                run = false;
            }
            else
            {
                run = Utility.ToBoolean(this.Condition.ParseResult(context));
            }
            using (var writer = new StringWriter())
            {
                while (run)
                {
                    for (int i = 0; i < this.Children.Count; i++)
                    {
                        var obj = this.Children[i].ParseResult(context);
                        if (obj != null) {
                            writer.Write(obj.ToString());
                        }
                    }

                    if (this.dothing != null)
                    {
                        //执行计算，不需要输出，比如i++
                        this.dothing.ParseResult(context);
                    }
                    run = Utility.ToBoolean(this.Condition.ParseResult(context));
                }
                return writer.ToString();
            }
        }

    }
}