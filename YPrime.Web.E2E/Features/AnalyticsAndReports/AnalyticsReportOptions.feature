@Portal

Feature: Analytics Report Options Displayed based on appropriate permission 
	
@MockStudyBuilder
@ID:4761
#1
Scenario: Verify that Analytics and Reports link in navigation bar is not displayed without appropriate permissions. 
	Given I have logged in with user "PortalE2EUser"
	And "CAN VIEW ANALYTICS" permission is "disabled"
    And "CAN VIEW REPORTS" permission is "disabled"
	When I am on "At a Glance" page
	Then "Analytics & Reports" link is "Not Visible"

@MockStudyBuilder
@ID:4761
#2
Scenario Outline: Verify that Analytics and Report options are displayed based on appropriate permissions. 
    Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
    And User "PortalE2EUser" with role "YP" has access to site "100000"
    And "CAN VIEW ANALYTICS" permission is "<CAN VIEW ANALYTICS>"
    And "CAN VIEW REPORTS" permission is "<CAN VIEW REPORTS>"
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    When I click on "Analytics & Reports" link on the top navigation bar
    Then "Analytics" link is "<Analytics Options Visible>"
    And "Reports" link is "<Reports Options Visible>"

    Examples: 
    | CAN VIEW ANALYTICS | CAN VIEW REPORTS | Analytics Options Visible | Reports Options Visible |
    | Enabled            | Enabled          | Visible                   | Visible                 |
    | Enabled            | Disabled         | Visible                   | Not Visible             |
    | Disabled           | Enabled          | Not Visible               | Visible                 |


