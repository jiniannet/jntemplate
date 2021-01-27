using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Test
{
    public class Entity
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Name {get;set;}
    }

    public class Nav
    {
        public class ClassValue
        {
            public int Sort { get; set; }
        }
        public ClassValue Class { get; set; }
    }
}
