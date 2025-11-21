using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Test.ViewModel
{
    public class Goods
    {
        public int Id { get; set; } 
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
}
