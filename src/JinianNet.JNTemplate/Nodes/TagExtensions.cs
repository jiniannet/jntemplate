/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Extensions methods for <see cref="ITag"/>.
    /// </summary>
    public static class TagExtensions
    {
        /// <summary>
        /// Returns a source code that represents the current tag.
        /// </summary>
        /// <param name="tag">The <see cref="ITag"/>.</param>
        /// <returns>The source code.</returns>
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
