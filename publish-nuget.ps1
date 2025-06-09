param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$ApiKey,
    
    [Parameter(Mandatory=$false)]
    [string]$Source = "https://api.nuget.org/v3/index.json"
)

$projects = @(
    "src\BlazeGate.AspNetCore",
    "src\BlazeGate.Common",
    "src\BlazeGate.JwtBearer",
    "src\BlazeGate.Model",
    "src\BlazeGate.RBAC.Components",
    "src\BlazeGate.Services.Implement.Remote",
    "src\BlazeGate.Services.Interface"
)

# 设置版本号
foreach ($project in $projects) {
    $csprojPath = "$project\$($project.Split('\')[-1]).csproj"
    Write-Host "更新项目版本号: $csprojPath 为 $Version" -ForegroundColor Cyan
    
    $content = Get-Content $csprojPath -Raw
    $newContent = $content -replace '<Version>([0-9]+\.[0-9]+\.[0-9]+)</Version>', "<Version>$Version</Version>"
    Set-Content -Path $csprojPath -Value $newContent
}

# 清理旧的输出
Write-Host "清理解决方案..." -ForegroundColor Yellow
dotnet clean -c Release

# 打包所有项目
Write-Host "开始打包所有项目..." -ForegroundColor Yellow
foreach ($project in $projects) {
    Write-Host "打包项目: $project" -ForegroundColor Cyan
    dotnet pack $project -c Release
}

# 发布所有包
Write-Host "开始发布所有NuGet包..." -ForegroundColor Yellow
foreach ($project in $projects) {
    $projectName = $project.Split('\')[-1]
    $nupkgPath = "$project\bin\Release\$projectName.$Version.nupkg"
    
    if (Test-Path $nupkgPath) {
        Write-Host "推送包: $nupkgPath" -ForegroundColor Green
        dotnet nuget push $nupkgPath --source $Source --api-key $ApiKey
    } else {
        Write-Host "警告: 找不到包 $nupkgPath" -ForegroundColor Red
    }
}

Write-Host "所有操作完成!" -ForegroundColor Green