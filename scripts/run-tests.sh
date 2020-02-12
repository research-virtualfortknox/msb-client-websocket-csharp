#!/bin/bash

SOLUTION_DIRECTORY=$(readlink -f $1)
TESTS_DIRECTORY=$SOLUTION_DIRECTORY/tests
ARTIFACTS_DIRECOTRY=$SOLUTION_DIRECTORY/artifacts
COVERLET_SETTINGS=$(readlink -f $SOLUTION_DIRECTORY/scripts/coverletArgs.runsettings)

echo "SOLUTION_DIRECTORY: $SOLUTION_DIRECTORY"
echo "COVERLET_SETTINGS: $COVERLET_SETTINGS"
echo "MSB_WEBSOCKET_INTERFACE_URL: $MSB_WEBSOCKET_INTERFACE_URL"
echo "MSB_SMARTOBJECTMGMT_URL: $MSB_WEBSOCKET_INTERFACE_URL"
echo "MSB_INTEGRATIONDESIGNMGMT_URL: $MSB_WEBSOCKET_INTERFACE_URL"

RETURN_CODE=0

ExecuteTestProject() 
{
    TEST_PROJECT_NAME=$1

	dotnet test --collect:"XPlat Code Coverage" --settings $COVERLET_SETTINGS $TESTS_DIRECTORY/$TEST_PROJECT_NAME/$TEST_PROJECT_NAME.csproj
	TEST_SUCCESSFUL=$?
	RESULT_PATH=$(find $TESTS_DIRECTORY/$TEST_PROJECT_NAME/TestResults -name coverage.opencover.xml)
	mkdir -p $ARTIFACTS_DIRECOTRY/coverage/$TEST_PROJECT_NAME
	cp -f $RESULT_PATH $ARTIFACTS_DIRECOTRY/coverage/$TEST_PROJECT_NAME
	rm -r $TESTS_DIRECTORY/$TEST_PROJECT_NAME/TestResults
	
	if [ $TEST_SUCCESSFUL -ne 0 ]
	then
		RETURN_CODE=1
	fi
}
  
dotnet restore $SOLUTION_DIRECTORY
dotnet build $SOLUTION_DIRECTORY -c Release

ExecuteTestProject Fraunhofer.IPA.MSB.Client.API.Tests.Unit
ExecuteTestProject Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit
ExecuteTestProject Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration

rm -r $ARTIFACTS_DIRECOTRY/coverage/report
dotnet ~/.nuget/packages/reportgenerator/4.1.2/tools/netcoreapp2.0/ReportGenerator.dll "-reports:$ARTIFACTS_DIRECOTRY/coverage/Fraunhofer.IPA.MSB.Client.API.Tests.Unit/coverage.opencover.xml;$ARTIFACTS_DIRECOTRY/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit/coverage.opencover.xml;$ARTIFACTS_DIRECOTRY/coverage/Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration/coverage.opencover.xml" "-reporttypes:HtmlSummary;xml" "-targetdir:$ARTIFACTS_DIRECOTRY/coverage/report"

echo "Script exited with code: $RETURN_CODE"

exit $RETURN_CODE