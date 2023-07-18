Feature: Pending Data Correction
NLA-296

#1
@MockStudyBuilder
#DCFWD.03
Scenario: Verify "Add New DCF" button is not visible when "CAN CREATE DATA CORRECTIONS" permission is disabled
	Given I am logged in as "PortalE2EUser"
	And "CAN CREATE DATA CORRECTIONS." permission is "Disabled"
	And I am on "At a Glance" page
	When I click on "Manage DCF's" button
	Then I am on "Data Correction" page
	And "Add New DCF" button is not "VISIBLE"

@MockStudyBuilder
#DCFWD.03
Scenario: Verify "Add New DCF" button is visible when "CAN CREATE DATA CORRECTIONS" permission is enabled
	Given I am logged in as "PortalE2EUser"
	And "CAN CREATE DATA CORRECTIONS." permission is "ENABLED"
	And I am on "At a Glance" page
	When I click on "Manage DCF's" button
	Then I am on "Data Correction" page
	And "Add New DCF" button is "VISIBLE"

@MockStudyBuilder
#
Scenario: Verify "Manage DCF's" button is visible when "CAN VIEW DATA CORRECTION." permission is enabled
	Given I am logged in as "PortalE2EUser"
	And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
	When I am on "At a Glance" page
	Then "Manage DCF's" button is "VISIBLE"

@MockStudyBuilder
#
Scenario: Verify "Manage DCF's" button is not visible when "CAN VIEW DATA CORRECTION." permission is disabled
	Given I am logged in as "PortalE2EUser"
	And "CAN VIEW DATA CORRECTIONS" permission is "Disabled"
	When I am on "At a Glance" page
	Then "Manage DCF's" button is not "VISIBLE"

@MockStudyBuilder
#DCFWD.04 DCFWD.02
Scenario: Verify User can access DCF's only of their assigned sites
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And Patient "patient 1" with patient number "S-10000-001" is associated with "Site 1"
	And Patient "patient 1" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Female         |
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I create a Data Correction for "Height" associated with Patient "S-10000-001"
	When I am on "At a Glance" page
	Then following data is displayed in "DCF" Grid
		| Subject     | Correction                 |
		| S-10000-001 | Change subject Information |
	And I click on "Manage DCF's" button
	When I am on "Data Correction" page
	Then following data is displayed in "Correction" Grid
		| Type						 | Subject                 |
		| Change subject Information | S-10000-001			   |
	And I click on the user icon
	And I click on " Logout" button
	And Site "Site 2" is assigned to Country "United States" and has site number "20000"
	And Patient "patient 1" with patient number "S-20000-001" is associated with "Site 2"
	And User "ypadmin1@yprime.com" with role "YP" has access to site "20000"
	And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
	When I am on "At a Glance" page
	Then "S-10000-001" text is "NOT VISIBLE"
	And "Change subject Information" text is "NOT VISIBLE"
	And I click on the user icon
	And I click on " Logout" button
	And Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And Patient "patient 1" with patient number "S-10000-001" is associated with "Site 1"
	And User "ypadmin2@yprime.com" with role "IV" has access to site "10000"
	And I have logged in with user "ypadmin2@yprime.com", password "Welcome012!"
	When I am on "At a Glance" page
	Then following data is displayed in "DCF" Grid
		| Subject     | Correction                 |
		| S-10000-001 | Change subject Information |


#DCFWD.04 
@MockStudyBuilder
Scenario: Verify DCFs that a user has access to will display within this widget.
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And User "ypadmin1@yprime.com" with role "IV" has access to site "10000"
	And Patient "patient 1" with patient number "S-10000-001" is associated with "Site 1"
	And Patient "patient 1" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Female         |
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I create a Data Correction for "Height" associated with Patient "S-10000-001"
	When I am on "At a Glance" page
	Then following data is displayed in "DCF" Grid
		| Subject     | Correction                 |
		| S-10000-001 | Change subject Information |
	And I click on the user icon
	And I click on " Logout" button
	And I have logged in with user "ypadmin2@yprime.com", password "Welcome012!"
	When I am on "At a Glance" page	
	Then "S-10000-001" text is "NOT VISIBLE"
	And "Change subject Information" text is "NOT VISIBLE"