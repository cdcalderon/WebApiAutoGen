@ApiTest
Feature: Check For Updates
	Check for updates endpoint scenarios

	Background: 
		Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
        And Patient "patient 1" with patient number "S-10001" with status "active" is associated with "Site 1"
		And The following Software Releases are assigned to the study
			| Version  | PackagePath   | Configuration Version |
			| 5.0.1.10 | software1.zip | 7.0-01.00             |
			| 5.0.1.12 | software2.zip | 10.0-5.0              |
		And "Tablet" Device "YP-E2ECHECKFORUPDATES" is assigned to Site "Site 1"
		And "Tablet" Device "YP-E2ECHECKFORUPDATES" is assigned to Software Release "Release 5.0.1.12"

	Scenario Outline: Check for updates response includes appropriate configuration and software version
		Given Check for Updates API request contains 
            | Device Type | AssetTag              | Site   | Software Version | Configuration Version |
            | BYOD        | YP-E2ECHECKFORUPDATES | Site 1 | <software>       | <configuration>       |
        When the request is made to "check for updates" endpoint
		Then The response contains Package Path "<packagePath>" and Configuration Update "<configurationUpdate>"

		Examples: 
			| software | configuration | packagePath   | configurationUpdate | description                                            |
			| 5.0.1.10 | 7.0-01.00     | software2.zip | 10.0-5.0            | device is eligible for new software and config         |
			| 5.0.1.10 | 10.0-5.0      | software2.zip | null                | device is eligible for new software but not new config |
			| 5.0.1.12 | 7.0-01.00     | null          | 10.0-5.0            | device is eligible for new config but not new software |
			| 5.0.1.12 | 10.0-5.0      | null          | null                | device is not eligible for new software or config      |
