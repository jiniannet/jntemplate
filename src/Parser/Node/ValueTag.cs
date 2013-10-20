using System;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ValueTag : SimpleTag
    {

        public ValueTag(ValueType kind, Int32 line, Int32 col)
            : this(kind,null, line, col)
        {

        }

        public ValueTag(ValueType kind, Object value, Int32 line, Int32 col)
            : base(ElementType.Object, line, col)
        {
            this._kind = kind;
            this._value = value;
        }

        private ValueType _kind;
        public ValueType Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        private Object _value;
        public Object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override Object Parse(TemplateContext context)
        {
            return this.Value;
        }
    }
}
