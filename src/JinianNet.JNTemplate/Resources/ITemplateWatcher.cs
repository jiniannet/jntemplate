/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Resources;

/// <summary>
/// 
/// </summary>
public interface ITemplateWatcher : IDisposable
{
    /// <summary>
    /// Occurs when a file or directory in the specified path  is changed.
    /// </summary>
    event EventHandler<FileSystemEventArgs>? Changed;

    /// <summary>
    /// monitor template
    /// </summary> 
    /// <param name="path"></param>
    /// <returns></returns>
    bool Watch(string path);
}
