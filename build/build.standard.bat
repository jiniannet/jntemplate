@echo off

echo 开始生成 JNTemplate ...
cd ../src/JinianNet.JNTemplate
if exist ..\..\tool\jiniannet.snk copy ..\..\tool\jiniannet.snk jiniannet.snk
echo 正在还原依赖项
dotnet restore JinianNet.JNTemplate_Standard.csproj
echo 环境设置
if not exist ..\..\lib\netstandard1.3 md ..\..\lib\netstandard1.3
ren .\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
 
echo 开始build
dotnet build JinianNet.JNTemplate_Standard.csproj --configuration Release --output ..\..\lib\netstandard1.3 --framework netstandard1.3
goto GOEND

:GOEND
ren .\Properties\AssemblyInfo.cs.build.tmp AssemblyInfo.cs
cd ..
cd ..
cd build
echo build完成...
pause