#!/bin/sh
cmdtype = 0

function menu(){
    if [ $# -gt 0 ] then
        cmdtext=$1
    else
        echo "
                欢迎使用服务管理控制台
        请确认您是否是以管理员身份模式运行本程序
                    www.jiniannet.com
        ┌─────请输入您要操作的命令─────┐
        │  1 构建netframework版　　　　│
        │  2 构建netcore 版　　　　　　│
        │  3 构建netstandard 版　　　　│
        │  4 清理无用文件　　　　　　　│
        │  5 配置core/standard环境 　　│
        │  6 配置netframework环境　　　│
        │  0 退出 　　　　　　　　 　　│
        └──────────────────────────────┘"
        echo -n "input your command:"
        read cmdtext
    fi
    cmdtype = 0
    If  [ $cmdtext ="1"] then
        cmdbuildnf
    elif [ $cmdtext =="2"] then
        cmdbuildnc
    elif [ $cmdtext =="3"] then
        cmdbuildns
    elif [ $cmdtext =="4"] then
        cmdclearfiles
    elif [ $cmdtext =="5"] then
        cmdncev
    elif [ $cmdtext =="6"] then
        cmdnfev
    elif [ $cmdtext =="0"] then
        cmdexit
    elif [ $cmdtext =="exit"] then
        cmdexit
    else
        echo input error,try again！
        menu
    fi
}

# mono
function cmdbuildnf(){
    echo "start build..."
    build_path="../lib"
    build_key="../../tool/jiniannet.snk"

    if [ ! -d "$build_path" ]; then
        mkdir "$build_path"
    fi

    cur_path=$(cd "$(dirname "$0")"; pwd)
    cd $cur_path/../src/JinianNet.JNTemplate

    if [ ! -f "../../tool/jiniannet.snk" ]; then
        mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /warn:3 /nologo /o /define:NET40,NOTDNX /recurse:*.cs
    else
        mcs /target:library /out:../../lib/JinianNet.JNTemplate.dll /doc:../../lib/JinianNet.JNTemplate.xml /keyfile:../../tool/jiniannet.snk /warn:3 /nologo /o /define:NET40,NOTDNX /recurse:*.cs
    fi

    echo "build complete..."
    cd $cur_path
    echo "jntemplate for mono build success!"
    cmdexit
}

#netcore
function cmdbuildnc(){

}

#netstandard
function cmdbuildns(){

}

#clear
function cmdclearfiles(){
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
function cmdncev(){

}

#mono nev
function cmdnfev(){

}

#exit
function cmdexit(){
    exit 0
}

menu