using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Exception
{
    public class ParseException : System.Exception
    {
        private Int32 errorLine;
        private Int32 errorColumn;
        public Int32 Line
        {
            get { return errorLine; }
            set { errorLine = value; }
        }

        public Int32 Column
        {
            get { return errorColumn; }
            set { errorColumn = value; }
        }

        public ParseException()
            : base()
        {
        }

        public ParseException(String message)
            : base(message)
        {
        }

        public ParseException(String message,Int32 line,Int32 column)
            : base(message)
        {
            this.errorColumn = column;
            this.errorLine = line;
        }

        public ParseException(String message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return String.Concat("Line:",
                this.Line.ToString(),
                " Column:",
                this.Column.ToString(),
                "\r\n",
                base.ToString());
        }
    }
}
