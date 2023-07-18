@Regression
Feature: Can Activate Visits In Portal Permission
    PBI 94332 Portal VisitManagement

    Background:
        Given I have logged in with user "PortalE2EUser"
        And I am on Role Management page
        And the "CAN ACTIVATE VISITS IN PORTAL." permission is "Enabled" for "YP"
        And the "CAN ACTIVATE VISITS IN PORTAL." permission is "Disabled" for "IV"

    #1
    Scenario: Verify that Can Activate Visits in Portal button is enabled for YP user by default
        When I click "Set Permissions" button for row "YPrime"
        Then "CAN ACTIVATE VISITS IN PORTAL." is displayed under "Patient Visit" header 
	    And "CAN ACTIVATE VISITS IN PORTAL." togglebutton is enabled

    #2
    Scenario: Verify that Can Activate Visits in Portal button is disabled for IV user by default
        When I click "Set Permissions" button for row "Investigator"
        Then "CAN ACTIVATE VISITS IN PORTAL." is displayed under "Patient Visit" header 
	    And "CAN ACTIVATE VISITS IN PORTAL." togglebutton is disabled

