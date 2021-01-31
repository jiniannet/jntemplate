
# JNTemplate
[![Build Status](https://travis-ci.org/jiniannet/jntemplate.svg?branch=master)](https://travis-ci.org/jiniannet/jntemplate)
[![GitHub stars](https://img.shields.io/nuget/v/JinianNet.JNTemplate.svg)](https://www.nuget.org/packages/JinianNet.JNTemplate/)
[![GitHub stars](https://img.shields.io/github/stars/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/stargazers)
[![GitHub license](https://img.shields.io/badge/license-Mit-blue.svg)](https://raw.githubusercontent.com/jiniannet/jntemplate/master/License.txt)
[![GitHub issues](https://img.shields.io/github/issues/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/issues)

[English](https://github.com/jiniannet/jntemplate/blob/master/README.md) | [中文](https://github.com/jiniannet/jntemplate/blob/master/README-zh-CN.md)

### What is JNTemplate?

JNTemplate is a .net template engine for generating html, xml, sql, or any other formatted text output.

### Features:
- Easy to learn
- Easy to use
- Easy to expand
- 100% free

### Quickstart

**Get it on NuGet!**
```
PM> Install-Package JinianNet.JNTemplate

```
or
```
> dotnet add package JinianNet.JNTemplate
```




**Building the source**
```

git clone https://github.com/jiniannet/jntemplate.git
```

Windows:After cloning the repository, run build/build.bat

Linux:After cloning the repository, run build/build.sh

  
  
**Configuration**

You can configure JNTemplate with the EngineConfig class.
```csharp
Engine.Configure((conf)=>{
// .. configure your instance
});
```

**Basic Example**

template code(index.html):
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

c# code:

```csharp
var template = Engine.LoadTemplate("C:\\wwwwroot\index.html");
template.Set("name", "JNTemplate");
var result = template.Render(); 
```

output:
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

### API
see: www.jiniannet.com


### Licenses
MIT