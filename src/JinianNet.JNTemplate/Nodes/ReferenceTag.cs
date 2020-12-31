/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
#if !NET20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 组合标签
    /// 用于执于复杂的方法或变量
    /// 通常由属性，方法，索引组合，比如
    /// $User.CreateDate.ToString("yyyy-MM-dd")
    /// $Db.Query().Result.Count
    /// </summary>
    [Serializable]
    public class ReferenceTag : BasisTag
    {
        /// <summary>
        /// 子标签
        /// </summary>
        public ITag Child
        {
            get
            {
                if (this.Children.Count > 0)
                {
                    return this.Children[0];
                }
                return null;
            }
        }
        /// <summary>
        /// 添加子标签
        /// </summary>
        /// <param name="node">子标签</param>
        public override void AddChild(ITag node)
        {
            if (this.Children.Count == 0)
            {
                base.AddChild(node);
            }
            else
            {
                ChildrenTag child = (ChildrenTag)node;
                if (child == null)
                {
                    throw new ArgumentException(nameof(node));
                }
                var parent = this.Children[0];
                child.Parent = (BasisTag)parent;
                this.Children[0] = child;
            }
        }

        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override object ParseResult(TemplateContext context)
        {
            if (Child != null)
            {
                return Child.ParseResult(context);
            }
            return null;
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 解析标签
        /// </summary>
        /// <param name="context">上下文</param>
        public override Task<object> ParseResultAsync(TemplateContext context)
        {
            if (Child != null)
            {
                return Child.ParseResultAsync(context);
            }
            return Task.FromResult((object)null); ;
        }
#endif
    }
}