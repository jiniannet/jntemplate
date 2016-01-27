namespace JinianNet.JNTemplate.Test.Model
{
    public class ProductModule
    {
        private int _productmoduleid;
        private string _modulename;
        private int _basisid;
        /// <summary>
        /// 产品模块
        /// </summary>
        public int ProductModuleId
        {
            set { _productmoduleid = value; }
            get { return _productmoduleid; }
        }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName
        {
            set { _modulename = value; }
            get { return _modulename; }
        }
        /// <summary>
        /// 产品编号
        /// </summary>
        public int BasisId
        {
            set { _basisid = value; }
            get { return _basisid; }
        }
    }
}
