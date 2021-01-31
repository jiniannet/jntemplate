
# JNTemplate
[![Build Status](https://travis-ci.org/jiniannet/jntemplate.svg?branch=master)](https://travis-ci.org/jiniannet/jntemplate)
[![GitHub stars](https://img.shields.io/nuget/v/JinianNet.JNTemplate.svg)](https://www.nuget.org/packages/JinianNet.JNTemplate/)
[![GitHub stars](https://img.shields.io/github/stars/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/stargazers)
[![GitHub license](https://img.shields.io/badge/license-Mit-blue.svg)](https://raw.githubusercontent.com/jiniannet/jntemplate/master/License.txt)
[![GitHub issues](https://img.shields.io/github/issues/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/issues)

[English](https://github.com/jiniannet/jntemplate/blob/master/README.md) | [中文](https://github.com/jiniannet/jntemplate/blob/master/README-zh-CN.md)

### JNTemplate 是什么

极念模板引擎(JNTemplate)是一款完全国产的基于C#语言开发的跨平台的文本解析引擎（模板引擎），它能生成任何基于文本的内容，包括且不限于html,xml,css等,让前端展示与后端代码逻辑分离。同时，它也提供了一个在WebForm与Razor之外的选择！

JNTemplate所有代码全部开源，且具有最小的依赖关系，能轻松实现迁移与跨平台。同时，在满足我们开源协议的前提下，大家可以自由使用，分发，和用于商业目的（具体见License.txt）。

从2.0版本开始，引擎全面升级为编译型模板引擎，在性能上得到了更大的提升。

### 特点:
- 易于学习使用：有一定前后端基础，最快可以10分钟上手
- 更方便的扩展：能轻松实现自定义标签（需要有一定IL基础）
- 更快解析速度：高效的词法语法分析器，模板预编译，让运行速度接近原生体验。
- 完全自由免费：允许商用且无须担心费用，所有代码完全开源，不依赖任何第三方DLL

### 快速开始

**您可以从NuGet获取我们的发行版本**
```
PM> Install-Package JinianNet.JNTemplate

```
或者（NET CORE）
```
> dotnet add package JinianNet.JNTemplate
```


**也可以克隆源代码自行构建**
```

git clone https://github.com/jiniannet/jntemplate.git
```

Windows:克隆完成后,运行 build/build.bat
linux:克隆完成后,运行 build/build.sh

  
  
**配置**

你可以使用Engine.Configure()方法来对 JNTemplate进行配置.可配置项包括是否区分大小写，标签符号，模板工作目录，全局对象等：
```csharp
Engine.Configure((conf)=>{
    // .. 配置你的具体参数
});
```

**简单示例**

模板代码(index.html):
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>JNTemplate demo</title>
  </head>
  <body>
      hello,$name!
  </body>
</html>


```

c# 代码:
csharp
```
var template = Engine.LoadTemplate("C:\\wwwwroot\index.html");
\\在配置中配置了模板目录的情况下，可以省略目录，如：Engine.LoadTemplate("index.html");
template.Set("name", "JNTemplate");
template.Render(TextWriter); 
```

输出结果:
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>JNTemplate demo</title>
  </head>
  <body>
      hello,JNTemplate!
  </body>
</html>


```

### API文档
[docs.jiniannet.com](https://docs.jiniannet.com)


### 授权
MIT 详细请查看 License.txt(1.4以下版本为apache 2.0)
  
### 联系方式：
- Email:i@jiniannet.com
- 交流Q群:5089240 欢迎加入
