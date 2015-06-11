/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 ********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// 操作符
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// <![CDATA[none]]>
        /// </summary>
        None,
        /// <summary>
        /// <![CDATA[+]]>
        /// </summary>
        Plus,
        /// <summary>
        /// <![CDATA[-]]>
        /// </summary>
        Minus,
        /// <summary>
        /// <![CDATA[*]]>
        /// </summary>
        Times,
        /// <summary>
        /// <![CDATA[%]]>
        /// </summary>
        Percent,
        /// <summary>
        /// <![CDATA[/]]>
        /// </summary>
        Divided,
        /// <summary>
        /// <![CDATA[|]]>
        /// </summary>
        LogicalOr,
        /// <summary>
        /// <![CDATA[||]]>
        /// </summary>
        Or,
        /// <summary>
        /// <![CDATA[&]]>
        /// </summary>
        LogicAnd,
        /// <summary>
        /// <![CDATA[&&]]>
        /// </summary>
        And,
        /// <summary>
        /// <![CDATA[>]]>
        /// </summary>
        GreaterThan,
        /// <summary>
        /// <![CDATA[>=]]>
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// <![CDATA[<]]>
        /// </summary>
        LessThan,
        /// <summary>
        /// <![CDATA[<=]]>
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// <![CDATA[==]]>
        /// </summary>
        Equal,
        /// <summary>
        /// <![CDATA[!=]]>
        /// </summary>
        NotEqual,
        /// <summary>
        /// <![CDATA[(]]>
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// <![CDATA[)]]>
        /// </summary>
        RightParentheses
    }
}