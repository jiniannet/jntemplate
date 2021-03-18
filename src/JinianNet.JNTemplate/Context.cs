/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using JinianNet.JNTemplate.Caching;
using JinianNet.JNTemplate.Dynamic;
using JinianNet.JNTemplate.Parsers;
using JinianNet.JNTemplate.Resources;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// Base class with Context.
    /// </summary>
    [Serializable]
    public class Context
    {
        private string currentPath;
        private Encoding charset;
        private bool throwErrors;
        private bool stripWhiteSpace;
        private List<string> resourceDirectories;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class
        /// </summary>
        public Context()
        {
            this.resourceDirectories = new List<string>();
            this.currentPath = null;
            this.throwErrors = Utility.StringToBoolean(Runtime.GetEnvironmentVariable(nameof(Configuration.IConfig.ThrowExceptions)));
            this.stripWhiteSpace = Utility.StringToBoolean(Runtime.GetEnvironmentVariable(nameof(Configuration.IConfig.StripWhiteSpace))); 
            this.charset = Runtime.Encoding;
        }

        /// <summary>
        /// Strip white-space characters from the template
        /// </summary>
        public bool StripWhiteSpace
        {
            get { return stripWhiteSpace; }
            set { this.stripWhiteSpace = value; }
        }

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        public string CurrentPath
        {
            get { return this.currentPath; }
            set { this.currentPath = value; }
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Charset
        {
            get { return this.charset; }
            set { this.charset = value; }
        }

        /// <summary>
        /// Gets or sets the exception handling.
        /// </summary>
        public bool ThrowExceptions
        {
            get { return this.throwErrors; }
            set { this.throwErrors = value; }
        }
    }
}
