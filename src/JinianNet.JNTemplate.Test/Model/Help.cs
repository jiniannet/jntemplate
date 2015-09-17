using System;

namespace JinianNet.JNTemplate.Test.Model
{
    public class Help : Basis
    {
        private string _content;
        private int _productid;
        private int _productmoduleid;
        private bool _ishot;
        private bool _isrecommend;
        private string _downloadurl;
        private string _videourl;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            set { _content = value; }
            get { return _content; }
        }
        /// <summary>
        /// 产品
        /// </summary>
        public int ProductId
        {
            set { _productid = value; }
            get { return _productid; }
        }
        /// <summary>
        /// 产品模块
        /// </summary>
        public int ProductModuleId
        {
            set { _productmoduleid = value; }
            get { return _productmoduleid; }
        }
        /// <summary>
        /// 是否热门    
        /// </summary>
        public bool IsHot
        {
            set { _ishot = value; }
            get { return _ishot; }
        }
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool IsRecommend
        {
            set { _isrecommend = value; }
            get { return _isrecommend; }
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
        /// 视频地址
        /// </summary>
        public string VideoUrl
        {
            set { _videourl = value; }
            get { return _videourl; }
        }
    }
}
