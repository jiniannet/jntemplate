Write-Host "            欢迎使用服务管理控制台
   请确认您是否是以管理员身份模式运行本程序

 ┌─────请输入您要操作的命令─────┐
 │  1 安装服务                            │
 │  2 启动服务                            │
 │  3 停止服务                            │
 │  4 卸载服务                            │
 │  5 命令行模式                          │
 │  6 安装更新服务                        │
 │  7 开始更新服务                        │
 │  8 停止更新服务                        │
 │  9 卸载更新服务                        │
 │  10 退出                                │
 └────────────────────┘"


Function ClearDirectory($dir) {
    Get-ChildItem "$dir" | ForEach-Object -Process {
        if ($_ -is [System.IO.DirectoryInfo]) { 
            if ($_.Name.ToLower() -eq "bin" -or $_.Name.ToLower() -eq "obj") {
                Remove-Item "$_.FullName" -recurse
            }
            else {
                ClearDirectory($_.FullName)
            }
        }
    }
}

Function ClearAssemblyInfo($dir) {
    Get-ChildItem "$dir" | ForEach-Object -Process {
        if ($_ -is [System.IO.FileInfo]) { 
            if ($_.Name.ToLower() -eq "bin" -or $_.Name.ToLower() -eq "obj") {
                Remove-Item "$_.FullName" -recurse
            }
            else {
                ClearDirectory($_.FullName)
            }
        }
    }
}