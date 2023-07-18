@ignore



Feature: SSO Study Access Deactivated
Background: 
	Given User "PortalE2EUser" with role "YP" access to study is disabled 


@ID:3155
#1
Scenario: Verify that an error message is displayed when the user's access to the study has been deactivated.
    When I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	Then "Your access to the study has been deactivated, please contact your system administrator for details." error message is displayed. 