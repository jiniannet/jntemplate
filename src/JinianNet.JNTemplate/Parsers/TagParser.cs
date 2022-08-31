using JinianNet.JNTemplate.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Parsers
{
    /// <summary>
    /// The tag parser.
    /// </summary>
    public class TagParser
    {
        private List<Func<TemplateParser, TokenCollection, ITag>> delegates;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagParser"/> class
        /// </summary>
        public TagParser()
        {
            delegates = new List<Func<TemplateParser, TokenCollection, ITag>>();
        }

        /// <summary>
        ///  Gets the number of elements contained in <see cref="TagParser"/>.
        /// </summary>
        public int Count => delegates.Count;

        /// <inheritdoc />
        public ITag Parsing(TemplateParser parser, TokenCollection tc)
        {
            if (tc == null
                || tc.Count == 0
                || parser == null)
            {
                return null;
            }

            ITag t;
            for (int i = 0; i < delegates.Count; i++)
            {
                t = delegates[i]?.Invoke(parser, tc);
                if (t != null)
                {
                    t.FirstToken = tc.First;

                    if (t.Children.Count == 0 ||
                        (t.LastToken = t.Children[t.Children.Count - 1].LastToken ?? t.Children[t.Children.Count - 1].FirstToken) == null
                        || tc.Last.CompareTo(t.LastToken) > 0)
                    {
                        t.LastToken = tc.Last;
                    }
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Register the new parse method.
        /// </summary>
        /// <param name="func">The parse mehtod.</param>
        public void Register(Func<TemplateParser, TokenCollection, ITag> func)
        {
            Register(func, -1);
        }


        /// <summary>
        /// Register the new parse method.
        /// </summary>
        /// <param name="func">The parse mehtod.</param>
        /// <param name="index">The zero-based index in the <see cref="TagParser"/>. </param>
        public void Register(Func<TemplateParser, TokenCollection, ITag> func, int index)
        {
            if (index >= 0)
            {
                delegates.Insert(index, func);
            }
            else
            {
                delegates.Add(func);
            }
        }

        /// <summary>
        /// Removes all elements from <see cref="TagParser"/>
        /// </summary>
        public void Clear()
        {
            delegates.Clear();
        }
    }
}
