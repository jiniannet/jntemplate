
using System;
namespace JinianNet.JNTemplate.Test.Model
{
    /// <summary>
    /// Product:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class Product : Basis
    {
        private string _content;
        private string _language;
        private string _platform;
        private string _exampleurl;
        private string _downloadurl;
        private int _salestatus;
        private string _filesize;
        private string _version;
        private decimal _saleprice;
        private decimal _userscale;
        private decimal _datescale;
        private string _gateway;
        private string _managerurl;
        private string _settings;

        /// <summary>
        ///  产品介绍
        /// </summary>
        public string Content
        {
            set { _content = value; }
            get { return _content; }
        }
        /// <summary>
        /// 使用语言（英语、简体中文、多语言等）
        /// </summary>
        public string Language
        {
            set { _language = value; }
            get { return _language; }
        }
        /// <summary>
        ///  平台，win2008/XP等
        /// </summary>
        public string Platform
        {
            set { _platform = value; }
            get { return _platform; }
        }
        /// <summary>
        ///  演示地址
        /// </summary>
        public string ExampleUrl
        {
            set { _exampleurl = value; }
            get { return _exampleurl; }
        }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadUrl
        {
            set { _downloadurl = value; }
            get { return _downloadurl; }
        }
        /// <summary>
        ///  销售状态 0 下架 正常
        /// </summary>
        public int SaleStatus
        {
            set { _salestatus = value; }
            get { return _salestatus; }
        }
        /// <summary>
        ///  文件大小
        /// </summary>
        public string FileSize
        {
            set { _filesize = value; }
            get { return _filesize; }
        }
        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version
        {
            set { _version = value; }
            get { return _version; }
        }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal SalePrice
        {
            set { _saleprice = value; }
            get { return _saleprice; }
        }
        /// <summary>
        /// 用户价格基数（每人每月）
        /// </summary>
        public decimal UserPrice
        {
            set { _userscale = value; }
            get { return _userscale; }
        }
        /// <summary>
        /// 时间价格基数（每人每月）
        /// </summary>
        public decimal DatePrice
        {
            set { _datescale = value; }
            get { return _datescale; }
        }
        /// <summary>
        ///  API网关地址
        /// </summary>
        public string Gateway
        {
            set { _gateway = value; }
            get { return _gateway; }
        }
        /// <summary>
        /// 管理地址
        /// </summary>
        public string ManagerUrl
        {
            set { _managerurl = value; }
            get { return _managerurl; }
        }
        /// <summary>
        /// 通信API设置
        /// </summary>
        public string Settings
        {
            set { _settings = value; }
            get { return _settings; }
        }
    }
}
