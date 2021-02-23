
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
PM&gt; Install-Package JinianNet.JNTemplate

```
or

```
&gt; dotnet add package JinianNet.JNTemplate
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
default.html

```html
&lt;!DOCTYPE html&gt;
&lt;html&gt;
&lt;body&gt;
  &lt;h1&gt;Hello, $name&lt;/h1&gt;
&lt;/body&gt;
&lt;/html&gt;
```

output:

```html
&lt;!DOCTYPE html&gt;
&lt;html&gt;
&lt;body&gt;
  &lt;h1&gt;Hello, jntemplate&lt;/h1&gt;
&lt;/body&gt;
&lt;/html&gt;
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
&lt;ul&gt;
${foreach(name in list)}
	&lt;li&gt;${name}&lt;/li&gt;
${end}
&lt;/ul&gt;
```

output:

```html
&lt;ul&gt;
	&lt;li&gt;github&lt;/li&gt;
	&lt;li&gt;jntemplate&lt;/li&gt;
&lt;/ul&gt;
```

**Configuration**

You can configure JNTemplate with the `EngineConfig` class.
```csharp
Engine.Configure((conf)=&gt;{
// .. configure your instance
});
```

### Links
Website: https://www.jiniannet.com
Documentation: https://docs-en.jiniannet.com
Code: https://github.com/jiniannet/jntemplate


### Licenses
MIT