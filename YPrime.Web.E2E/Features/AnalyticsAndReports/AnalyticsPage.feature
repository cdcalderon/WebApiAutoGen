@Portal

Feature: Analytics Page
	

@ID:3639
#1
Scenario: Verify that the user is navigated to the Analytics page when the user clicks Analytics from the navigation bar. 
	Given I have logged in with user "PortalE2EUser"
	And "CAN VIEW ANALYTICS" permission is "enabled"
	And I am on "At a Glance" page
	When I click on "Analytics & Reports" link on top navigation bar
	And I click on "Analytics" link
	Then I am on "Analytics" page


@ID:3639
#2
Scenario: Verify that 403 page is displayed when user tries to access the Direct link access to analytics page when user does not have permissions.
	Given I have logged in with user "PortalE2EUser"
	And "CAN VIEW ANALYTICS" permission is "disabled"
	And I am on "At a Glance" page
	When I navigate to "Analytics" page
	Then I get a 403 - Forbidden error