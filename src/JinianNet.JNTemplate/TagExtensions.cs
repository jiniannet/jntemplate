/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

using JinianNet.JNTemplate.Nodes;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// tag Extensions
    /// </summary>
    public static class TagExtensions
    {
        /// <summary>
        /// 获取标签的源代码
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>string</returns>
        public static string ToSource(this ITag tag)
        {
            if (tag == null)
            {
                return null;
            }
            if (tag.FirstToken == null)
            {
                return tag.GetType().Name;
            }
            if (tag.LastToken == null)
            {
                return tag.FirstToken.ToString();
            }
            var sb = new System.Text.StringBuilder();
            Token t = tag.FirstToken;
            do
            {
                sb.Append(t.Text);

                if (t == tag.LastToken)
                {
                    break;
                }

                t = t.Next;

            } while (t != null);

            return sb.ToString();
        }
    }
}
