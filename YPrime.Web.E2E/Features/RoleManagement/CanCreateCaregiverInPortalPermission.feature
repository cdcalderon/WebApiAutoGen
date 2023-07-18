Feature: Can Create Caregiver Permission In Portal 
	#ECOA-2028 
	#New permission "CAN CREATE CAREGIVER IN PORTAL" is added to role management in Portal
 
Background:
    Given I have logged in with user "PortalE2EUser"
    And I am on Role Management page

@ID:2028	
#1
Scenario: Verify that Can Create Caregiver in Portal permission is enabled for YP user by default 
	When I click "Set Permissions" button for row "YPrime"
	Then "CAN CREATE CAREGIVER IN PORTAL" is displayed under "Caregiver" header 
	And "CAN CREATE CAREGIVER IN PORTAL" togglebutton is enabled
	
@ID:2028
#2
Scenario: Verify that Can Create Caregiver in Portal permission is disabled for IV user by default
	When I click "Set Permissions" button for row "Investigator"
	Then "CAN CREATE CAREGIVER IN PORTAL" is displayed under "Caregiver" header
	And "CAN CREATE CAREGIVER IN PORTAL" togglebutton is disabled 

	