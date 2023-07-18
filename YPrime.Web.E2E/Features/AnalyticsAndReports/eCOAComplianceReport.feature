@eCOAComplianceReport
Feature: eCOAComplianceReport

Validate the ecoa complaince functionality and grid
NLA-130
Background: 
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001" 
	And User "PortalE2EUser" with role "YP" has access to site "10001"
    And Patient "patient 1" with status "Screened" and patient number "S-10001-004" is associated with "Initial Site"
    And Patient "patient 2" with status "Enrolled" and patient number "S-10001-005" is associated with "Initial Site"
    And Patient "patient 3" with status "Screen Failed" and patient number "S-10001-006" is associated with "Initial Site"
    And "2" questionnaire has been configured in "Treatment Visit"
    And "2" questionnaire has been configured in "Screening Visit"
    And "2" questionnaire has been configured in "Enrollment Visit"
    And Patient "patient 1" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Female         |  
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |
    And Patient "patient 2" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Male           |  
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And Subject "S-10001-004" is assigned to "YP-E2E-Device" Device
    And "Phone" Device "YP-E2E-Device-2" is assigned to Site "Initial Site"
    And "Phone" Device "YP-E2E-Device-2" is assigned to Software Release "Initial Release"
    And Subject "S-10001-005" is assigned to "YP-E2E-Device-2" Device
    And Patient Visit "Enrollment Visit" is associated with "patient 1"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And Patient Visit "Treatment Visit" is associated with "patient 2"
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "In general, How would you say your health is:" and choice "Not better"
    And Subject "S-10001-004" has completed "Number Spinner with 1 decimal digit " questionnaire for question "How many pills did you take this morning?" and choice "3"
    And Subject "S-10001-004" has completed "Header Display Name Test" questionnaire for question "Are you experiencing Headache?" and choice "No"
	And Subject "S-10001-005" has completed "NRS Choices Questionnaire" questionnaire for question "Please select the percentage that shows how much relief you have received." and choice "10%"
    And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label            | Value           |
        | Data Source Name | eCOA App        |
        | Started Time     | Current Date    |
        | Completed Time   | Current Date    |
        | Transmitted Time | Current Date    |
        | Diary Date       | Current Date    |
        | Visit Name       | Screening Visit |  
    And I update Diary Entry "Number Spinner with 1 decimal digit " for "patient 1" with following details
        | Label            | Value            |
        | Data Source Name | Web Diary        |
        | Started Time     | Current Date     |
        | Completed Time   | Current Date     |
        | Transmitted Time | Current Date     |
        | Diary Date       | Current Date     |  
        | Visit Name       | Enrollment Visit |
    And I update Diary Entry "Header Display Name Test" for "patient 1" with following details
        | Label            | Value            |
        | Data Source Name | Web Diary        |
        | Started Time     | Current Date     |
        | Completed Time   | Current Date     |
        | Transmitted Time | Current Date     |
        | Diary Date       | Current Date     |  
        | Visit Name       | Enrollment Visit |
    And I update Diary Entry "NRS Choices Questionnaire" for "patient 2" with following details
        | Label            | Value           |  
        | Data Source Name | Web Diary       |
        | Started Time     | Current Date    |
        | Completed Time   | Current Date    |
        | Transmitted Time | Current Date    |
        | Visit Name       | Treatment Visit |

#1
#RR.01
#ECCR.02, 03, 05, 06 
@MockStudyBuilder
Scenario: Verify the compliance grid, inactive subject, visibility status of report
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And "eCOA Compliance (Unblinded)" report visibility status is "Enabled"
    And Key "visitOrder" is set with value "1" in configuration for "Enrollment Visit" 
    And Key "visitOrder" is set with value "2" in configuration for "Screening Visit" 
    And Key "visitOrder" is set with value "3" in configuration for "Treatment Visit"
	When I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    And I click on "eCOA Compliance (Unblinded)" from the Report List
    Then following data is not displayed in "eCOA Compliance (Unblinded)" Grid
        | Site Id      | Subject Number |
        | Initial Site | S-10001-006    |  
	And following data is displayed in "eCOA Compliance (Unblinded)" Grid
    | Site Id      | Subject Number | Enrollment Visit | Screening Visit | Treatment Visit  | Total Compliance |
    | Initial Site | S-10001-004    | 100.00           | 50.00           | -                | 3/6              |
    | Initial Site | S-10001-005    | -                | -               | 50.00            | 1/6              |  

#2
#RR.01
@MockStudyBuilder
Scenario: Verify that eCOA Compliance (Unblinded) report link is not visible when permission is disabled
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    When "eCOA Compliance (Unblinded)" report visibility status is "Disabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    Then "eCOA Compliance (Unblinded)" report link is "Not Visible"

#3
#RR.01,02,03
#ECCR.04
@MockStudyBuilder
Scenario: Verify if title on page is correct and search boxes are working fine
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And "eCOA Compliance (Unblinded)" report visibility status is "Enabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    When I click on "eCOA Compliance (Unblinded)" from the Report List
    Then "eCOA Compliance (Unblinded)" report title is displayed on screen
    And I enter "S-10001-004" in "Search_Subject Number" textbox field
	And "1" records are displayed in "eCOA Compliance (Unblinded)" data grid
    And following data is displayed in "eCOA Compliance (Unblinded)" Grid
        | Site Id      | Subject Number | Enrollment Visit | Screening Visit | Treatment Visit  | Total Compliance |
        | Initial Site | S-10001-004    | 100.00           | 50.00           | -                | 3/6              |
    And I clear text field "Search_Subject Number"
    And I click on "Hamburger" button
    And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_eCOA Compliance (Unblinded)" in ".pdf" format file to save in Export Evidence folder
    And hamburger menu is displayed with the following functionality for export
		| ButtonName |  
		| Excel      |
		| CSV        |
		| PDF        |
		| Print      |
    And I click on "Site Id" link
    And I click on the background screen
    And "Site Id" column is not visible in "eCOA Compliance (Unblinded)" grid
    And following data is displayed in "eCOA Compliance (Unblinded)" Grid
        | Subject Number | Enrollment Visit | Screening Visit | Treatment Visit  | Total Compliance |
        | S-10001-004    | 100.00           | 50.00           | -                | 3/6              |  
        | S-10001-005    | -                | -               | 50.00            | 1/6              |
