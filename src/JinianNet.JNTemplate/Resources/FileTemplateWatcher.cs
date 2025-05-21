/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO; 

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// Listens to the template change notifications and raises events when a directory, or file in a directory, changes.
    /// </summary>

    [DefaultEvent("Changed")]
    public class FileTemplateWatcher : IDisposable,ITemplateWatcher
    {
        /// <summary>
        /// Occurs when a file or directory in the specified path  is changed.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> Changed;
        private List<FileSystemWatcher> pool;
        private List<string> resources;


        /// <summary>
        /// Initializes a new instance of the <see cref="FileTemplateWatcher"/> class.
        /// </summary>
        public FileTemplateWatcher()
        {
            resources = new List<string>();
            pool = new List<FileSystemWatcher>();
        }

        /// <summary>
        /// monitor template
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Watch(ITemplateContext ctx, string path)
        {
            if (!File.Exists(path))
                return false;

            var parent = Path.GetDirectoryName(path);
            if (parent == null)
                return false;

            lock (this)
            {
                if (!Add(path))
                    return true;

                var watcher = GetOrAddWatcher(ctx, parent);

                if (watcher ==null)
                    return false;

                return true;
                //return Watch(watcher, ctx, path);
            }

        }

        private bool Add(string path)
        {
            if (resources.Contains(path))
                return false;
            resources.Add(path);
            return true;
        } 

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (resources.Contains(e.FullPath))
            {
                //var args = new TemplateEventArgs(e.FullPath, item.Keys.ToArray());
                Changed?.Invoke(sender, e);
            }
        } 

        private FileSystemWatcher GetOrAddWatcher(ITemplateContext ctx, string parent)
        {
            var watcher = FindWatcher(parent) ?? ChangeWatcher(parent);


            if (watcher!=null)
                return watcher;


            watcher = new FileSystemWatcher();
            watcher.Path = parent;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += OnFileChanged;

            pool.Add(watcher);

            return watcher;
        }

        private FileSystemWatcher FindWatcher(string path)
        {
            foreach (var watcher in pool)
            {
                if (path.StartsWith(watcher.Path))
                    return watcher;
            }
            return null;
        }

        private FileSystemWatcher ChangeWatcher(string path)
        {
            foreach (var watcher in pool)
            {
                if (path.StartsWith(watcher.Path))
                    return watcher;
                var parent = Path.GetDirectoryName(watcher.Path);
                while (parent!=null)
                {
                    if (path.StartsWith(parent))
                    {
                        watcher.Path = parent;
                        return watcher;
                    }
                    parent = Path.GetDirectoryName(parent);
                }
            }
            return null;
        }

        /// <inheritdoc/> 
        public void Dispose()
        {
            resources.Clear();
            Changed = null; 
            foreach (var watcher in pool)
                watcher.Dispose();
            pool.Clear(); 
        }
    }
}
