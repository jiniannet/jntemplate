@echo off
del ..\src\JinianNet.JNTemplate.Test\dll\JinianNet.JNTemplate.dll
rd ..\lib\2.0 /s /q
rd ..\lib\4.0 /s /q
rd ..\lib\netcoreapp1.1 /s /q
del ..\src\JinianNet.JNTemplate.Test\html\*.html
del ..\src\JinianNet.JNTemplate.Test\html\*.txt
if exist ..\TestResults rd ..\TestResults /s /q
echo 清理完成
pause