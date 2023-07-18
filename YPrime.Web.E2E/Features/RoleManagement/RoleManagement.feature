Feature: Role Management
	NLA-146

Background: 
Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
            
#Roles-Mgmt.07 REQ-Roles-Mgmt.01 Roles-Mgmt.08
@MockStudyBuilder
Scenario: Verify Roles on "Role Management" page
	Given Role "Test Role" is configured for the study
    And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    When I click on "Role Management" link
    Then I am on "Role Management" page
    And following data is displayed in "roles" Grid
		| Name                        |
		| Clinical Research Associate |
		| Investigator                |
		| Sponsor                     |
		| Study Coordinator           |
		| Sub-Investigator            |
		| Test Role                   |
		| YPrime                      |
    
#Roles-Mgmt.04 Roles-Mgmt.03
@MockStudyBuilder
Scenario: Verify "Role Management" grid and "Set Permissions" allows a user to configure functionality of devices and portal
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
    When I click "Set Permissions" button for row "YPrime"
    Then I am on "Manage Role Permissions" page
    And I "Disable" "Upload reference materials." permission
    And I refresh page
    When I click on "Manage Study" link on top navigation bar
    Then "UPLOAD REFERENCE MATERIALS." link is "NOT VISIBLE"
    And I click on the background screen
    And I "Disable" "Ability to View 'At a Glance' dashboard." permission
    When I refresh page
    Then "At a Glance" link is "NOT VISIBLE"
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
    Then following data is displayed in "roles" Grid
    | Name         | Last Update    | Set Permissions        | Set Subscriptions        | Set Reports        |
    | YPrime       | Current Date   | Set Permissions        | Set Subscriptions        | Set Reports        |
    When I click on Hamburger icon for Grid
    Then following columns are displayed for "roles" Grid Menu
        | Label            |
        | Name              |
        | Last Update       |
        | Set Permissions   |
        | Set Subscriptions |
        | Set Reports       |
     And I click on "Last Update" link
     When I click on the background screen
     Then "Last Update" column is not visible in "roles" grid
  
  #Roles-Mgmt.05
@MockStudyBuilder
Scenario: Verify Alert is dispalyed under "View Emails" when Alert "CONFIRMATION OF BULK SITE ACTIVATION"  is enabled
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
    And I click "Set Subscriptions" button for row "YPrime"
    And I am on "Manage Role Subscriptions" page
    And I "Enable" "Confirmation of Bulk Site Activation" permission
    And I click on "Sites" link on top navigation bar
    And I am on "Site Management" page
    And I click on "Bulk Edit Sites" button
    And I am on "Bulk Site Management" page
    And I click on "Master Active toggle" togglebutton
    When I click on "Save" button
    Then "The Site Management changes have been updated successfully. " is displayed in the page
    And I click on "Manage Study" link on top navigation bar
    When I click on "View Emails" link
    Then I am on "Saved Emails" page
    And following data is displayed in "Saved Emails" Grid
		| Email Subject                                                  | Type  |
		| VAL-YPrime eCOA-E2E-Mock Confirmation of Bulk Site Management	 | Alert |

#Roles-Mgmt.05
@MockStudyBuilder
Scenario: Verify Email is dispalyed under "View Emails" when Email "CONFIRMATION OF SITE MANAGEMENT" is enabled
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
    And I click "Set Subscriptions" button for row "YPrime"
    And I am on "Manage Role Subscriptions" page
    And I "Enable" "Confirmation of Site Management" permission
    And I click on "Sites" link on top navigation bar
    And I am on "Site Management" page
    And I click on "10001" link
    And I am now on "Site Details" page
    And I enter the following data
    | Label           | Value                                  | FieldType    |
    | Primary Contact | TesterEdit                             | Inputtextbox |
    And I click on "Save Tab"
    And I click on "Save" button
    Then I am on "Confirmation of Site Management" page
    And I click on "Manage Study" link on top navigation bar
    When I click on "View Emails" link
    Then I am on "Saved Emails" page
    And following data is displayed in "Saved Emails" Grid
		| Email Subject                                                         | Type  |
		| VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 10001 | Email |

@ID:4761
#1
Scenario: Verify that the permissions Can View Analytics is displayed under Analytics label and Can View Reports under Report Label.
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
    When I click "Set Permissions" button for row "YPrime"
    Then "CAN VIEW ANALYTICS" is displayed under "Analytics" header
    And "CAN VIEW REPORTS" is displayed under "Report" header
   