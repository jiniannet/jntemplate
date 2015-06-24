using System;
namespace JinianNet.JNTemplate.Test.Model
{
	/// <summary>
	/// Basis:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Basis
	{
        private int _basisid = 0;
        private string _title = "";
        private string _keywords = "";
        private string _description = "";
        private int _categoryid = 0;
        private string _englistname = "";
        private string _template = "";
        private int _hits = 0;
        private DateTime _editdate = DateTime.Now;
        private DateTime _createdate = DateTime.Now;
        private string _defaultpicture = "";
        private int _sequence = 0;
        private int _moduleid = 0;
        private int _userid = 0;
        private DateTime _lasthitsdate = DateTime.Now;
        private int _hitsbymonth = 0;
        private int _hitsbyweek = 0;
        private int _hitsbyday = 0;
        /// <summary>
        /// 
        /// </summary>
        public int BasisId
        {
            set { _basisid = value; }
            get { return _basisid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string KeyWords
        {
            set { _keywords = value; }
            get { return _keywords; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId
        {
            set { _categoryid = value; }
            get { return _categoryid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EnglishName
        {
            set { _englistname = value; }
            get { return _englistname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Template
        {
            set { _template = value; }
            get { return _template; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Hits
        {
            set { _hits = value; }
            get { return _hits; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EditDate
        {
            set { _editdate = value; }
            get { return _editdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DefaultPicture
        {
            set { _defaultpicture = value; }
            get { return _defaultpicture; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Sequence
        {
            set { _sequence = value; }
            get { return _sequence; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int ModuleId
        {
            set { _moduleid = value; }
            get { return _moduleid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastHitsDate
        {
            set { _lasthitsdate = value; }
            get { return _lasthitsdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int HitsByMonth
        {
            set { _hitsbymonth = value; }
            get { return _hitsbymonth; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int HitsByWeek
        {
            set { _hitsbyweek = value; }
            get { return _hitsbyweek; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int HitsByDay
        {
            set { _hitsbyday = value; }
            get { return _hitsbyday; }
        }
	}
}

