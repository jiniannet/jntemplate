/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/

namespace JinianNet.JNTemplate
{
    /// <summary>
    /// The token kind.
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// The text.
        /// </summary>
        Text,
        /// <summary>
        /// The text inside the tags 
        /// </summary>
        TextData,
        /// <summary>
        /// tag
        /// </summary>
        Tag,
        /// <summary>
        /// Start tag ${
        /// </summary>
        TagStart,
        /// <summary>
        /// End tag }
        /// </summary>
        TagEnd,
        /// <summary>
        /// string.
        /// </summary>
        String,
        /// <summary>
        /// number
        /// </summary>
        Number,
        /// <summary>
        /// Left Bracket (
        /// </summary>
        LeftBracket,
        /// <summary>
        /// Right Bracket )
        /// </summary>
        RightBracket,
        /// <summary>
        /// Left Parentheses [
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// Right Parentheses ]
        /// </summary>
        RightParentheses,
        /// <summary>
        /// Left Brace {
        /// </summary>
        LeftBrace,
        /// <summary>
        /// Right Brace }
        /// </summary>
        RightBrace,
        /// <summary>
        /// New Line
        /// </summary>
        NewLine,
        /// <summary>
        /// Dot .
        /// </summary>
        Dot,
        /// <summary>
        /// Start String "
        /// </summary>
        StringStart,
        /// <summary>
        /// End String "
        /// </summary>
        StringEnd,
        /// <summary>
        /// Space
        /// </summary>
        Space,
        /// <summary>
        /// Punctuation (,:...)
        /// </summary>
        Punctuation,
        /// <summary>
        /// Operator (+,-,*,/,%..)
        /// </summary>
        Operator,
        /// <summary>
        /// Comma (,)
        /// </summary>
        Comma,
        /// <summary>
        /// Colon (:)
        /// </summary>
        Colon,
        /// <summary>
        /// Comment ($* comment *$)
        /// </summary>
        Comment,
        /// <summary>
        /// eof
        /// </summary>
        EOF
    }

}