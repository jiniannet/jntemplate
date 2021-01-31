# Hello World

本文介绍如何创建和运行“Hello World!” Jntemplate 应用。

## 创建应用程序

首先，在计算机上下载并安装 .NET SDK。

然后，打开某一终端，如 PowerShell、命令提示符或 Bash 。 输入以下 dotnet 命令，创建并运行 C# 应用程序：

```bash
dotnet new console --output sample
cd sample
dotnet add package JinianNet.JNTemplate
notepad Program.cs
```

输入如下代码：

```csharp
using System;
using JinianNet.JNTemplate;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var template = Engine.CreateTemplate("Hello $name!");
            template.Set("name","World");
            template.Render(Console.Out);
        }
    }
}

```

关闭记本事并保存，然后在刚才的终端继续运行：

```bash
dotnet run
```

终端将输出最终结果：Hello World!

恭喜，你的第一个jntemplate应用已经完成了，快来试试吧。


## 后续步骤

欲了解更多用法，可以观看 [标签与语法](zh-cn/tag.md) 章节。
