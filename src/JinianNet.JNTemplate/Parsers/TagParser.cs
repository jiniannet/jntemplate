/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
#if !NET20
using System.Threading.Tasks;
#endif


namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// 标签分析器
    /// </summary>
    public class TagParser : List<ITagParser>, IList<ITagParser>
    {
        /// <summary>
        /// 分析标签
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="tc"></param>
        /// <returns></returns>
        public ITag Parsing(TemplateParser parser, TokenCollection tc)
        {

            if (tc == null || tc.Count == 0 || parser == null)
            {
                return null;
            }

            ITag t;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == null)
                {
                    continue;
                }
                t = this[i].Parse(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 ||
                        (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null ||
                        tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }
    }
}
