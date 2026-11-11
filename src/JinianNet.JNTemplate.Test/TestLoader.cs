using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestLoader : IResourceLoader
    {
        public string Find(ITemplateContext ctx, string filename)
        {
            return filename;
        }
         
        public string GetDirectoryName(string fullPath)
        {
            return fullPath;
        }

        public ResourceInfo Load(ITemplateContext ctx, string filename)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            };
        }
 
        public async Task<ResourceInfo> LoadAsync(ITemplateContext ctx, string filename)
        {
            return new ResourceInfo
            {
                Content = $"当前是模板：{ filename} hello,$name",
                FullPath = filename
            };
        }
         
    }
}
