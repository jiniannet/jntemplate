
# JNTemplate
[![Build Status](https://travis-ci.org/jiniannet/jntemplate.svg?branch=master)](https://travis-ci.org/jiniannet/jntemplate)
[![GitHub stars](https://img.shields.io/nuget/v/JinianNet.JNTemplate.svg)](https://www.nuget.org/packages/JinianNet.JNTemplate/)
[![GitHub stars](https://img.shields.io/github/stars/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/stargazers)
[![GitHub license](https://img.shields.io/badge/license-Mit-blue.svg)](https://raw.githubusercontent.com/jiniannet/jntemplate/master/License.txt)
[![GitHub issues](https://img.shields.io/github/issues/jiniannet/jntemplate.svg)](https://github.com/jiniannet/jntemplate/issues)

[English](https://github.com/jiniannet/jntemplate/blob/master/README.md) | [中文](https://github.com/jiniannet/jntemplate/blob/master/README-zh-CN.md)

### What is JNTemplate?

JNTemplate is fast, lightweight, extensible .net template engine for generating html, xml, sql, or any other formatted text output. 

Special placeholders in the template allow writing code similar to c# syntax. Then the template is passed data to render the final document.


### Installation

Install and update using NuGet:
```
PM> Install-Package JinianNet.JNTemplate

```
or

```
> dotnet add package JinianNet.JNTemplate
```


### Quickstart

**Basics**

Rendering a basic html template with a predefined data model.

c# code

```csharp
var template = Engine.LoadTemplate(@"c:\wwwroot\view\index.html"); ;
template.Set("name", "jntemplate");
var result = template.Render(); 
```
index.html

```html
<!DOCTYPE html>
<html>
<body>
  <h1>Hello, ${name}</h1>
</body>
</html>
```

output:

```html
<!DOCTYPE html>
<html>
<body>
  <h1>Hello, jntemplate</h1>
</body>
</html>
```

**Iteration**

Iteration is achieved by using the foreach binding on the element you wish to iterate.

c# code

```csharp
var template = Engine.LoadTemplate(@"c:\wwwroot\view\view.html"); ;
template.Set("list", new string[] { "github","jntemplate" });
var result = template.Render(); 
```
view.html

```html
<ul>
${foreach(name in list)}
	<li>${name}</li>
${end}
</ul>
```

output:

```html
<ul>
	<li>github</li>
	<li>jntemplate</li>
</ul>
```

**Configuration**

You can configure JNTemplate with the `EngineConfig` class.
```csharp
Engine.Configure((conf)=>{
// .. configure your instance
});
```

### Links

- Website: https://www.jiniannet.com
- Documentation: https://docs-en.jiniannet.com
- Code: https://github.com/jiniannet/jntemplate


### Licenses
MIT