@NavigationBar
Feature: SubjectIndex
	

Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
    And Site "Site 2" is assigned to Country "United States" and has site number "100001"
    And Patient "patient 1" with patient number "S-100000-004" is associated with "Site 1"
    And Diary Entry "diary 1" is associated with "patient 1"
    And Patient "patient 2" with patient number "S-100000-005" is associated with "Site 1"
    And Patient "patient 3" with patient number "S-100000-006" is associated with "Site 1"
    And Patient "patient 4" with patient number "S-100001-007" is associated with "Site 2"
	And User "PortalE2EUser" with role "YP" has access to site "100000"
    And User "PortalE2EUser" with role "YP" has access to site "100001"


	
 @MockStudyBuilder
#SIRE.02.1 Following columns will be diaplyed by default within subject index grid i.e. "Site Name","Subject Number","Subject Status",
           #"Enrollment Date","Compliance","Handheld Training Complete","Tablet Training Complete","Last Diary Date","Last Sync Date"
Scenario: Columns will be displayed within the subject index grid by default
     Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And the following columns is displayed within subject index grid
      | Label                 |
      | Site Name                  |
      | Subject Number             |
      | Subject Status             |
      | Enrollment Date            |
      | Compliance                 |
      | Handheld Training Complete |
      | Tablet Training Complete   |
      | Last Diary Date            |
      | Last Sync Date             |
     And the value with respect to given column name is displayed as
     | SiteName | SubjectNumber | SubjectStatus | EnrollmentDate | Compliance | HandheldTrainingComplete  |TabletTrainingComplete  | LastDiaryDate | LastSyncDate |
     | 100000   | S-100000-004  | Screened      | Current Date   | False      | False                     | False                  | Current Date  | Current Date |
     | 100000   | S-100000-005  | Screened      | Current Date   | False      | False                     | False                  |               |              |
     | 100000   | S-100000-006  | Screened      | Current Date   | False      | False                     | False                  |               |              |
     | 100001   | S-100001-007  | Screened      | Current Date   | False      | False                     | False                  |               |              |

  
@MockStudyBuilder
#Sire .03
Scenario: Verify the "ADD NEW SUBJECT" button is visible when "CAN CREATE SUBJECT" is enabled
    Given "CAN CREATE SUBJECT" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page 
    And "Add New Subject" button is displayed
    And Add New Subject button is disabled
    When I hover on "Add New Subject" button "Please select a site before trying to add a Subject" message is displayed
    Then I click on "All Sites" dropdown
    And I select "Site 1" from "All Sites" dropdown
    And "Add New Subject" button is enabled

@MockStudyBuilder
#Sire 03.1
Scenario: Verify the "ADD NEW SUBJECT" button is not visible when "CAN CREATE SUBJECT" is disabled
    Given "CAN CREATE SUBJECT" permission is "Disabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page 
    And "Add New Subject" button is not "Visible"
   
    
@MockStudyBuilder
Scenario:Verify that both inactive and active Subjects that the user has access to will display within All Subjects tab
	Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	And I select Subject "S-100000-006"
    And I am on Subject "S-100000-006" page
    And I click on "Change Subject Status" button
    And I click on "Patient Status" dropdown
    And I select "Screen Failed" from "Patient Status" dropdown
    And "Screen Failed" is displayed for "Patient Status" dropdown
    And I click on "Save Patient Status" button
    And Subject "S-100000-006" is in "inactive" status
    And I am back on "Subject Management" page
    And I click on "Subject" link on the top navigation bar
    And I am on "Subject Management" page
	And Subject "S-100000-004" is in "Active" status
	And Subject "S-100000-005" is in "Active" status
    And Subject "S-100001-007" is in "Active" status
    And I click on "Inactive Subjects" tab
	And Subject "S-100000-006" is in "inactive" status
	When I click on "All Subjects" tab
    Then "S-100000-004" link is "Visible"
	And "S-100000-005" link is "Visible"
    And "S-100000-006" link is "Visible"
    And "S-100001-007" link is "Visible"