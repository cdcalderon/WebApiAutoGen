@Portal


Feature: Set Analytics Permission

Background: 
	Given I have logged in with user "PortalE2EUser"
	And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page

@ID:4533
#1
Scenario: Verify that when the "Set Analytics" button is clicked, the user is navigated to the Analytics Permission page.
	When I click "Set Analytics" button for row "YPrime"
	Then I am on "Manage Role Analytics" page