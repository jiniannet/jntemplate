# 复合标签
复合标签通常由一到多个基本标签组合成。


## 布局标签
Layout Tag: 语法：`${ layout("路径") }`
布局标签类似于ASP.NET的母版页。必须出现在页面的第一行。

**示例 父页面(parent.html):**
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>JNTemplate demo</title>
  </head>
  <body>
      ${body}
  </body>
</html>
```
**示例 子页面(index.html):**
```html
$layout("layout.txt")
<p>i'm child!</p>
```

**输出结果:**
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>JNTemplate demo</title>
  </head>
  <body>
      <p>i'm child!</p>
  </body>
</html>
```

!> 支持版本 1.4 +，layout标签必须在在模板的最顶行申明。


## IF标签
IF Tag，语法：

```js
$if(条件1)
..
$elseif(条件2)
..
$else
..
$end
```
用来处理逻辑代码，必须以End标签结束。多个条件可以配合Elseif标签使用。条件里可以是逻辑算运标签，也可以是单个对象，如果是单个对象时，按以下规则计算：

1. 数字：0 false 否则 true
2. 字符串：null或者"" false 否则 true
3. 对象： null或 false 否则 true


示例
```csharp
var  templateContent = @"
${if(value>5)}
    value大于5
${elif(value<5)}
    value小于5
${else}
    value等于5
${end}
";
var template = Engine.CreateTemplate(templateContent);
template.Set("value", 10);
var result = template.Render();
Assert.AreEqual("value大于5", result);
```


## Elseif标签
Elseif Tag， 语法：`${ elseif("条件") }`

Elseif标签是IF标签的子标签，只能和IF一起使用，不能单独存在，v1.4.0以上版本也可以简写成 `$elif(条件)`。用法参见：`IF`标签。



## Else标签
Else Tag，语法：`${ else }`

Else标签是IF标签的子标签，只能和IF一起使用，不能单独存在，用法参见：`IF`标签。


## 迭代标签
Foreach Tag， 语法：

```js
$foreach(子对象 in 循环对象)，
...
$end
```

其中循环对象必须实现`Enumerable`接口。

注意：在循环体内的对象（即foreach与end之间的内容 ）在标签结束后无法继续访问，在标签体内使用SET标签设定的值在离开该标签作用域后也会失效。

1.4以上版本`foreach . in .`可以简写成`for . in .`

**示例**
```csharp
var  templateContent = @"
<ul>
$foreach(i in list)
<li>$i</li>
${end}
</ul>
";
var template = Engine.CreateTemplate(templateContent);
template.Set("list", new char[] { 'j', 'n', 't', 'e', 'm', 'p', 'l', 'a', 't', 'e' });
var result = template.Render();
```

**输出:**
```html
<ul>
<li>j</li>
<li>n</li>
<li>t</li>
<li>e</li>
<li>m</li>
<li>p</li>
<li>l</li>
<li>a</li>
<li>t</li>
<li>e</li>
</ul>
```

!> 对象必须实现`IEnumerable`，注意作用域。


## 循环标签
For Tag:， 语法：
```js
$for(初始化码; 逻辑代码; 增量代码)
...
$end
```

循环标签必须以end结束。注意：在循环体内的对象（即for与end之间的内容 ）在标签 结束后无法再访问,  该标签如果条件永恒为真的话会造成死循环，请慎用。比如`for(i=0;i<1;i--)`

**示例：**
```csharp
var  templateContent = @"
<ul>
$for(i=1;i<4;i=i++)
<li>$i</li>
${end}
</ul>
";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
```

**输出：**
```html
<ul>
<li>1</li>
<li>2</li>
<li>3</li>
</ul>
```

!> 注意条件是否会造成死循环。


## 赋值标签
Set Tag，语法：`$set(名称=值)`

赋值标签用于定义一个变量或者给改变指定变量的值。 如：`$(set id=10)` 或者 `$(set model=GetData())`

**示例：**
```csharp
var templateContent = "$set(id=10)$id";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
Assert.AreEqual("10", render);
```

!> 注意在`for`与`foreach`中使用时，离开标签后改变的值将会恢复原样。


## 加载标签
Load Tag，语法：`$load(路径)`

加载标签用于加载指定的模板页面，并解析模板内容。

**示例 模板文件header.html：**

```html
<p>你好，$name</p>
```

**后台代码**
```csharp
var templateContent = "$load(\"header.html\")";
var template = Engine.CreateTemplate(templateContent);
template.Set("name","jntemplate");
var result = template.Render();
```

**输出:**
```html
<p>你好，jntemplate</p>
```

## 引用标签
Include Tag，语法：`$include(路径)`

引用标签用于引用其它模板页，作用与LoadTag相似，但是不执行解析。

**示例 模板文件header.html：**

```html
<p>你好，$name</p>
```

**后台代码**
```csharp
var templateContent = "$load(\"include.html\")";
var template = Engine.CreateTemplate(templateContent);
template.Set("name","jntemplate");
var result = template.Render();
```

**输出:**
```html
<p>你好，$name</p>
```