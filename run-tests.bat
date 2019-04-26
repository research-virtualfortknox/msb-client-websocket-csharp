dotnet restore
dotnet build -c Release
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.API.Tests.Unit\ /p:CoverletOutput=../../artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.API.Tests.Unit.xml @coverage.config
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit\ /p:CoverletOutput=../../artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit.xml @coverage.config
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration\ /p:CoverletOutput=../../artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration.xml @coverage.config

@RD /S /Q "./artifacts/coverage/report"
dotnet %userprofile%\.nuget\packages\reportgenerator\4.1.2\tools\netcoreapp2.0\ReportGenerator.dll -reports:./artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.API.Tests.Unit.xml;./artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit.xml;./artifacts/coverage/coverage-Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration.xml -targetdir:./artifacts/coverage/report