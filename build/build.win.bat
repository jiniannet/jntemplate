@echo off

echo 开始生成 JNTemplate ...
set fdir=%WINDIR%\Microsoft.NET\Framework
set key_param=/warn:3 /nologo /o /recurse:*.cs

cd ../src/JinianNet.JNTemplate
rd obj\Debug /s /q
rd obj\Release /s /q
del obj\*.* /q

if exist ../../tool/jiniannet.snk set key_param=/keyfile:../../tool/jiniannet.snk %key_param%

if exist %fdir%\v3.5\csc.exe goto BUILD20
if exist %fdir%\v4.0.30319\csc.exe goto BUILD40
goto SUCCESS

:BUILD20
if not exist ..\..\lib\net20 md ..\..\lib\net20
%fdir%\v3.5\csc.exe /target:library /out:../../lib/net20/JinianNet.JNTemplate.dll /doc:../../lib/net20/JinianNet.JNTemplate.xml %key_param% /define:NET20,NOTDNX
echo jntemplate for .net v2.0 生成完毕

:BUILD40
if not exist ..\..\lib\net40 md ..\..\lib\net40
%fdir%\v4.0.30319\csc.exe /target:library /out:../../lib/net40/JinianNet.JNTemplate.dll /doc:../../lib/net40/JinianNet.JNTemplate.xml %key_param% /define:NET40,NOTDNX
echo jntemplate for .net v4.0 生成完毕

:SUCCESS

cd ..
cd ..
cd build

if exist ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll del ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll
copy ..\lib\net20\JinianNet.JNTemplate.dll ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll
echo 生成完成...
pause