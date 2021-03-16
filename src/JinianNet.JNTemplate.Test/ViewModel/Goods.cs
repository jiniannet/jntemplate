using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Test.ViewModel
{
    public class Goods
    {
        public int id { get; set; } 
        public string img_url { get; set; }
        public string title { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
