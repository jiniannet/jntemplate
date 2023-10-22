using JinianNet.JNTemplate.CodeCompilation;
using JinianNet.JNTemplate.Exceptions;
using JinianNet.JNTemplate.Nodes;
using JinianNet.JNTemplate.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 
    /// </summary>
    public class VisitorEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public ITagVisitor Visitor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Next { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Resolver : IList<ITagVisitor>
    {
        private Dictionary<string, VisitorEntry> refs;
        private string firstKey;
        private string lastKey;

        /// <inheritdoc />
        public int Count => refs.Count;
        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public ITagVisitor this[int index]
        {
            get
            {
                var key = firstKey;
                foreach (var value in refs.Values)
                    if (value.Index == index)
                        return value.Visitor;
                return null;
            }
            set
            {
                var old = this[index];
                if (old == null)
                    Register(index, value);
                else
                {
                    throw new NotImplementedException();
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Resolver()
        {
            // Visitor
            //Internal
            //CompilerResults
            refs = new Dictionary<string, VisitorEntry>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public VisitorEntry this[string key]
        {
            get
            {
                if (!string.IsNullOrEmpty(key) && refs.TryGetValue(key, out var entry))
                    return entry;
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="tc"></param>
        /// <returns></returns>
        public ITag Parsing(TemplateParser parser, TokenCollection tc)
        {
            if (tc == null
                || tc.Count == 0
                || parser == null)
            {
                return null;
            }
            ITag t;
            var f = refs[firstKey];
            while (f != null)
            {
                ITagVisitor resolver = f.Visitor;
                t = resolver.Parse(parser, tc);
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
                f = refs[f.Next];
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Excute(ITag tag, TemplateContext context)
        {
            if (tag == null)
            {
                return null;
            }
            return Excute(tag.GetType().Name, tag, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Excute(string name, ITag tag, TemplateContext context)
        {
            var resolver = FindEntry(name);
            if (resolver != null)
            {
                return resolver.Excute(tag, context);
            }
            throw new ParseException($"The tag \"{name}\" is not supported .");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public MethodInfo Compile<T>(T tag, CompileContext context)
            where T : ITag
        {
            return Compile(typeof(T).Name, tag, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public MethodInfo Compile(ITag tag, CompileContext context)
        {
            return Compile(tag.GetType().Name, tag, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public MethodInfo Compile(string name, ITag tag, CompileContext context)
        {
            var resolver = FindEntry(name);
            if (resolver != null)
            {
                return resolver.Compile(tag, context);
            }
            throw new CompileException($"The tag \"{name}\" is not supported .");
        }


        /// <summary>
        /// Gets the <see cref="Type"/> with the specified tag.
        /// </summary>
        /// <param name="tag">The tag of the type to get.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public Type GetType(ITag tag, CompileContext ctx)
        {
            return GuessType(tag.GetType().Name, tag, ctx);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> with the specified tag.
        /// </summary>
        /// <typeparam name="T">ITag</typeparam>
        /// <param name="tag">The tag of the type to get.</param>
        /// <param name="context">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public Type GuessType<T>(T tag, CompileContext context)
            where T : ITag
        {
            return GuessType(typeof(T).Name, tag, context);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> with the specified tag.
        /// </summary>
        /// <param name="tag">The tag of the type to get.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns></returns>
        public Type GuessType(ITag tag, CompileContext ctx)
        {
            return GuessType(tag.GetType().Name, tag, ctx);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> with the specified tag.
        /// </summary>
        /// <param name="name">The tag name of the type to get.</param>
        /// <param name="tag">The tag of the type to get.</param>
        /// <param name="ctx">The <see cref="CompileContext"/>.</param>
        /// <returns>The type with the specified tag, if found; otherwise, null.</returns>
        public Type GuessType(string name, ITag tag, CompileContext ctx)
        {
            var resolver = FindEntry(name);
            if (resolver != null)
            {
                var type = resolver.GuessType(tag, ctx);
                if (type != null)
                {
                    return type;
                }
            }
            throw new CompileException(tag, $"[{name}]:\"{tag.ToSource()}\" is not defined!");
        }

        /// <summary>
        /// Register a guess mehtod for the tag.
        /// </summary>
        /// <param name="visitor">The guess method.</param>
        public bool Register(ITagVisitor visitor)
        {
            VisitorEntry entry;
            if (!refs.TryGetValue(visitor.Name, out entry))
            {
                entry = new VisitorEntry();
                if (refs.Count == 0)
                {
                    firstKey = visitor.Name;
                }
                var last = this[lastKey];
                if (last != null)
                {
                    last.Next = visitor.Name;
                }
                lastKey = visitor.Name;
                entry.Index = refs.Count;
            }
            entry.Visitor = visitor;

            refs[visitor.Name] = entry;

            return true;
        }

        /// <summary>
        /// Register a guess mehtod for the tag.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="visitor">The guess method.</param>
        public bool Register(int index, ITagVisitor visitor)
        {
            if (refs.Count == 0)
                return Register(visitor);

            VisitorEntry entry;
            if (refs.TryGetValue(visitor.Name, out entry))
                return false;

            entry = new VisitorEntry();
            entry.Index = index;
            entry.Visitor = visitor;
            var start = firstKey;
            var next = this[start];
            while (next != null)
            {
                if (next.Index == index - 1)
                {
                    var nextKey = next.Next;
                    next.Next = entry.Visitor.Name;
                    next = this[nextKey];
                    //next.Index++;
                    continue;
                }
                else if (next.Index == index)
                {
                    entry.Next = next.Visitor.Name;
                    next.Index++;
                    refs[visitor.Name] = entry;
                }
                else if (next.Index > index)
                {
                    next.Index++;
                }
                lastKey = next.Visitor.Name;
                next = this[next.Next];

            }
            if (index == 0)
                firstKey = visitor.Name;
            return true;
        }

        private ITagVisitor FindEntry(string key)
        {
            var entry = this[key];
            if (entry != null)
            {
                return entry.Visitor;
            }
            return null;
        }

        /// <inheritdoc />
        public int IndexOf(ITagVisitor item)
        {
            var key = firstKey;
            while (!string.IsNullOrEmpty(key))
            {
                var value = refs[key];
                if (value.Visitor.Name == item.Name)
                    return value.Index;
                key = value.Next;
            }
            return -1;
        }

        /// <inheritdoc />
        public void Insert(int index, ITagVisitor item)
        {
            Register(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Add(ITagVisitor item)
        {
            Register(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            refs.Clear();
        }

        /// <inheritdoc />
        public bool Contains(ITagVisitor item)
        {
            return refs.ContainsKey(item.Name);
        }

        /// <inheritdoc />
        public void CopyTo(ITagVisitor[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Remove(ITagVisitor item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerator<ITagVisitor> GetEnumerator()
        {
            var key = firstKey;
            while (!string.IsNullOrEmpty(key))
            {
                var value = refs[key];
                key = value.Next;
                yield return value.Visitor;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
