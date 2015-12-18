#!/bin/sh

echo "begin build JNTemplate ..."

build_path="../lib"
build_key="../../tool/jiniannet.snk"

if [ ! -d "$build_path" ]; then 
mkdir "$build_path"
fi

cd ../src/JinianNet.JNTemplate

if [ ! -f "../../tool/jiniannet.snk" ]; then 
	mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NET20 /recurse:*.cs
else
	mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /keyfile:../../tool/jiniannet.snk /warn:3 /nologo /o /define:NET20 /recurse:*.cs
fi

echo "build complete..."

cd ..
cd ..
cd build

echo "success..."
