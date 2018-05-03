@echo off

echo 开始生成 JNTemplate ...
cd ../src/JinianNet.JNTemplate
if exist ..\..\tool\jiniannet.snk copy ..\..\tool\jiniannet.snk jiniannet.snk
echo 正在还原依赖项
dotnet restore JinianNet.JNTemplate_core.csproj
echo 环境设置
if not exist ..\..\lib\netcoreapp1.1 md ..\..\lib\netcoreapp1.1
if not exist ..\..\lib\netcoreapp2.0 md ..\..\lib\netcoreapp2.0
ren .\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp

:cmdinput
echo 开始生成 请输入您要生成的版本号（需要先修改JinianNet.JNTemplate_core.csproj 文件以保存版本号一致）
set /p cmdtext=请输入（1.1 or 2.0 or exit ）：
 
if /i "%cmdtext%"=="1.1" goto BUILD11
if /i "%cmdtext%"=="2.0" goto BUILD20
if /i "%cmdtext%"=="exit" goto GOEND 
echo 输入错误!请重新输入！
goto cmdinput

:BUILD11
echo 开始build1.1
dotnet build JinianNet.JNTemplate_core.csproj --configuration Release --output ..\..\lib\netcoreapp1.1 --framework netcoreapp1.1 
dotnet build JinianNet.JNTemplate_core.csproj --configuration DEBUG --output ..\..\lib\netcoreapp1.1 --framework netcoreapp1.1 
goto GOEND

:BUILD20
echo 开始build2.0
dotnet build JinianNet.JNTemplate_core.csproj --configuration Release --output ..\..\lib\netcoreapp2.10 --framework netcoreapp 2.0 
dotnet build JinianNet.JNTemplate_core.csproj --configuration DEBUG --output ..\..\lib\netcoreapp2.10 --framework netcoreapp 2.0 
goto GOEND

:GOEND
ren .\Properties\AssemblyInfo.cs.build.tmp AssemblyInfo.cs
cd ..
cd ..
cd build
echo build完成...
pause