nuget pack ../src/JinianNet.JNTemplate/JinianNet.JNTemplate.csproj -Properties Configuration=Release
nuget push *.nupkg
del *.nupkg