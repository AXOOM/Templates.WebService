﻿Param ([string]$Version = "0.1-dev")
$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet clean MyVendor.MyService.sln
dotnet msbuild /t:Restore /t:Build /t:Publish /p:PublishDir=./obj/Docker/publish /p:Configuration=Release /p:Version=$Version MyVendor.MyService.sln

popd
