/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Hosting;
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

#if !NF40 && !NF45 && !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Resources
{
    /// <summary>
    /// Resource Manager exposes an template's resources to an engine 
    /// </summary>
    public class ResourceManager : IDisposable
    {
        private ITemplateWatcher watcher;
        private Dictionary<string, List<string>> resources;
        private bool enableWatcher;
        private IHostEnvironment env;

        #region
        /// <summary>
        /// 
        /// </summary>
        public bool EnableWatcher
        {
            get => enableWatcher;
            set
            {
                if (enableWatcher == value)
                    return;
                enableWatcher = value;

                if (value)
                    EnableTemplateWatcher();
                else
                    DisabledTemplateWatcher();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public ITemplateWatcherProvider WatcherProvider => env.TemplateWatcherProvider;
        /// <summary>
        /// 
        /// </summary>
        public IResourceLoader ResourceLoader => env.ResourceLoader;
        #endregion

        #region private

        private void EnableTemplateWatcher()
        {
            if (watcher == null)
            {
                watcher = WatcherProvider.Create();
                watcher.Changed += OnRemoveTemplateCache;
            }
        }

        private void Watch(ITemplateContext ctx, ResourceInfo res)
        {
            if (!EnableWatcher || watcher == null)
                return;

            if (watcher.Watch(ctx, res.FullPath))
            {
                var keys = FindCacheKeys(res.FullPath);
                if (!keys.Contains(ctx.Name))
                    keys.Add(ctx.Name);
            }

        }

        private void OnRemoveTemplateCache(object sender, FileSystemEventArgs e)
        {
            if (!resources.TryGetValue(e.FullPath, out var keys))
                return;
            for (var i = 0; i < keys.Count; i++)
            {
                env.Cache.Remove(keys[i]);
            }
        }

        private List<string> FindCacheKeys(string fullPath)
        {
            if (resources.TryGetValue(fullPath, out var keys))
                return keys;
            keys = new List<string>();
            return resources[fullPath] = keys;
        }

        private void DisabledTemplateWatcher()
        {
            if (watcher != null)
            {
                watcher.Changed -= OnRemoveTemplateCache;
                watcher.Dispose();
                watcher = null;
            }
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class
        /// </summary> 
        /// <param name="env"></param>
        internal ResourceManager(IHostEnvironment env)
        {
            enableWatcher = false;
            resources = new Dictionary<string, List<string>>();
            this.env = env;
        }


        /// <summary>
        /// Loads the resource on the specified path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>An instance of a resource.</returns>
        public ResourceInfo Load(ITemplateContext ctx, string filename)
        {
            var info = ResourceLoader.Load(ctx, filename);
            if (info != null)
            {
                Watch(ctx, info);
            }
            return info;
        }

        /// <summary>
        /// Search for file full path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>The file full path of the resource.</returns>
        public string Find(ITemplateContext ctx, string filename)
        {
            return ResourceLoader.Find(ctx, filename);
        }


#if !NF40 && !NF45 && !NF35 && !NF20

        /// <summary>
        /// Loads the resource on the specified path.
        /// </summary>
        /// <param name="filename">The fully qualified path or file name of the file to load.</param>
        /// <param name="ctx">The <see cref="Context"/>.</param> 
        /// <returns>An instance of a resource.</returns>
        public async Task<ResourceInfo> LoadAsync(ITemplateContext ctx, string filename)
        {
            var info = await ResourceLoader.LoadAsync(ctx, filename);
            if (info != null)
            {
                Watch(ctx, info);
            }
            return info;
        }
#endif


        ///  <inheritdoc/>
        public void Dispose()
        {
            DisabledTemplateWatcher();
            resources.Clear();
        }
    }
}
