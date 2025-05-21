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
                return GetVisitorEntry(index)?.Visitor;
            }
            set
            {

                SetVisitor(index, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Resolver()
        {
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
            var f = this[firstKey];
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
                f = this[f.Next];
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
            lock (this)
            {
                refs[visitor.Name] = entry;
            }
            return true;
        }

        /// <summary>
        /// Register a guess mehtod for the tag.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="visitor">The guess method.</param>

        /// <inheritdoc />
        public void Insert(int index, ITagVisitor visitor)
        {
            if (refs.Count == 0 || (index >= refs.Count))
                Register(visitor);

            VisitorEntry entry;
            if (refs.TryGetValue(visitor.Name, out entry))
                throw new Exception($"the key \"{visitor.Name}\"  exists.");

            entry = new VisitorEntry();
            entry.Index = index;
            entry.Visitor = visitor;


            if (index == 0)
            {
                entry.Next = firstKey;
                firstKey = visitor.Name;
            }
            else
            {
                var prev = GetVisitorEntry(index - 1);

                if (prev == null)
                    throw new InvalidOperationException();

                entry.Next = prev.Next;
                prev.Next = visitor.Name;
            }

            lock (this)
            {
                refs[visitor.Name] = entry;
            }

            string tmpKey = entry.Next;
            int tmpIndex = index + 1;
            VisitorEntry tmpEntry;

            while (!string.IsNullOrEmpty(tmpKey) && (tmpEntry = this[tmpKey])!=null)
            {
                tmpKey = tmpEntry.Next;
                tmpEntry.Index = tmpIndex;
                tmpIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected ITagVisitor FindEntry(string key) => this[key]?.Visitor;

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
        public void RemoveAt(int index)
        {
            var entry = GetVisitorEntry(index);
            if (entry != null)
                Remove(entry);
        }

        /// <inheritdoc />
        public void Add(ITagVisitor item)
        {
            Register(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (this)
            {
                refs.Clear();
            }
        }

        /// <inheritdoc />
        public bool Contains(ITagVisitor item)
        {
            return refs.ContainsKey(item.Name);
        }

        /// <inheritdoc />
        public void CopyTo(ITagVisitor[] array, int arrayIndex)
        {
            lock (this)
            {
                foreach (var node in refs.Values)
                {
                    array[arrayIndex + node.Index] = node.Visitor;
                }
            }
        }

        /// <inheritdoc />
        public bool Remove(ITagVisitor item)
        {
            var entry = this[item.Name];
            if (entry != null)
            {
                return Remove(entry);
            }
            return false;
        }


        /// <inheritdoc />
        private bool Remove(VisitorEntry entry)
        {
            if (entry != null)
            {
                if (entry.Index == 0)
                {
                    firstKey = entry.Next;
                }
                else
                {
                    var prev = GetVisitorEntry(entry.Index - 1);
                    if (prev == null)
                        throw new NullReferenceException();
                    prev.Next = entry.Next;
                }
                lock (this)
                {
                    refs.Remove(entry.Visitor.Name);
                }
                ResetIndex();
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public IEnumerator<ITagVisitor> GetEnumerator()
        {
            var key = firstKey;
            VisitorEntry entry;
            while (!string.IsNullOrEmpty(key) && (entry = this[key])!=null)
            {
                key = entry.Next;
                yield return entry.Visitor;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region
        private VisitorEntry GetVisitorEntry(int index)
        {
            lock (this)
            {
                foreach (var value in refs.Values)
                    if (value.Index == index)
                        return value;
                return null;
            }
        }

        private void ResetIndex()
        {
            var index = 0;
            var key = firstKey;
            VisitorEntry entry;
            while (!string.IsNullOrEmpty(key) && (entry = this[key])!=null)
            {
                key = entry.Next;
                entry.Index = index;
                index++;
            }
        }

        private void SetVisitor(int index, ITagVisitor visitor)
        {
            if (this[visitor.Name]!=null && this[visitor.Name].Index != index)
                throw new Exception($"the key \"{visitor.Name}\"  exists.");

            VisitorEntry old;
            if (index < 0 || index >= this.Count || (old = GetVisitorEntry(index)) == null)
                throw new ArgumentOutOfRangeException("index");

            var entry = new VisitorEntry();
            entry.Index = index;
            entry.Visitor = visitor;
            entry.Next = old.Next;

            if (index == 0)
                firstKey = visitor.Name;
            else
                GetVisitorEntry(index - 1).Next = visitor.Name;

            lock (this)
            {
                refs.Remove(old.Visitor.Name);
                refs[visitor.Name] = entry;
            }

            if(index == this.Count - 1)
                lastKey = visitor.Name;

        }
        #endregion
    }
}
