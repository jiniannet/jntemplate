@echo off

set fdir=%WINDIR%\Microsoft.NET\Framework64

if not exist %fdir% (
	set fdir=%WINDIR%\Microsoft.NET\Framework
)

set msbuild=%fdir%\v2.0.50727\msbuild.exe

%msbuild% ..\src\JinianNet.JNTemplate\JinianNet.JNTemplate.V2005.csproj /p:Configuration=Release /t:Rebuild /p:OutputPath=..\..\bin\4.0\Release

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

%msbuild% ..\src\JinianNet.JNTemplate\JinianNet.JNTemplate.V2005.csproj /p:Configuration=Debug /t:Rebuild /p:OutputPath=..\..\bin\4.0\Debug

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

pause