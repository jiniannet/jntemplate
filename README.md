# JNTemplate  [![Build Status](https://travis-ci.org/jiniannet/jntemplate.svg?branch=master)](https://travis-ci.org/jiniannet/jntemplate)

## JNTemplate 是什么

极念模板引擎(JNTemplate)是一款C#语言开发的跨平台的文本解析引擎（模板引擎），它能生成任何基于文本的内容，包括且不限于html,xml,css等,让前端展示与后端代码逻辑分离。同时，它也提供了一个在WebForm与Razor之外的选择！

JNTemplate具有简单，易用，高效等特点，拓展十分方便！而且它使用的是对商业更加友好的Apache license 2.0协议开源，在满足该协议的前提下，大家可以自由使用，分发，甚至可以用于商业目的（具体见License.txt）。

### 为什么要使用JNTemplate
1. 简单易用 有一定 c#/java/javascript 语法基础的用户只需要10分钟就可以上手使用
2. 轻便高效 使用了更高效的解析机制，引擎执行更快。
3. 自由拓展 可以十分方便的自由扩展功能，灵活性更强。
4. 免费开源 可以免费使用而无须支付任何费用，只需要在代码中保留我们的署名即可。

### 如何使用JNTemplate
```bash
ITemplate template = new Template("hello,$name!");//创建Template实例
template.Context.CurrentPath = "D:\\templets\\"; //指定当前模板目录
template.Context.TempData["name"] = "world";//设置模板数据
template.Render(Response.Output);//呈现解析结果
```


### 如何下载JNTemplate

1. 官网下载:[http://bbs.jiniannet.com/forum.php?mod=viewthread&tid=2&extra=page%3D1](http://bbs.jiniannet.com/forum.php?mod=viewthread&tid=2&extra=page%3D1)
2. 源代码克隆:[https://github.com/jiniannet/jntemplate.git](https://github.com/jiniannet/jntemplate.git)


```bash
git clone https://github.com/jiniannet/jntemplate.git
```


### JNTemplate帮助

1. 开源协议：[https://github.com/jiniannet/jntemplate/blob/master/License.txt](https://github.com/jiniannet/jntemplate/blob/master/License.txt)
2. 开源主页：[https://github.com/jiniannet/jntemplate/](https://github.com/jiniannet/jntemplate/)
3. 官方网站：[http://www.jiniannet.com](http://www.jiniannet.com)
4. 交流社区：[http://bbs.jiniannet.com](http://bbs.jiniannet.com)
5. 国内代码库：[http://git.oschina.net/jiniannet/jntemplate](http://git.oschina.net/jiniannet/jntemplate)

### 联系方式：
1. Email:i@jiniannet.com
2. 交流Q群:5089240 欢迎加入
