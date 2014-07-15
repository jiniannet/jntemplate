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
        //private Tag parent;
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


        public virtual Boolean ToBoolean(TemplateContext context)
        {
            Object value = Parse(context);
            if (value == null)
                return false;
            switch (value.GetType().FullName)
            {
                case "System.Boolean":
                    return (Boolean)value;
                case "System.String":
                    return !String.IsNullOrEmpty(value.ToString());
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    return value.ToString()!="0";
                case "System.Decimal":
                    return (Decimal)value != 0;
                case "System.Double":
                    return (Double)value != 0;
                case "System.Single":
                    return (Single)value != 0;
            }
            return value != null;
        }


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

        //public Tag Parent
        //{
        //    set { parent = value; }
        //    get { return parent; }
        //}

        public void AddChild(Tag node)
        {
            //node.Parent = this;
            Children.Add(node);
        }


    }
}