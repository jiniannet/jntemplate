using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITemplateWatcher : IDisposable
    {
        /// <summary>
        /// Occurs when a file or directory in the specified path  is changed.
        /// </summary>
        event EventHandler<FileSystemEventArgs> Changed;

        /// <summary>
        /// monitor template
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Watch(ITemplateContext ctx, string path);
    }
}
