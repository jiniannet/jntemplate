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
    /// IF标签
    /// </summary>
    [Serializable]
    public class IfTag : ComplexTag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            for (int i = 0; i < Children.Count - 1; i++) //最后面一个子对象为EndTag
            {
                var tag = (ElseifTag)Children[i];
                if (tag == null)
                {
                    continue;
                }
                if (tag.ToBoolean(context))
                {
                    Children[i].Parse(context, write);
                    break;
                }
            }
        }

#if NETCOREAPP || NETSTANDARD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="write"></param>
        public override async Task ParseAsync(TemplateContext context, TextWriter write)
        {
            for (int i = 0; i < Children.Count - 1; i++) //最后面一个子对象为EndTag
            {
                var tag = (ElseifTag)Children[i];
                if (tag == null)
                {
                    continue;
                }
                if (tag.ToBoolean(context))
                {
                    await Children[i].ParseAsync(context, write);
                    break;
                }
            }
        }
#endif
    }
}