/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    public class EngineCollection: Collection<IEngine> {

        public EngineCollection() {
        }

        public EngineCollection(IList<IEngine> list)
            : base(list) {
        }

        protected override void InsertItem(Int32 index, IEngine item)
        {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(Int32 index, IEngine item)
        {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }
    }
}
