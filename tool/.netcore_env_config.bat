@echo off
ren ..\src\JinianNet.JNTemplate\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
ren ..\src\JinianNet.JNTemplate.Test\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
rd ..\src\JinianNet.JNTemplate\obj\Debug /s /q
rd ..\src\JinianNet.JNTemplate\obj\Release /s /q
echo 配置完成，您可以使用VS2017打开解决方案 JinianNet.JNTemplate_core.sln 了
pause