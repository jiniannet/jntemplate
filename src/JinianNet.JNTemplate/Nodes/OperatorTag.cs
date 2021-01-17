/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System; 

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// Operator tag
    /// </summary>
    public class OperatorTag: TypeTag<Operator>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public OperatorTag()
        { 
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="token">First Token</param>
        public OperatorTag(Token token)
        {
            this.FirstToken = token;
            this.Value = JNTemplate.Dynamic.OperatorConvert.Parse(token.Text);
        }
    }
}
