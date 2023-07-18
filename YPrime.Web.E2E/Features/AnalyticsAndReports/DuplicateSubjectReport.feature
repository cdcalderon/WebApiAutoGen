@DuplicateSubjectReport
Feature: Duplicate Subject Report
	#NLA-125
	#Able to check Duplicate Subject Report at Analytics & Report page    
 
Background: 
       Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
        And User "PortalE2EUser" with role "YP" has access to site "10000"
        And Patient "patient 1" with status "Screened" and patient number "S-10000-123" is associated with "Site 1"
   	    And Patient "patient 2" with status "Enrolled" and patient number "S-10000-123" is associated with "Site 1"
        And Patient "patient 3" with status "Screened" and patient number "S-10000-111" is associated with "Site 1"
        And Patient "patient 4" with status "Enrolled" and patient number "S-10000-111" is associated with "Site 1"

@MockStudyBuilder
#1 DSR.01, DSR.02, DSR.03, RR.01
Scenario: Verify that Duplicate Subject Record should be visible on Duplicate Subject Report page
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Duplicate Subject (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        When I click on "Duplicate Subject (Unblinded)" from the Report List
        Then following data is displayed in "Duplicate Subject Report" Grid
            | Site      | Subject Number | Enrolled Date   |
            | 10000     | S-10000-111    | Current Date    |
            | 10000     | S-10000-111    | Current Date    |
            | 10000     | S-10000-123    | Current Date    |
            | 10000     | S-10000-123    | Current Date    |

@MockStudyBuilder
#2 DSR.04, RR.02, RR.03
Scenario: Verify that after clicking on export file name must be displayed as "<Sponsor>_<Protocol>_Duplicate Subject.xxx"
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And "Duplicate Subject (Unblinded)" report visibility status is "Enabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    And I click on "Duplicate Subject (Unblinded)" from the Report List
    And I click on "Hamburger" button
	And hamburger menu is displayed with the following functionality for export
		| ButtonName	|
		| Excel			|
		| CSV           |
		| PDF           |
		| Print         |
    When I click on "Enrolled Date" link
    Then "Enrolled Date" column is not visible in "Duplicate Subject Report" grid
    And following data is displayed in "Duplicate Subject Report" Grid
        | Site  | Subject Number |
        | 10000 | S-10000-111    |
        | 10000 | S-10000-111    |
        | 10000 | S-10000-123    |
        | 10000 | S-10000-123    |
    And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Duplicate Subject (Unblinded)" in ".pdf" format file to save in Export Evidence folder

@MockStudyBuilder
#3 DSR.05
Scenario: Verify that Duplicate Subject Record should not be visible on Duplicate Subject Report page if subject number is removed or changed.
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Duplicate Subject (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Duplicate Subject (Unblinded)" from the Report List
        And following data is displayed in "Duplicate Subject Report" Grid
        | Site      | Subject Number | Enrolled Date   |
        | 10000     | S-10000-111    | Current Date    |
        | 10000     | S-10000-111    | Current Date    |
        | 10000     | S-10000-123    | Current Date    |
        | 10000     | S-10000-123    | Current Date    |
        When I update subject number "S-10000-123" to "S-10000-121"
        And I click on "Duplicate Subject (Unblinded)" from the Report List
        Then Subject number "S-10000-123" is not visible
        And following data is displayed in "Duplicate Subject Report" Grid
        | Site      | Subject Number | Enrolled Date   |
        | 10000     | S-10000-111    | Current Date    |
        | 10000     | S-10000-111    | Current Date    |
        And I marked "S-10000-111" as removed
        And I click on "Duplicate Subject (Unblinded)" from the Report List
        And Subject number "S-10000-111" is not visible

@MockStudyBuilder
#4 DSR.01, RR.01
Scenario: Verify if Duplicate subject study is disabled then Duplicate study report should not be visible at Analytics & Report page
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Duplicate Subject (Unblinded)" report visibility status is "Disabled"
        When I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        Then "Duplicate Subject (Unblinded)" link is "Not Visible"


@MockStudyBuilder
#5 RR.02
Scenario: Verify if title on page is correct and search boxes are working fine
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Duplicate Subject (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
       When I click on "Duplicate Subject (Unblinded)" from the Report List
       Then "Duplicate Subject (Unblinded)" report title is displayed on screen
        And I enter "S-10000-111" in "Search_Subject Number" textbox field
	   When "2" records are displayed in "Duplicate Subject Report" data grid
       Then following data is displayed in "Duplicate Subject Report" Grid
        | Site      | Subject Number | Enrolled Date   |
        | 10000     | S-10000-111    | Current Date    |
        | 10000     | S-10000-111    | Current Date    |