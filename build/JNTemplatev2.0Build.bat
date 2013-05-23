cd ..
cd bin
rd /s /q 2.0
md 2.0
cd ..
cd src
C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc /target:library /out:../bin/2.0/JinianNet.JNTemplate.dll /warn:0 /nologo /o /recurse:*.cs
cd ..\build
echo Éú³ÉÍê±Ï...