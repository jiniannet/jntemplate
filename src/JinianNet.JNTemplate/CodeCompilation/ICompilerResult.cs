using System.IO;

namespace JinianNet.JNTemplate.CodeCompilation
{
    /// <summary>
    /// Represents the results of compilation that are returned from a compiler. 
    /// </summary>
    public interface ICompilerResult
    {
        /// <summary>
        /// Performs the render for a template.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/>.</param>
        /// <param name="context">The <see cref="TemplateContext"/>.</param>
        void Render(TextWriter writer, TemplateContext context);
    }
}
