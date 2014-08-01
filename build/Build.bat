@echo off

echo 开始生成 JNTemplate ...

set fdir=%WINDIR%\Microsoft.NET\Framework

cd ../src/JinianNet.JNTemplate

%fdir%\v3.5\csc.exe /target:library /out:../../lib/2.0/JinianNet.JNTemplate.dll /doc:../../lib/2.0/JinianNet.JNTemplate.xml /keyfile:../../tool/jiniannet.snk /warn:3 /nologo /o /recurse:*.cs

echo v2.0.50727 生成完毕

%fdir%\v4.0.30319\csc.exe /target:library /out:../../lib/4.0/JinianNet.JNTemplate.dll /doc:../../lib/4.0/JinianNet.JNTemplate.xml /keyfile:../../tool/jiniannet.snk /warn:3 /nologo /o /recurse:*.cs

echo v4.0.30319 生成完毕

cd ..

cd ..

cd build

echo 生成完成...

pause