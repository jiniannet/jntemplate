﻿/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JinianNet.JNTemplate.Configuration
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ConfigExtensions
    {

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey, TValue}"/> from an <see cref="IConfig"/>.
        /// </summary>
        /// <param name="config">config</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this IConfig config)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Type type = config.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo p in properties)
            {
                if (!p.PropertyType.IsPrimitive
                     && p.PropertyType.FullName != "System.String")
                {
                    continue;
                }
#if NF20 || NF40
                object value = p.GetValue(config, null);
#else
                object value = p.GetValue(config);
#endif
                if (value == null)
                {
                    continue;
                }

                dict[p.Name] = value.ToString();
            }

            return dict;
        }

    }
}
