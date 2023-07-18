Feature: Hangfire Scheduled Jobs Permission
	

@ID:2937
#1
Scenario: Verify that Can Schedule Hangfire Jobs persmission is enabled for YP user by default 
	Given I have logged in with user "PortalE2EUser"
	And I am on Role Management page
	When I click "Set Permissions" button for row "YPrime"
	Then "CAN SCHEDULE HANGFIRE JOBS" is displayed under "Hangfire Jobs" header 
	And "CAN SCHEDULE HANGFIRE JOBS" togglebutton is enabled
	 

@ID:2937
#2
Scenario: Verify that Can Schedule Hangfire Jobs permission is disabled for IV user by default 
	Given I have logged in with user "PortalE2EUser"
	And I am on Role Management page
	When I click "Set Permissions" button for row "Investigator"
	Then "CAN SCHEDULE HANGFIRE JOBS" is displayed under "Hangfire Jobs" header 
	And "CAN SCHEDULE HANGFIRE JOBS" togglebutton is disabled


@ID:2937
#3
Scenario Outline: Verify that "Scheduled Jobs" link is visible in the Manage Study based on the permission.
	Given I have logged in with user "PortalE2EUser"
	And "CAN SCHEDULE HANGFIRE JOBS" permission is "<Can Schedule Hangfire Jobs>"
	And I am on "At a Glance" page
	When I click on "Manage Study" link on the top navigation bar
	Then "Schedule Jobs" link is "<Visible>"

Examples: 
| Can Schedule Hangfire Jobs | Visible     |
| Enabled                    | Visible     |
| Disabled                   | Not Visible |


@ID:2937
#4
Scenario: Verify that when the user clicks "Schedule Jobs" link the user is navigated to the Hangfire Dashboard.
	Given I have logged in with user "PortalE2EUser"
	And "CAN SCHEDULE HANGFIRE JOBS" permission is "Enabled"
	And I am on "At a Glance" page
	And I click on "Manage Study" link on the top navigation bar
	When I click on "Schedule Jobs" link 
	Then I am on "Hangfire Dashboard" page


@ID:2937
#5
Scenario: Verify that when Permission is off, “Schedule Jobs” button is hidden in the Manage Study tab and if the user is navigated to the Hangfire dashboard they’re shown a 403 error.
	Given I have logged in with user "PortalE2EUser"
	And "CAN SCHEDULE HANGFIRE JOBS" permission is "Disabled" 
	And I am on "At a Glance" page
	When I navigate to "Schedule All Jobs" page 
	Then I get a 403 - Forbidden error
