@echo off

echo 开始生成 JNTemplate ...
cd ../src/JinianNet.JNTemplate
echo 正在还原依赖项
dotnet restore JinianNet.JNTemplate_core.csproj
echo 环境设置
if not exist ..\..\lib\netcoreapp1.1 md ..\..\lib\netcoreapp1.1
if not exist ..\..\lib\netcoreapp2.0 md ..\..\lib\netcoreapp2.0
ren .\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
echo 开始build
dotnet build JinianNet.JNTemplate_core.csproj --configuration Release --output ..\..\lib\netcoreapp2.0 --framework netcoreapp2.0 
ren .\Properties\AssemblyInfo.cs.build.tmp AssemblyInfo.cs
cd ..
cd ..
cd build
echo build完成...
pause