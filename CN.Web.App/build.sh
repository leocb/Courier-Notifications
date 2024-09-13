#!/bin/sh
cd CN.Web.App
pwd
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 8.0 -InstallDir ./dotnet
./dotnet/dotnet --version
./dotnet/dotnet publish -c Release -o ./publish &
wait
echo "{\"ApiHostname\": \"https://$1/\"}" > publish/wwwroot/appsettings.json