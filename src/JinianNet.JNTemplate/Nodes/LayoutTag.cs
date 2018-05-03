/********************************************************************************
 Copyright (c) jiniannet (http://www.jiniannet.com). All rights reserved.
 Licensed under the MIT license. See licence.txt file in the project root for full license information.
 ********************************************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    public class LayoutTag : LoadTag
    {
        //public override object Parse(TemplateContext context)
        //{
        //    Object path = this.Path.Parse(context);
        //    base.LoadResource(path, context);
        //    string result =  base.Parse(context).ToString();
        //}

        //public override void Parse(TemplateContext context, TextWriter write)
        //{

        //    base.Parse(context, write);
        //}

        protected override Tag[] ReadTags()
        {
            Tag[] tags = base.ReadTags();
            for(Int32 i = 0; i < tags.Length; i++)
            {

            }
        }
    }
}
