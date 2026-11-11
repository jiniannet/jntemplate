namespace JinianNet.JNTemplate.Test
{
    public static class TemplateExtensions
    {
        /// <summary>
        /// 是否测试异步方法
        /// </summary>
        static public bool IsTestAsync = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Render(this ITemplate t)
        {
            if (IsTestAsync)
                return JNTemplate.
                    TemplateExtensions.
                    RenderAsync(t).
                    GetAwaiter().
                    GetResult();
            return JNTemplate.
                    TemplateExtensions.Render(t);
        }
    }
}
