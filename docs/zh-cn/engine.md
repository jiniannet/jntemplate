# Engine 类
命名空间: JinianNet.JNTemplate
程序集:JinianNet.JNTemplate.dll

Jntemplate引擎的配置与使用入口


```csharp
public sealed class Engine
```

## 属性
属性|说明|
:--|:--|
Version|当前引擎版本
EnableCompile|是否启用编译模式

## 方法
方法|说明|
:--|:--|
Configure(Action&lt;IConfig&gt;)|通过Action&lt;IConfig&gt;初始化配置
Configure(Action&lt;IConfig, VariableScope&gt;)|通过Action&lt;IConfig, VariableScope&gt;初始化配置
Configure(IConfig&gt;)|通过IConfig初始化配置
Configure(IConfig conf, VariableScope scope)|通过IConfig与VariableScope初始化配置
Configure(IDictionary&lt;string, string&gt; conf, VariableScope scope)|配过字典与VariableScope初始化配置
Precompiled(string, string,Action&lt;CompileContext&gt;)|预编译指定内容
Precompiled(FileInfo[], Action&lt;CompileContext&gt;)|预编译多个文件
CreateContext()|创建模板上下文
CreateTemplate(string)|从指定内容创建模板
CreateTemplate(string,string)|从指定内容创建模板
LoadTemplate(string)|从指定文件创建模板
LoadTemplate(string,string)|从指定文件创建模板
Register&lt;T&gt;(ITagParser,Func&lt;ITag, CompileContext, MethodInfo&gt;,Func&lt;ITag, CompileContext, Type&gt;)|注册一个编译型标签
Register&lt;T&gt;(ITagParser,Func&lt;ITag, TemplateContext, object&gt;)|注册一个解释型标签