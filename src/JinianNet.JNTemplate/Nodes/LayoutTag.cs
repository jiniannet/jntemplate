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
    /// Layout标签
    /// </summary>
    [Serializable]
    public class LayoutTag : LoadTag
    {
        /// <summary>
        /// 读取取签
        /// </summary>
        /// <returns></returns>
        protected override ITag[] ReadTags()
        {
            ITag[] tags = base.ReadTags();
            return ProcessBody(tags);
        } 
        private ITag[] ProcessBody(ITag[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                return tags;
            }
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] is BodyTag)
                {
                    BodyTag tag = (BodyTag)tags[i];
                    for (int j = 0; j < this.Children.Count; j++)
                    {
                        tag.AddChild(this.Children[j]);
                    }
                    this.Children.Clear();
                    tags[i] = tag;
                }
            }

            return tags;
        }
    }
}
