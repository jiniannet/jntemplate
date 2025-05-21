/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public class FileTemplateWatcherProvider : ITemplateWatcherProvider
    {
        ///  <inheritdoc/>>
        public ITemplateWatcher Create()
        {
            return new FileTemplateWatcher();
        }
    }
}
