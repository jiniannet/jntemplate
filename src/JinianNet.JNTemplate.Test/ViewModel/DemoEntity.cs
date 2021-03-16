using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate.Test.ViewModel
{
    public class DemoEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DemoType Type { get; set; }
    }

    public enum DemoType
    {
        None,
        No,
        Yes
    }
}
