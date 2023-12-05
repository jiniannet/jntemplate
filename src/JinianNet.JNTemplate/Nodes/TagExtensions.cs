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
        static char[][] newLines = new char[][] {
            new char[]{ '\r','\n' },
            new char[]{ '\n' },
        };

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

        private static int StartsWithNewLine(string text)
        {
            for (var i = 0; i < newLines.Length; i++)
            {
                if (text.Length < newLines[i].Length)
                    continue;
                var find = true;
                for (var j = 0; j < newLines[i].Length; j++)
                {
                    if (text[j] != newLines[i][j])
                    {
                        find = false;
                        break;
                    } 
                }
                if(find)
                    return newLines[i].Length;
            }
            return 0;
        }

        /// <summary>
        /// Returns tag instance of <see cref="string"/> 
        /// </summary>
        /// <param name="tag">The <see cref="TextTag"/> </param>
        /// <param name="mode">The <see cref="OutMode"/></param>
        /// <returns></returns>
        public static string ToString(this TextTag tag, OutMode mode)
        {
            var text = tag?.FirstToken?.Text;
            if (string.IsNullOrEmpty(text))
                return text;
            switch (mode)
            {
                case OutMode.Auto:
                    if (!tag.Previous)
                    {
                        int len = StartsWithNewLine(text);
                        if (len > 0)
                        {
                            if (text.Length <= len)
                            {
                                return string.Empty;
                            }
                            text = text.Substring(len);
                        }
                    }
                    if (!tag.Next && text.Length > 0 && text[text.Length - 1] == ' ')
                    {
                        var preText = text.TrimEnd(' ');
                        if (preText.Length > 0 && preText[preText.Length - 1] == '\n')
                            text = preText;
                    }
                    return text;
                case OutMode.StripWhiteSpace:
                    return text.Trim();
                default:
                    return text;
            }
        }
    }
}
