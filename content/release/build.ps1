Param ([Parameter(Mandatory=$True)][string]$Version)
$ErrorActionPreference = "Stop"
Push-Location $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

$dockerRegistry = $(if ($Version.Contains("-")) {"docker-ci.axoom.cloud"} else {"docker.axoom.cloud"})
ax release my_service.yml $Version --arg DOCKER_REGISTRY=$dockerRegistry

Pop-Location
