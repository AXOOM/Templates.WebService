﻿Param ([string]$Version = "0.1-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

src\build.ps1 $Version
src\test.ps1

popd
