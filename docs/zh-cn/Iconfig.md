# IConfig 接口
命名空间: JinianNet.JNTemplate.Configuration
程序集:JinianNet.JNTemplate.dll

Jntemplate引擎的配置与使用入口


```csharp
public interface IConfig
```

## 属性
属性|说明|
:--|:--|
ResourceDirectories|模板目录
Charset|字符编码，默认`utf-8`
TagPrefix|标签前缀，默认`${`
TagSuffix|标签后缀，默认`}`
TagFlag|简写标签标志，默认`$`
ThrowExceptions|运行时出错是否抛出异常(仅对解释型引擎生效)
StripWhiteSpace|是否处理标签前后空白字符
IgnoreCase|是否忽略大小写
EnableTemplateCache|是否启用模板缓存，建议启用(仅对解释型引擎生效)
DisableeLogogram|是否禁用标签 简写
IgnoreCase|是否忽略大小写
Loader|资源加载器
TagParsers|标签解析器


## 方法
方法|说明|
:--|:--|
ToDictionary|将配置转换成字典(扩展方法)
Set&lt;T&gt;(string, T)|设置呈现对象，可以在模板中调用
SetStaticType(string, Type)|设置呈现类型，可以在模板中调用该类型的静态属性或者方法

Render(TextWriter)|呈现模板内容