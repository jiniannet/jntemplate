# 快速开始

1. 我们可以通过 `Engine.LoadTemplate(...)`快速从一个文件来创建一个模板，也可以通过 `Engine.CreateTemplate(...)`从指定模板内容创建模板。
2. 通过`ITemplate.Set(...)` 可以为模板赋值
3. 调用 `ITemplate.Rebder(...)` 获取呈现结果

**示例**：

模板文件default.html

```html
<p>你好，$name</p>
```

后台代码

```csharp
var template = Engine.LoadTemplate("c:\\default.html");
template.Set("name","jntemplate");
var result = template.Render();
```

或者

```csharp
var template = Engine.CreateTemplate("<p>你好，$name</p>");
template.Set("name","jntemplate");
var result = template.Render();
```

output:

```html
<p>你好，jntemplate</p>
```




**说明**
***

我们可以通过扩展方法`template.Render()`直接获取生成的文本内容，也可以通过`template.Render(TextWriter:writer)`直接写入输出流，如：

- 在控制台程序中

```csharp
var template = Engine.CreateTemplate("<p>你好，$name</p>");
template.Set("name","jntemplate");
template.Render(Console.Out);
```

- 在WebForm中

```csharp
var template = Engine.CreateTemplate("<p>你好，$name</p>");
template.Set("name","jntemplate");
template.Render(Response.Output);
```