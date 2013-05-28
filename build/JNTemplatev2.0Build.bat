@echo off
echo 开始生成...
cd ..
cd bin
rd /s /q 2.0
md 2.0
cd 2.0
md Release
md Debug
cd ../../src
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /target:library /out:../bin/2.0/Release/JinianNet.JNTemplate.dll /warn:0 /nologo /o /recurse:*.cs
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /target:library /out:../bin/2.0/Debug/JinianNet.JNTemplate.dll /warn:0 /nologo /Debug /recurse:*.cs
cd ..\build
echo 生成完毕...
pause