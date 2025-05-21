
namespace JinianNet.JNTemplate.Hosting
{
    /// <summary>
    /// Provides convenience methods for creating instances of <see cref="EngineBuilder"/> with pre-configured defaults.
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineBuilder"/> class with pre-configured defaults.
        /// </summary>
        public static EngineBuilder CreateDefaultBuilder() =>
            new EngineBuilder();
    }
}
