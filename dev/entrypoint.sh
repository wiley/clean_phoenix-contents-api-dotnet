#!/bin/bash

dotnet nuget add source --name Artifactory https://artifactory.aws.wiley.com/artifactory/api/nuget/nuget
dotnet watch run --project=Contents.API --urls=http://+:80 --no-launch-profile