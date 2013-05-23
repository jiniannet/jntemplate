/*****************************************************
 * 本类库的核心系 JNTemplate
 * 作者：翅膀的初衷 QQ:4585839
 * Mail: i@Jiniannet.com
 * 网址：http://www.JiNianNet.com
 *****************************************************/
using System;
using System.IO;

namespace JinianNet.JNTemplate
{
    public interface IEngine
    {
        //void Render(TemplateContext context, TextWriter writer);
        ITemplate CreateTemplate();
    }
}
