/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// OperatorTag
    /// </summary>
    public class OperatorTag: TypeTag<Operator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorTag"/> class
        /// </summary>
        public OperatorTag()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorTag"/> class
        /// </summary>
        /// <param name="token">The <see cref="Token"/>.</param>
        public OperatorTag(Token token)
        {
            this.FirstToken = token;
            this.Value = JNTemplate.Dynamic.OperatorConvert.Parse(token.Text);
        }
    }
}
