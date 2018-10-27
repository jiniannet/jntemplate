#!/bin/sh
cmdtype=0
menu(){
    if [ $# -gt 0 ] ; then
        cmdtext=$1
    else
        echo "
              welcome use jntemplate tools
          请确认您是否是以管理员身份模式运行本程序
                  www.jiniannet.com

           ┌──────── please enter ────────┐
           │  1 build by net40 or net20   │
           │  2 build by netcoreapp       │
           │  3 build by netstandard      │
           │  4 clear temporary files     │
           │  5 core/standard setting     │
           │  6 net framework setting     │
           │  0 exit                      │
           └──────────────────────────────┘
                         "
        echo -n "input your command:"
        read cmdtext
    fi
    if  [ "$cmdtext" = "1" ] ; then
        cmdbuildnf
    elif [ "$cmdtext" = "2" ] ; then
        cmdbuildnc
    elif [ "$cmdtext" = "3" ] ; then
        cmdbuildns
    elif [ "$cmdtext" = "4" ] ; then
        cmdclearfiles
    elif [ "$cmdtext" = "5" ] ; then
        cmdncev
    elif [ "$cmdtext" = "6" ] ; then
        cmdnfev
    elif [ "$cmdtext" = "0" ] ; then
        cmdexit
    elif [ "$cmdtext" = "exit" ] ; then
        cmdexit
    else
        echo "input error,try again！"
        menu
    fi
}

# mono
cmdbuildnf(){
    echo "start build..."
    build_path="../lib"
    build_key="../../tool/jiniannet.snk"

    if [ ! -d "$build_path" ]; then
        mkdir "$build_path"
    fi

    cur_path=$(cd "$(dirname "$0")"; pwd)
    cd $cur_path/../src/JinianNet.JNTemplate

    if [ ! -f "../../tool/jiniannet.snk" ]; then
        mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NET40,NOTDNX ecurse:*.cs
    else
        mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /keyfile:../../tool/jiniannet.snk /warn:3 /nologo /o /define:NET40,NOTDNX ecurse:*.cs
    fi

    echo "build complete..."
    cd $cur_path
    echo "jntemplate for mono build success!"
    cmdexit
}

#netcore
cmdbuildnc(){
    cmdncev

    cd ../src/JinianNet.JNTemplate
    dotnet restore JinianNet.JNTemplateCore.csproj
    dotnet build JinianNet.JNTemplateCore.csproj --configuration Release --output ../../lib/netcoreapp2.0

    cd ..
    cd ..
    cd build
    echo "jntemplate for netcoreapp 2.0 build success!"
}

#netstandard
cmdbuildns(){
    cmdncev

    cd ../src/JinianNet.JNTemplate
    dotnet restore JinianNet.JNTemplateStandard.csproj
    dotnet build JinianNet.JNTemplateStandard.csproj --configuration Release --output ../../lib/netstandard2.0 --framework netstandard2.0

    cd ..
    cd ..
    cd build
    echo jntemplate for netStandard 2.0 build success!
}

#clear
cmdclearfiles(){
    rm -f ../src/JinianNet.JNTemplate.Test/dll/JinianNet.JNTemplate.dll
    rm -f ../lib/*.*
    rm -f ../src/JinianNet.JNTemplate.Test/html/*.html
    rm -f ../src/JinianNet.JNTemplate.Test/html/*.txt
    rm -f ../TestResults/*.*
    rm -rf ../src/JinianNet.JNTemplate/obj
    rm -rf ../src/JinianNet.JNTemplate/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/obj
}

#net core nev
cmdncev(){
    if [ -d "../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs.build.tmp" ];then
        if [ -d "../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs" ];then
            rm -f ../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs.build.tmp
        fi
    fi
    if [ -d "../src/JinianNet.JNTemplate.Test/Properties/AssemblyInfo.cs.build.tmp" ];then
        if [ -d "../src/JinianNet.JNTemplate.Test/Properties/AssemblyInfo.cs" ];then
            rm -f ../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs.build.tmp
        fi
    fi
    rm -rf ../src/JinianNet.JNTemplate/obj
    rm -rf ../src/JinianNet.JNTemplate/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/obj

    echo "config net core or net Standard env success!"

}

#mono nev
cmdnfev(){
    if [ -d "../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs.build.tmp" ];then
        if [ ! -d "../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs" ];then
            mv../src/JinianNet.JNTemplate/Properties/AssemblyInfo.cs.build.tmp AssemblyInfo.cs
        fi
    fi

    if [ -d "../src/JinianNet.JNTemplate.Test/Properties/AssemblyInfo.cs.build.tmp" ];then
        if [ ! -d "../src/JinianNet.JNTemplate.Test/Properties/AssemblyInfo.cs" ];then
            mv../src/JinianNet.JNTemplate.Test/Properties/AssemblyInfo.cs.build.tmp AssemblyInfo.cs
        fi
    fi
    rm -rf ../src/JinianNet.JNTemplate/obj
    rm -rf ../src/JinianNet.JNTemplate/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/bin
    rm -rf ../src/JinianNet.JNTemplate.Test/obj

    echo config framework env success!
}

#exit
cmdexit(){
    exit 0
}

menu
cmdexit