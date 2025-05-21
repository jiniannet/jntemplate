# JNTemplate

JNTemplate is fast, lightweight, extensible .net template engine for generating html, xml, sql, or any other formatted text output.

Special placeholders in the template allow writing code similar to c# syntax. Then the template is passed data to render the final document.

## How to use

### Install

```
dotnet add package JinianNet.JNTemplate
```

### Import

```cs
 using JinianNet.JNTemplate;
```

### var

chsare code:

```cs
var text = @"Hello, ${name}";
var template = Engine.CreateTemplate(text);
template.Set("name","jntemplate");
var result = template.Render();
Console.WriteLine(result);
```

output:

```html
Hello, jntemplate
```

### Iteration

```cs
var text = @"
<ul>
${foreach(name in list)}
	<li>${name}</li>
${end}
</ul>
";
var template = Engine.CreateTemplate(text);
template.Set("list", new string[] { "github","jntemplate" });
var result = template.Render();
Console.WriteLine(result);
```

output:

```html
<ul>
  <li>github</li>
  <li>jntemplate</li>
</ul>
```

### Function

```cs
var text = "$test.SayHello(\"jntemplate\")";
var template = Engine.CreateTemplate(text);
template.Set("test", new TestHelper());
var result = template.Render();
Console.WriteLine(result);


public class TestHelper
{
    public string SayHello(string name)
    {
        return "hello " + name;
    }
}

```

output:

```html
hello jntemplate
```

### License

[MIT](./LICENSE.md)
