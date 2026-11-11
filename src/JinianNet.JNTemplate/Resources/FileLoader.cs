/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Text;
#if !NF35 && !NF20
using System.Threading.Tasks;
#endif

namespace JinianNet.JNTemplate.Resources;

/// <summary>
/// The resource loader
/// </summary>
public class FileLoader : IResourceLoader
{
    /// <inheritdoc />
    public virtual ResourceInfo? Load(ITemplateContext ctx, string filename)
    {

        var full = Find(ctx, filename);

        if (string.IsNullOrEmpty(full) || !File.Exists(full))
        {
            return null;
        }
        return new ResourceInfo
        {
            Content = LoadResource(full!, ctx.Charset),
            FullPath = full
        };
    }

    /// <inheritdoc />
    public string? Find(ITemplateContext ctx, string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return null;
        if (filename.IsAbsolutePath())
        {
            return filename;
        }
        if (!string.IsNullOrEmpty(ctx.CurrentPath))
        {
            var full = FindPath(filename, new string[] { ctx.CurrentPath! });
            if (!string.IsNullOrEmpty(full))
                return full;
        }
        return FindPath(filename, ctx.GetResourceDirectories());
    }

    /// <summary>
    /// Find the specified file
    /// </summary>
    /// <param name="paths">The resource search directory.</param>
    /// <param name="filename">The file name.</param>
    /// <returns>The full path.</returns>
    private static string? FindPath(string filename, IEnumerable<string> paths)
    {
        if (string.IsNullOrEmpty(filename) || paths == null)
            return null;

        var fullPath = NormalizePath(filename);
        if (fullPath == null)
        {
            return null;
        }

        int i = 0;
        foreach (string checkUrl in paths)
        {
            if (checkUrl[checkUrl.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                fullPath = string.Concat(checkUrl, fullPath);
            }
            else
            {
                fullPath = string.Concat(checkUrl.Remove(checkUrl.Length - 1, 1), fullPath);
            }
            if (System.IO.File.Exists(fullPath))
            {
                return fullPath;
            }
            i++;
        }

        return null;

    }

    /// <summary>
    /// Load the resource
    /// </summary>
    /// <param name="fullPath">The fully qualified path or file name to load.</param>
    /// <param name="encoding">The <see cref="Encoding"/>.</param> 
    /// <returns>An string.</returns>
    private static string LoadResource(string fullPath, Encoding encoding)
    {
        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }
        return File.ReadAllText(fullPath, encoding);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static string? NormalizePath(string filename)
    {
        if (string.IsNullOrEmpty(filename) || filename.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
        {
            return null;
        }

        List<string> values = new List<string>(filename.Split('/'));

        int i = 0;
        while (i < values.Count)
        {
            if (values[i] == "." || string.IsNullOrEmpty(values[i]))
            {
                values.RemoveAt(i);
                i--;
            }
            else if (values[i] == "..")
            {
                if (i == 0)
                {
                    return null;
                }
                values.RemoveAt(i);
                i--;
                values.RemoveAt(i);
                i--;
            }
            i++;
        }

        values.Insert(0, string.Empty);

        return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), values.ToArray());
    }

#if !NF40 && !NF45 && !NF35 && !NF20


    /// <summary>
    /// Load the resource
    /// </summary>
    /// <param name="fullPath">The fully qualified path or file name to load.</param>
    /// <param name="encoding">The <see cref="Encoding"/>.</param> 
    /// <returns>An string.</returns>
    private static Task<string> LoadResourceAsync(string fullPath, Encoding encoding)
    {
        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }
#if NFW || NETSTANDARD2_0
        var text = System.IO.File.ReadAllText(fullPath, encoding);
        return Task.FromResult<string>(text);
#else
        return System.IO.File.ReadAllTextAsync(fullPath, encoding);
#endif
    }
    /// <inheritdoc />
    public virtual async Task<ResourceInfo?> LoadAsync(ITemplateContext ctx, string filename)
    {
        var full = filename;
        if (!full.IsAbsolutePath())
        {
            full = Find(ctx, filename);
        }

        if (string.IsNullOrEmpty(full) || !File.Exists(full))
        {
            return null;
        }
        return new ResourceInfo
        {
            Content = await LoadResourceAsync(full!, ctx.Charset),
            FullPath = full
        };
    }
#endif

}