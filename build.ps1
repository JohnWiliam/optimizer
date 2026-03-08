[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release',
    [ValidateSet('win-x64','win-arm64')]
    [string]$Runtime = 'win-x64',
    [switch]$Clean
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$logDir = Join-Path $repoRoot 'artifacts/logs'
$outputDir = Join-Path $repoRoot "artifacts/publish/$Runtime/$Configuration"
$logFile = Join-Path $logDir ("build-{0:yyyyMMdd-HHmmss}.log" -f (Get-Date))

New-Item -ItemType Directory -Path $logDir -Force | Out-Null
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

function Write-Log {
    param([string]$Message)

    $line = "[{0:HH:mm:ss}] {1}" -f (Get-Date), $Message
    $line | Tee-Object -FilePath $logFile -Append
}

Write-Log '==== Optimizer modern build started ===='
Write-Log "Repository: $repoRoot"
Write-Log "Configuration: $Configuration"
Write-Log "Runtime: $Runtime"
Write-Log "Log file: $logFile"

$dotnetInfo = dotnet --info
$dotnetInfo | Out-File -FilePath $logFile -Append

if ($dotnetInfo -notmatch '10\.0') {
    throw 'SDK .NET 10 não encontrado. Instale o SDK 10.x para gerar o executável portátil.'
}

Push-Location $repoRoot
try {
    if ($Clean) {
        Write-Log 'Limpando artefatos antigos...'
        dotnet clean .\Optimizer.sln -c $Configuration | Tee-Object -FilePath $logFile -Append
    }

    Write-Log 'Restaurando pacotes...'
    dotnet restore .\Optimizer.sln | Tee-Object -FilePath $logFile -Append

    Write-Log 'Compilando projeto WPF moderno...'
    dotnet build .\Optimizer\Optimizer.csproj -c $Configuration -r $Runtime --no-restore | Tee-Object -FilePath $logFile -Append

    Write-Log 'Publicando executável self-contained + single-file...'
    dotnet publish .\Optimizer\Optimizer.csproj `
        -c $Configuration `
        -r $Runtime `
        --self-contained true `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true `
        /p:EnableCompressionInSingleFile=true `
        /p:DebugType=None `
        /p:DebugSymbols=false `
        -o $outputDir `
        --no-build | Tee-Object -FilePath $logFile -Append

    Write-Log "Build concluído com sucesso. Saída: $outputDir"
}
catch {
    Write-Log "ERRO: $($_.Exception.Message)"
    throw
}
finally {
    Pop-Location
    Write-Log '==== Optimizer modern build finished ===='
}
