$currentFolder = $PSScriptRoot

$slnFolder = Join-Path $currentFolder "../"
$certsFolder = Join-Path $currentFolder "certs"

If(!(Test-Path -Path $certsFolder))
{
    New-Item -ItemType Directory -Force -Path $certsFolder
    if(!(Test-Path -Path (Join-Path $certsFolder "localhost.pfx") -PathType Leaf)){
        Set-Location $certsFolder
        dotnet dev-certs https -v -ep localhost.pfx -p BB7BBDD7-5D33-48D6-9B6F-C49E3EF468ED -t        
    }
}

Set-Location $currentFolder
docker-compose up
