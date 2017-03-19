开发注意事项：

1.解决方案与项目说明：
	1.JinianNet.JNTemplate_core.sln(JinianNet.JNTemplate_core) 为.NET CORE项目，适用于VS2017
	2.JinianNet.JNTemplate.sln(JinianNet.JNTemplate) 为.net framework/MONO项目，适用于VS2012/VS2013/VS2015 
	3.VS2015/VS2018无法直接打开本项目，但是运行build/build.win.bat 可以生成基于.net framework 2.0的DLL文件（位于lib/2.0目录），可用于VS2015/VS2018项目开发。
	4.不支持.NET 1.1与.NET 1.0，不支持VS2003
	5.build目录下build.mono.sh用于linux+mono构建，build.win.bat用于win+.net framework构建，build.core.bat用于win+.net core构建

	          
2.版本号规则：
	2.1 版号号组成规则：
		2.1.1 主版本.次版本.内部版本号.修订号
		2.1.2 有极大更新，或者版本无法向下兼容时，主版本号加+1，有较大更新时，次版本号+1，BUG修复后发布时，内部版本号+1，每次代码更新，修订号+1
	2.2 文件版本号：同 程序集版本号 但是 省略修订号

3.引用/依赖规则：
	项目必须只依赖最小的dll(即只引用System),以保证项目的可移植性。

4.参与规则：
	如果您想对jntemplate进行代码贡献，请在开源主页（https://github.com/jiniannet）上fork  dev分支，提交您的代码后，向我们进行 pull request即可！

5.注意事项
	1.如果正式使用，请到nuget下载最新版本或者到gitub下载master分支自行生成。dev分支属于开发分支，虽然功能比较新，但是存在变更的风险，请谨慎使用。
	2.IL部分代码尚未完全确定，请勿使用（2015-12-02）