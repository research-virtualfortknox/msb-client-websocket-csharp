#!/bin/bash

# Nuget
echo "Pushing packages to Nuget..." &&
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
NUGET_FILE=$(find src/Fraunhofer.IPA.MSB.Client.API/bin/Release/ -name \*.nupkg)
dotnet nuget push $NUGET_FILE --source $NUGET_SOURCE --api-key $NUGET_API_KEY
NUGET_FILE=$(find src/Fraunhofer.IPA.MSB.Client.Websocket/bin/Release/ -name \*.nupkg)
dotnet nuget push $NUGET_FILE --source $NUGET_SOURCE --api-key $NUGET_API_KEY