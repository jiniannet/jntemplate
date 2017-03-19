@echo off
ren ..\src\JinianNet.JNTemplate\Properties\AssemblyInfo.cs.build.tmp AssemblyInfo.cs
ren ..\src\JinianNet.JNTemplate.Test\Properties\AssemblyInfo.cs.build.tmp AssemblyInfo.cs
echo 清理完成，您可以使用VS2015/13/12打开解决方案 JinianNet.JNTemplate.sln 了
pause