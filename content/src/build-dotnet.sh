#!/bin/sh
set -e
cd `dirname $0`

dotnet clean MyVendor.MyService.sln
dotnet msbuild /t:Restore /t:Build /t:Publish /p:PublishDir=./obj/Docker/publish /p:Configuration=Release /p:Version=${1:-0.1-dev} MyVendor.MyService.sln
