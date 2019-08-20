dotnet restore
dotnet build -c Release

CALL :ExecuteTestProject "Fraunhofer.IPA.MSB.Client.API.Tests.Unit"
CALL :ExecuteTestProject "Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit"
CALL :ExecuteTestProject "Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration"

rmdir /S /Q "artifacts/coverage/report"
dotnet %userprofile%\.nuget\packages\reportgenerator\4.1.2\tools\netcoreapp2.0\ReportGenerator.dll -reports:./artifacts/coverage/Fraunhofer.IPA.MSB.Client.API.Tests.Unit/coverage.opencover.xml;./artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit/coverage.opencover.xml;./artifacts/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration/coverage.opencover.xml -reporttypes:HtmlSummary;xml -targetdir:./artifacts/coverage/report

EXIT /B %ERRORLEVEL%

:ExecuteTestProject
set testProjectName=%~1

dotnet test --collect:"XPlat Code Coverage" --settings coverletArgs.runsettings tests\%testProjectName%\%testProjectName%.csproj
for /f "tokens=*" %%i in ('where /r tests\%testProjectName%\TestResults coverage.opencover.xml') do set resultPath=%%i
if not exist "artifacts\coverage\%testProjectName%" mkdir artifacts\coverage\%testProjectName%
copy "%resultPath%" "artifacts\coverage\%testProjectName%\coverage.opencover.xml"
rmdir /S /Q tests\%testProjectName%\TestResults 

EXIT /B 0

