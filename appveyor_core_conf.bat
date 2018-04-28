@echo off
ren src\JinianNet.JNTemplate\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
ren src\JinianNet.JNTemplate.Test\Properties\AssemblyInfo.cs AssemblyInfo.cs.build.tmp
echo ren success.
nuget restore JinianNet.JNTemplateStandard.sln
echo ren success.