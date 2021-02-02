# ITemplate 接口
命名空间: JinianNet.JNTemplate
程序集:JinianNet.JNTemplate.dll

模板对象


```csharp
public interface ITemplate
```

## 属性
属性|说明|
:--|:--|
Context|模板上下文
Path|模板文件路径
TemplateContent|模板内容
TemplateKey|模板标识，必须唯一


## 方法
方法|说明|
:--|:--|
Render(TextWriter)|呈现模板内容
Render()|呈现模板内容(扩展方法)
Set&lt;T&gt;(string, T)|设置呈现对象，可以在模板中调用
SetStaticType(string, Type)|设置呈现类型，可以在模板中调用该类型的静态属性或者方法