# Runtime 类
命名空间: JinianNet.JNTemplate
程序集:JinianNet.JNTemplate.dll

引擎运行时相关数据与配置


```csharp
public sealed class Runtime
```

## 属性
属性|说明|
:--|:--|
ResourceDirectories|模板资源搜寻目录
Loader|资源加载器
Encoding|当前资源编码格式
Cache|系统内存缓存
Templates|已编译模板
Encoding|当前资源编码



## 方法
方法|说明|
:--|:--|
SetLoader(IResourceLoader)|设置资源加载器
AppendResourcePath(string)|增加资源目录
GetEnvironmentVariable(string)|获取环境变量
SetEnvironmentVariable(IResourceLoader)|设置环境变量
RegisterTagParser(ITagParser,int)|注册标签分析器
Parsing(TemplateParser,TokenCollection)|标签分析

