# 获取

通过以下方式可以获取到Jntemplate

##  通过NuGet获取发行版本
***

- 在Visual Studio中获取
打开Visual Studio，新建一个项目，点击菜单栏=>项目=>管理NUGET程序包(Alt+G,N),搜索JNTemplate

- 在命令行获取

```bash
> Install-Package JinianNet.JNTemplate

```

或者（NET CORE）

```bash
> dotnet add package JinianNet.JNTemplate
```


## 通过源码自行构建
***
- WINDOWS:
```batch
> git clone https://gitee.com/jiniannet/jntemplate.git
> build/build.bat
```

- LINUX:
```bash
> git clone https://gitee.com/jiniannet/jntemplate.git
> cd build
> ./build.sh
```

## 仓库镜像地址
- 国内：[码云(gitee)](https://gitee.com/jiniannet/jntemplate)
- 国际：[github](https://github.com/jiniannet/jntemplate)

## 建议
***
1. 我们强烈建议用户通过nuget包来获取我们的发行版本，这样能及时的发现版本更新及获得更好的技术支持。
2. 可通过PublicKeyToken：2b90a65531efdba4 来判断该发行版本是否是官方发行版。