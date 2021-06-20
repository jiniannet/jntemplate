﻿using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestLoader : IResourceLoader
    {
        public string Find(Context ctx, string filename)
        {
            return filename;
        }

        public string GetDirectoryName(string fullPath)
        {
            return fullPath;
        }

        public ResourceInfo Load(Context ctx, string filename)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            };
        }

        public async Task<ResourceInfo> LoadAsync(Context ctx, string filename)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            };
        }
    }
}
