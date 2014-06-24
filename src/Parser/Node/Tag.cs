/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using JinianNet.JNTemplate.Context;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public abstract class Tag 
    {
        private Token first, last;
        private Tag parent;
        private List<Tag> children;

        public List<Tag> Children { get; private set; }

        public abstract Object Parse(TemplateContext context);

        public abstract Object Parse(Object baseValue, TemplateContext context);

        public abstract void Parse(TemplateContext context, System.IO.TextWriter write);

        public Token FirstToken
        {
            get { return first; }
            set { first = value; }
        }

        public Token LastToken
        {
            get { return last; }
        }

        public Tag Parent
        {
            set { parent = value; }
            get { return parent; }
        }

        public void AddChild(Tag node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            Token t = this.first;
            sb.Append(t.ToString());
            if (this.last != null)
            {
                while ((t = t.Next) != null)
                {
                    sb.Append(t.ToString());
                }
            }

            return sb.ToString();
        }

        public virtual Boolean ToBoolean(JinianNet.JNTemplate.Context.TemplateContext context)
        {
            return Parse(context) == null;
        }

    }
}