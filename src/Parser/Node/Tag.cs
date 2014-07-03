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

        public Tag()
        {
            this.children = new List<Tag>();
        }

        public List<Tag> Children { 
            get { return this.children; }
        }

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
            set {  last = value; }
            get { return last;}
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
            if (this.last != null && this.first != this.last)
            {
                StringBuilder sb = new StringBuilder();
                Token t = this.first;
                sb.Append(t.ToString());
                while ((t = t.Next) != null && t != this.last)
                {
                    sb.Append(t.ToString());
                }
                sb.Append(this.last.ToString());
                return sb.ToString();
            }
            else
            {
                return this.first.ToString();
            }

       
        }

        public virtual Boolean ToBoolean(TemplateContext context)
        {
            return Parse(context) == null;
        }

    }
}