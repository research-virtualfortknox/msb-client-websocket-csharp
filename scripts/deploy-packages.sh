#!/bin/bash

# Nuget
echo "Pushing packages to Nuget..." &&
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
dotnet nuget push src/Fraunhofer.IPA.MSB.Client.API/bin/Release/Fraunhofer.IPA.MSB.Client.API.$TRAVIS_BRANCH.nupkg --source $NUGET_SOURCE --api-key $NUGET_API_KEY
dotnet nuget push src/Fraunhofer.IPA.MSB.Client.Websocket/bin/Release/Fraunhofer.IPA.MSB.Client.Websocket.$TRAVIS_BRANCH.nupkg --source $NUGET_SOURCE --api-key $NUGET_API_KEY