
/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
namespace JinianNet.JNTemplate.Hosting;

/// <summary>
/// Provides convenience methods for creating instances of <see cref="EngineBuilder"/> with pre-configured defaults.
/// </summary>
public static class Host
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EngineBuilder"/> class with pre-configured defaults.
    /// </summary>
    public static EngineBuilder CreateDefaultBuilder() =>
        new EngineBuilder();
}
