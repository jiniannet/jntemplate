using System;
namespace JinianNet.JNTemplate.Test.Model
{
	/// <summary>
	/// Article:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Article : Basis
	{
		public Article()
		{}
		#region Model

		private string _content;
        private bool _isaudit;
        private string _source;
        private string _author;
        private string _editor;
        private string _turnurl;
        private bool _isurl;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            set { _content = value; }
            get { return _content; }
        }

        /// <summary>
        ///  是否审核
        /// </summary>
        public bool IsAudit
        {
            set { _isaudit = value; }
            get { return _isaudit; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Source
        {
            set { _source = value; }
            get { return _source; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            set { _author = value; }
            get { return _author; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Editor
        {
            set { _editor = value; }
            get { return _editor; }
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

