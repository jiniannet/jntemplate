cd ..
cd bin
rd /s /q 4.0
md 4.0
cd ..
cd src
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\csc /target:library /out:../bin/4.0/JinianNet.JNTemplate.dll /warn:0 /nologo /o /recurse:*.cs
cd ..\build
echo Éú³ÉÍê±Ï...