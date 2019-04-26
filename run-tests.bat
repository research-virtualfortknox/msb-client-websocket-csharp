dotnet restore
dotnet build -c Release
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.API.Tests.Unit\ /p:CoverletOutput=../../artifacts/coverage/Fraunhofer.IPA.MSB.Client.API.Tests.Unit/ @coverage.config
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit\ /p:CoverletOutput=../../artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit/ @coverage.config
dotnet test -c Release tests\Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration\ /p:CoverletOutput=../../artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration/ @coverage.config

@RD /S /Q "./artifacts/coverage/report"
dotnet %userprofile%\.nuget\packages\reportgenerator\4.1.2\tools\netcoreapp2.0\ReportGenerator.dll -reports:./artifacts/coverage/Fraunhofer.IPA.MSB.Client.API.Tests.Unit/coverage.opencover.xml;./artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit/coverage.opencover.xml;./artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration/coverage.opencover.xml -reporttypes:HtmlSummary;xml -targetdir:./artifacts/coverage/report