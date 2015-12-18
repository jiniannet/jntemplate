echo off
echo 开始生成 JNTemplate ...
if not exist ..\lib\2.0 md ..\lib\2.0
if not exist ..\lib\4.0 md ..\lib\4.0
set fdir=%WINDIR%\Microsoft.NET\Framework
cd ../src/JinianNet.JNTemplate
set /p input=是否需要支持字段取值(请填Y或者N,默认为N)： 
if "%input%"=="Y" goto Y 
goto N

:Y
%fdir%\v3.5\csc.exe /target:library /out:../../lib/2.0/JinianNet.JNTemplate.dll /doc:../../lib/2.0/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NEEDFIELD,NET20 /recurse:*.cs
echo jntemplate for .net v2.0(支持 field) 生成完毕
%fdir%\v4.0.30319\csc.exe /target:library /out:../../lib/4.0/JinianNet.JNTemplate.dll /doc:../../lib/4.0/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NEEDFIELD,NET40 /recurse:*.cs
echo jntemplate for .net v4.0(支持 field) 生成完毕
goto LAST

:N

%fdir%\v3.5\csc.exe /target:library /out:../../lib/2.0/JinianNet.JNTemplate.dll /doc:../../lib/2.0/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NET20 /recurse:*.cs
echo jntemplate for .net v2.0 生成完毕
%fdir%\v4.0.30319\csc.exe /target:library /out:../../lib/4.0/JinianNet.JNTemplate.dll /doc:../../lib/4.0/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NET40 /recurse:*.cs
echo jntemplate for .net v4.0 生成完毕
goto LAST 

:LAST
cd ..
cd ..
cd build
if exist ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll del ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll
copy ..\lib\2.0\JinianNet.JNTemplate.dll ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll
echo 生成完成...

pause

exit 


