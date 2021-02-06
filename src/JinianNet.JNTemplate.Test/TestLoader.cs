using JinianNet.JNTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestLoader : JinianNet.JNTemplate.Resources.IResourceLoader
    {
        public string FindFullPath(string filename, params string[] directory)
        {
            return filename;
        }

        public string GetDirectoryName(string fullPath)
        {
            return fullPath;
        }

        public ResourceInfo Load(string filename, Encoding encoding, params string[] directory)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            };
        }

        public async Task<ResourceInfo> LoadAsync(string filename, Encoding encoding, params string[] directory)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            }; 
        }
    }
}
