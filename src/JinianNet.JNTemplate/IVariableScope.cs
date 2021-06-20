/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using JinianNet.JNTemplate.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Variable Scope
    /// </summary>
    public interface IVariableScope
    {

        /// <summary>
        /// Gets or sets the  detect patterns.
        /// </summary>
        TypeDetect DetectPattern { get; set; }

        /// <summary>
        /// Removes all items from the <see cref="IVariableScope"/>.
        /// </summary>
        /// <param name="all">is removes all</param>
        void Clear(bool all);

        /// <summary>
        /// Removes all items from the <see cref="IVariableScope"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// gets the parent from the <see cref="IVariableScope"/>.
        /// </summary>
        IVariableScope Parent { get; set; }

        /// <summary>
        /// update the element with the specified key from the <see cref="IVariableScope"/>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IVariableScope"/>.</param>
        /// <param name="value">The value with the specified key.</param>
        /// <returns>true if the element is successfully updated; otherwise, false.</returns>
        bool Update<T>(string key, T value);

        /// <summary>
        /// Determines whether the <see cref="IVariableScope"/>. contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IVariableScope"/>.</param>
        /// <returns>true if the <see cref="IVariableScope"/> contains an element with the key; otherwise, false.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IVariableScope"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original <see cref="IVariableScope"/>.</returns>
        bool Remove(string key);

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IVariableScope"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get a <see cref="Type"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <returns>The <see cref="Type"/> with the specified key.</returns>
        Type GetType(string key);


        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element with the specified key.</returns>
        object this[string key] { get; }

        /// <summary>
        /// Set a new value for variables.
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        /// <typeparam name="T">The type of elements in the  <see cref="IVariableScope"/>.</typeparam>
        void Set<T>(string key, T value);

        /// <summary>
        /// Set a new <see cref="object"/> for variables
        /// </summary>
        /// <param name="key">The key of the element to get</param> 
        /// <param name="value">The element with the specified key.</param>
        /// <param name="type"><see cref="Type"/> of the value.</param>
        void Set(string key, object value, Type type);

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/>  containing the keys of the <see cref=" IVariableScope"/>.
        /// </summary>
        ICollection<string> Keys { get; }
    }
}
