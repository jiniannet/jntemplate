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

        /// <summary>
        /// Returns tag instance of <see cref="string"/> 
        /// </summary>
        /// <param name="tag">The <see cref="TextTag"/> </param>
        /// <param name="mode">The <see cref="OutMode"/></param>
        /// <returns></returns>
        public static string ToString(this TextTag tag, OutMode mode)
        {
            switch (mode)
            {
                case OutMode.Auto:
                    if ((tag.Previous == null || !tag.Previous.Out) && !string.IsNullOrEmpty(tag.Text))
                    {
                        if (tag.Text.Length > 1 && tag.Text[0] == '\r' && tag.Text[1] == '\n')
                        {
                            return tag.Text.Substring(2);
                        }

                        if (tag.Text.Length > 0 && tag.Text[0] == '\n')
                        {
                            return tag.Text.Substring(1);
                        }
                    }
                    return tag.Text;
                case OutMode.StripWhiteSpace:
                    return tag.Text?.Trim();
                default:
                    return tag.Text;
            }
        }
    }
}
