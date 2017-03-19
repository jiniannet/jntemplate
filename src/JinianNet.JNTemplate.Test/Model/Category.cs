using System;
namespace JinianNet.JNTemplate.Test.Model
{
    /// <summary>
    /// Category:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
	public partial class Category
	{
		public Category()
		{}
		#region Model
		private int _categoryid;
		private string _categoryname;
		private string _keywords;
		private string _description;
		private string _englistname;
		private string _template;
		private string _infotemplate;
		private bool _isverify;
		private int _sequence;
		private int _parentid;
		private DateTime _createdate;
		private int _depth;
		private string _allparentid="";
		private string _allchildid="";
		private int _moduleid;
        private string _turnurl;
        private bool _isurl;

		/// <summary>
		/// 
		/// </summary>
		public int CategoryId
		{
			set{ _categoryid=value;}
			get{return _categoryid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CategoryName
		{
			set{ _categoryname=value;}
			get{return _categoryname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string KeyWords
		{
			set{ _keywords=value;}
			get{return _keywords;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Description
		{
			set{ _description=value;}
			get{return _description;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string EnglishName
		{
			set{ _englistname=value;}
			get{return _englistname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Template
		{
			set{ _template=value;}
			get{return _template;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string InfoTemplate
		{
			set{ _infotemplate=value;}
			get{return _infotemplate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsVerify
		{
			set{ _isverify=value;}
			get{return _isverify;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Sequence
		{
			set{ _sequence=value;}
			get{return _sequence;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ParentId
		{
			set{ _parentid=value;}
			get{return _parentid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateDate
		{
			set{ _createdate=value;}
			get{return _createdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Depth
		{
			set{ _depth=value;}
			get{return _depth;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string AllParentId
		{
			set{ _allparentid=value;}
			get{return _allparentid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string AllChildId
		{
			set{ _allchildid=value;}
			get{return _allchildid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ModuleId
		{
			set{ _moduleid=value;}
			get{return _moduleid;}
		}

        /// <summary>
        /// 
        /// </summary>
        public string TurnUrl
        {
            set { _turnurl = value; }
            get { return _turnurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsUrl
        {
            set { _isurl = value; }
            get { return _isurl; }
        }
		#endregion Model

	}
}

