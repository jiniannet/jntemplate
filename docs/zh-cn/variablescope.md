# VariableScope 接口
命名空间: JinianNet.JNTemplate
程序集:JinianNet.JNTemplate.dll

 引擎变量作用域


```csharp
public class VariableScope
```

## 属性
属性|说明|
:--|:--|
Parent|父域
Count|变量总数量


## 方法
方法|说明|
:--|:--|
Clear(bool)|清除变量
Clear()|清除当前域变量
ContainsKey(string)|是否包含KEY
GetType(string)|获取变量类型
Set&lt;T&gt;(string, T)|设置变量
Set(string, object, Type)|设置变量
SetElement(string, element)|设置变量元素