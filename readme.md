# AXOOM WebService Template

This repository contains a dotnet-new template for AXOOM WebServices using ASP.NET Core and Docker.

## Building

Run `build.ps1` to package the template as a NuGet package.
This script takes a version number as an input argument. The source code itself contains no version numbers. Instead version numbers should be determined at build time using [GitVersion](http://gitversion.readthedocs.io/).

The `content` directory is what actually gets packaged into the template. The file `content/template.config/template.json` controls which placeholders are replaced during instantiation.

## Using

To install the template run `dotnet new --install axoom-webservice::*`.
To use the template run `dotnet new axoom-webservice --dockerName myservice --friendlyName "My Service" --description "my description"`.
