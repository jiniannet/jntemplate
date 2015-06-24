using System;
using System.Collections.Generic;
namespace JinianNet.JNTemplate.Test.ViewModel
{
	/// <summary>
	/// Category:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Category : Model.Category
	{
        private List<Category> _children = new List<Category>();

        public List<Category> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        public string aaa {
            get { return "哈"; }
        }
	}
}

