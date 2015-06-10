using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinianNet.JNTemplate
{
    public enum Operator
    {
        /// <summary>
        /// none
        /// </summary>
        None,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// *
        /// </summary>
        Times,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// /
        /// </summary>
        Divided,
        /// <summary>
        /// |
        /// </summary>
        LogicalOr,
        /// <summary>
        /// ||
        /// </summary>
        Or,
        /// <summary>
        /// &
        /// </summary>
        LogicAnd,
        /// <summary>
        /// &&
        /// </summary>
        And,
        /// <summary>
        /// >
        /// </summary>
        GreaterThan,
        /// <summary>
        /// >=
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// <
        /// </summary>
        LessThan,
        /// <summary>
        /// <=
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// ==
        /// </summary>
        Equal,
        /// <summary>
        /// !=
        /// </summary>
        NotEqual,
        /// <summary>
        /// (
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// )
        /// </summary>
        RightParentheses
    }
}
