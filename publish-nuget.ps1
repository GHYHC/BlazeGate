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

# ���ð汾��
foreach ($project in $projects) {
    $csprojPath = "$project\$($project.Split('\')[-1]).csproj"
    Write-Host "������Ŀ�汾��: $csprojPath Ϊ $Version" -ForegroundColor Cyan
    
    $content = Get-Content $csprojPath -Raw
    $newContent = $content -replace '<Version>([0-9]+\.[0-9]+\.[0-9]+)</Version>', "<Version>$Version</Version>"
    Set-Content -Path $csprojPath -Value $newContent
}

# ����ɵ����
Write-Host "����������..." -ForegroundColor Yellow
dotnet clean -c Release

# ���������Ŀ
Write-Host "��ʼ���������Ŀ..." -ForegroundColor Yellow
foreach ($project in $projects) {
    Write-Host "�����Ŀ: $project" -ForegroundColor Cyan
    dotnet pack $project -c Release
}

# �������а�
Write-Host "��ʼ��������NuGet��..." -ForegroundColor Yellow
foreach ($project in $projects) {
    $projectName = $project.Split('\')[-1]
    $nupkgPath = "$project\bin\Release\$projectName.$Version.nupkg"
    
    if (Test-Path $nupkgPath) {
        Write-Host "���Ͱ�: $nupkgPath" -ForegroundColor Green
        dotnet nuget push $nupkgPath --source $Source --api-key $ApiKey
    } else {
        Write-Host "����: �Ҳ����� $nupkgPath" -ForegroundColor Red
    }
}

Write-Host "���в������!" -ForegroundColor Green