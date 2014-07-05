using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ElseTag : ElseifTag
    {
        private BooleanTag testValue;

        public ElseTag()
        {
            this.testValue = new BooleanTag();
            this.testValue.Value = true;
        }

        public override Tag Test
        {
            get
            {
                return testValue;
            }
            set { }
        }
    }
}
