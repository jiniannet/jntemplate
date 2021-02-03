#!/bin/sh

cd ..
rm -rf lib
cd src/JinianNet.JNTemplate
rm -rf obj
rm -rf bin
dotnet build JinianNet.JNTemplate.csproj --configuration Release
cp -avx bin/Release/* ../../lib/
cd ../../build/