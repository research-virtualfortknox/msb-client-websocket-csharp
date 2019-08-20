#!/bin/bash

# Nuget
echo "Pushing packages to Nuget..." &&
dotnet nuget push src/Fraunhofer.IPA.MSB.Client.API/bin/Release/Fraunhofer.IPA.MSB.Client.API.*.nupkg --api-key $NUGET_API_KEY
dotnet nuget push src/Fraunhofer.IPA.MSB.Client.Websocket/bin/Release/Fraunhofer.IPA.MSB.Client.Websocket.*.nupkg --api-key $NUGET_API_KEY