#if !NET20_NOTUSER
using System.Collections.Generic;
using System.Dynamic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 动态行为基类
    /// </summary>
    public class DynamicObject : System.Dynamic.DynamicObject
    {
        /// <summary>
        /// 存储字典
        /// </summary>
        private readonly IDictionary<string, object> _values;

        /// <summary>
        /// 动态类的方法基类
        /// </summary>
        /// <param name="values"></param>
        public DynamicObject(IDictionary<string, object> values = null)
        {
            _values = values ?? (new Dictionary<string, object>());
        }

        /// <summary>  
        /// 获取属性值  
        /// </summary>  
        /// <param name="propertyName"></param>  
        /// <returns></returns>  
        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                return _values[propertyName];
            }
            return null;
        }
        /// <summary>  
        /// 设置属性值  
        /// </summary>  
        /// <param name="propertyName"></param>  
        /// <param name="value"></param>  
        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }
        }
        /// <summary>  
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result != null;
        }
        /// <summary>  
        /// 实现动态对象属性值设置的方法。  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }
    }
}
#endif
