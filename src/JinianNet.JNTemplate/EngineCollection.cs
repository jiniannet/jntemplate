/*****************************************************
   Copyright (c) 2013-2015 jiniannet (http://www.jiniannet.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   Redistributions of source code must retain the above copyright notice
 *****************************************************/

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// ÒýÇæ¼¯ºÏ
    /// </summary>
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
