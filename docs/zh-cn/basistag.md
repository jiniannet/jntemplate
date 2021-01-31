# 基本标签
有且仅有一个标签，所有代码都包含在标签起始符`${` 与 结束符`}` 之间。

## 变量标签（属性、字段）
Variable Tag，语法：`${ 变量名 }`
- 呈现后台设定的对象,如:`$name`
- 呈现指定对象的属性,如：`$model.UserName`
- 呈现对象的字段，如：`$data.Version`

变量标签属于子标签，当父对象为空时，属于普通变量标签，存在父对象时，为对象属性或者字段。如`$model.UserId`，其中`model`为该标签的父标签，`UserId`为特殊的变量标签，可以是属性也可以是字段(1.x版本默认不支持字段)。


**示例一 呈现变量：**

```csharp
var templateContent = "hello $name";
var template = Engine.CreateTemplate(templateContent);
template.Set("name","jntemplate");
var result = template.Render();
Assert.AreEqual("hello jntemplate", render);
```

**示例二 呈现属性：**

```csharp
var templateContent = "hello $model.Name";
var template = Engine.CreateTemplate(templateContent);
template.Set("model",new SiteInfo {  Name = "jntemplate" });
var result = template.Render();
Assert.AreEqual("hello jntemplate", render);
```

!> 只有编译版本才默认支持字段。


## 函数标签

Functaion Tag ，语法：`${ 函数名(参数1,参数2....) }`。
- 调用后台实例方法
- 调用后台静态方法（2.0+）
- 调用后台委托方法。

函数标签属于子标签，当父对象为空时，只能是委托方法，如`$test(1,2,3)`，父对像不为空时不做限制，如：`${date.ToString("yyyy-MM-dd")}`。

**示例一 调用实例方法：**
```csharp
var templateContent = "$fun.Test(\"your input\")";
var template = Engine.CreateTemplate(templateContent);
template.Set("fun", new TestHelper());
var result = template.Render();
Assert.AreEqual("您输入的参数是有：your input", result);
```

**示例二 调用委托方法：**
```csharp
var templateContent = "$add(8,-2)";
var template = Engine.CreateTemplate(templateContent);
template.Set<Func<int, int, int>>("add", (x, y) =>
{
    return x + y;
});
var result = template.Render();
Assert.AreEqual("6", result);
```

**示例三 调用静态方法：**
```csharp
var templateContent = "${string.Concat(\"str1\",\"str2\")}";
var template = Engine.CreateTemplate(templateContent);
template.SetStaticType("string", typeof(string));
var result = template.Render();
Assert.AreEqual("str1str2", result);
```

!> v1.1 - v1.3版本委托只能是`FuncHandler`，1.4以上版本可以为任意委托。

## 索引标签
IndexValue Tag，语法`${ 父对象[索引]}`
- 访问数组,List等对象
- 访问字典
- 其它任意实现了索引的对象

索引标签属于子标签，父对象不能为空，比如：`$[i]`是错误的，只能为`$list[i]`。

**示例一 字典：**
```csharp
var templateContent = "$dict[\"name\"]";
var template = Engine.CreateTemplate(templateContent);
var dic = new System.Collections.Generic.Dictionary<string, string>();
dic["name"] = "jntemplate";
dic["age"] = "1";
template.Set("dict",dict);
var result = template.Render();
Assert.AreEqual("jntemplate", render);
```

**示例二 数组：**
```csharp
var templateContent = "$arr[0]";
var template = Engine.CreateTemplate(templateContent);
template.Set("arr",new int[] { 1, 2, 3 });
var result = template.Render();
Assert.AreEqual("1", render);
```

!> 1.4 以上版本才支持索引。1.4以下版本可以通过变量标签访问，如`$row["name"]`在老版本中可以写成`$row.name`或者`$row.get_Item("name")`

## 逻辑表达式标签

Logic Tag，语法：`${ 逻辑表达式 }`。
逻辑表达式标签属于基本标签，支持进行逻辑运算，通常与IF标签一起使用(具体见IF标签)。
支持以下逻辑运算符：大于(`>`)，小于(`<`)，大于等于(`>=`)，小于等于(`<=`)，等于(`==`)，不等于(`!=)`，或者(`||)`， 并且(`&&`)。

**示例**
```csharp
var templateContent = "${3==8}";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
Assert.AreEqual("False", result);
```

## 算术表达式标签
Arithmetic　Tag， 语法：`${ 算术表达式 }`。
算术表达式标签属于基本标签， 用于算术运算。
支持 加（`+`），减（`-`），乘（`*`），除（`/`），取余（`%`）五种运算符与小括号。

**示例**
```csharp
var templateContent = "${(8+2)*10)}";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
Assert.AreEqual("100", result);
```


## 组合标签
Reference Tag， 语法：`${ 标签1.标签2 }`

组合标签属于基本标签， 通常组合标签由字段，属性，索引，方法几个标签共同组合而成，但是依然在同一个标签内。

比如`${ test().Id }`就是由函数标签（`test()`）与属性（`Id`）组成。

**示例**
```csharp
var templateContent = "$date.Year.ToString().Length";
var template = Engine.CreateTemplate(templateContent);
template.Set("date",DateTime.Now);
var result = template.Render();
Assert.AreEqual("4", render);
```



## 布尔标签
Boolean Tag，语法：`${ true/false }`
值只能是下列之一：
- true
- false

布尔标签属于特殊标签，通常配合逻辑表达式标签或if标签使用。

**示例：**
```csharp
var templateContent = "${true}";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
Assert.AreEqual("True", result);
```

## 结束标签
End Tag，语法：`${ end }`

结束标签属于特殊标签，通常用于表示复合标签的结束，需要和其它标签一起配合使用，不能单独存在。

具体请参见`for` 标签，`foreach`标签或者`if`标签。


## 占位标签
Body Tag:，语法：`${ body }`

占位标签属于特殊标签，用作Layout标签占位符。只能配合Layout标签使用，在实际渲染时，子页面内容会替代该标签位置。具体用法参见`Layout`标签。


## Null标签
Null Tag，语法：`${null}`

Null标签属于特殊标签，表示一个空对象。一般配合逻辑表达式或IF标签使用，比如`$if(model == null)`


## 数字标签

Number Tag，语法：`${数字}`

数字标签属于特殊标签，表示一个小数或者整数，默认小数会解析为double类型，整体会解析为int32, 超出int32范围才会解析为int64，如`${64.0}`


## 操作符标签
Operator Tag，语法：`${操作符}`

操作符标签属于特殊标签, 不能单独使用，只能用在逻辑表达式或者算术表达式中，可以是逻辑运算符，也可以是算术运算符，具体用法参见逻辑表达式标签（LogicTag）与算术表达式标签（ArithmeticTag）。


## 文本标签
Text Tag，语法：无

文本标签属于特殊标签，所有未被包含在标签符号内的内容都是文本标签，表示一段不是标签的静态文本。如：`<p>${name}</p>`，在此段文本中，`<p>`与`</p>`都属于TextTag。

文本标签属静态内容，不需要解析。


## 字符串标签
String Tag，语法：`${ "字符串" }`

字符串标签属于特殊标签，用双引号包含起来的字符串。如：`${"this is string"}`

**示例：**
```csharp
var templateContent = "${\"this is string\"}";
var template = Engine.CreateTemplate(templateContent);
var result = template.Render();
Assert.AreEqual("this is string", render);
```

## 注释标签
Comment Tag，语法：`$* 注释内容 *$`

注释标签属于特殊标签，用作代码注释，注释内容在实际输出时不会被渲染。

**示例:**
```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <title>demo</title>
  </head>
  <body>
      $* 这是注释，实际不会被渲染 *$
  </body>
</html>
```
