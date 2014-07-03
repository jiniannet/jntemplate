using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Parser.Node
{
    public class ElseTag : ElseifTag
    {
        public override Tag Test
        {
            get
            {
                return new TrueTag();
            }
            set
            {

            }
        }
    }
}
