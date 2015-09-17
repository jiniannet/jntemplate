/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 引擎集合
    /// </summary>
    public class EngineCollection : Collection<IEngine>
    {
        /// <summary>
        /// 引擎集合
        /// </summary>
        public EngineCollection()
        {
        }
        /// <summary>
        /// 引擎集合
        /// </summary>
        public EngineCollection(IList<IEngine> list)
            : base(list)
        {
        }
        /// <summary>
        /// 在指定位置插入引擎
        /// </summary>
        protected override void InsertItem(Int32 index, IEngine item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }
        /// <summary>
        /// 设置引擎
        /// </summary>
        protected override void SetItem(Int32 index, IEngine item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }
    }
}