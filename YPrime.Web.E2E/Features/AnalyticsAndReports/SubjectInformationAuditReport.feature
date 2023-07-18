@SubjectInformationAuditReport
Feature: SubjectInformationAuditReport
#NLA-133
#Subject Information Audit Report

Background: 
      Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
        And Site "Site 1" is assigned to Country "United States" and has site number "10000"
        And User "PortalE2EUser" with role "YP" has access to site "10000"
        And User "PortalE2EUser" with role "YP" has access to site "10001"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page


@MockStudyBuilder
#1 
#SIAR.01,02,03,04,05,07,12
#RR.01,02,03
Scenario: Verify title on page is correct, Grid, Search boxes, hamburger menu and coloumn visibility are working fine
      Given "Subject Information Audit Report" report visibility status is "Enabled"
        And I click on "Subject" link on top navigation bar
        And I am on "Subject Information" page
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 203                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
        And I click on "No, subject will use a provisioned device" button
        And I click on "Language" dropdown
        And I select "English(United States)" from "Language" dropdown
        And I click on "Create" button
        And I click "Ok" button in the popup
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Subject Information Audit Report" from the Report List
        And "site Number" dropdown is "Visible"
        And I click at "Initial Site" from "site Number" dropdown
        And I click at "S-10001-203" from "Subject Number" dropdown
       When I click on "Display the report" button
       Then "Subject Information Audit Report (Subject number: S-10001-203)" report title is displayed on screen
        And following data is displayed in "Subject Information Audit Report" Grid
            | Protocol             | Site Number | Subject Number | Subject Attribute              | Old Value | New Value                  | Change Reason Type | Changed By    | Changed Date | Correction Reason | DCF Number | Asset Tag |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Current Status                 |           | Screened                   | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Date of Birth                  |           | (dd-MMMM-yyyy)Current Date | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Enrolled Date                  |           | Current Date               | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Gender                         |           | Female                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Height                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Is Handheld Training Completed |           | No                         | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Language                       |           | English (United States)    | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | PIN                            |           | ######                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Subject Number                 |           | S-10001-203                | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-203    | Weight                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           | 
        And I click on "Hamburger" button
	    And hamburger menu is displayed with the following functionality for export
		    | ButtonName	|
		    | PDF			|
        When I click on "Subject Number" link
        Then "Subject Number" column is not visible in "Subject Information Audit Report" grid
        And following data is displayed in "Subject Information Audit Report" Grid
            | Protocol             | Site Number | Subject Attribute              | Old Value | New Value                  | Change Reason Type | Changed By    | Changed Date | Correction Reason | DCF Number | Asset Tag |
            | YPrime_eCOA-E2E-Mock | 10001       | Current Status                 |           | Screened                   | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Date of Birth                  |           | (dd-MMMM-yyyy)Current Date | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Enrolled Date                  |           | Current Date               | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Gender                         |           | Female                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Height                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Is Handheld Training Completed |           | No                         | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Language                       |           | English (United States)    | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | PIN                            |           | ######                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Subject Number                 |           | S-10001-203                | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | Weight                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           |        
        And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Subject Information Audit Report" in ".pdf" format file to save in Export Evidence folder
        And I click on the background screen
        And I enter "Language" in "Search_Subject Attribute" textbox field
	   When "1" records are displayed in "Subject Information Audit Report" data grid
       Then following data is displayed in "Subject Information Audit Report" Grid
            | Protocol             | Site Number | Subject Attribute | Old Value | New Value               | Change Reason Type | Changed By    | Changed Date | Correction Reason | DCF Number | Asset Tag |
            | YPrime_eCOA-E2E-Mock | 10001       | Language          |           | English (United States) | New                | PortalE2EUser | Current Date |                   |            |           |

@MockStudyBuilder
#2
#SIAR.08,09
Scenario: All subjects will be displayed regardless of subject status and duplicate subject should also be displayed
      Given Patient "patient 1" with status "Screened" and patient number "S-10001-321" is associated with "Initial Site"
   	    And Patient "patient 2" with status "Enrolled" and patient number "S-10001-321" is associated with "Initial Site" 
        And "Subject Information Audit Report" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Subject Information Audit Report" from the Report List
        And "site Number" dropdown is "Visible"
        And "Please select a site" is displayed for "site Number" dropdown
        And I click at "Initial Site" from "site Number" dropdown
        And "Please select a Subject number" is displayed for "Subject Number" dropdown
       When I click on "Subject Number" dropdown
       Then the following choices are displayed in "Subject Number" dropdown
            | Value                          |
            | Please select a Subject number |
            | S-10001-321                    |
            | S-10001-321                    |
        And I merge two subjects as one for "S-10001-321" patient number
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Subject Information Audit Report" from the Report List
        And "site Number" dropdown is "Visible"
        And I click at "Initial Site" from "site Number" dropdown
        And "Subject Number" dropdown is "Visible"
       When I click on "Subject Number" dropdown
       Then the following choices are displayed in "Subject Number" dropdown
            | Value                          |
            | Please select a Subject number |
            | S-10001-321                    |
        And I change Subject status from "Screened" to "Screen Failed" for "S-10001-321" patient number
        And I refresh page
        And I click on "Subject Information Audit Report" from the Report List
        And "site Number" dropdown is "Visible"
        And I click at "Initial Site" from "site Number" dropdown
        And "Subject Number" dropdown is "Visible"
       When I click on "Subject Number" dropdown
       Then the following choices are displayed in "Subject Number" dropdown
            | Value                          |
            | Please select a Subject number |
            | S-10001-321                    |

@MockStudyBuilder
#3 
#RR.01
Scenario: Verify if Subject Information Audit study is disabled then Subject Information Audit report should not be visible at Analytics & Report page
      Given "Subject Information Audit Report" report visibility status is "Disabled"
        When I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        Then "Subject Information Audit Report" link is "Not Visible"


@MockStudyBuilder
#4 
#SIAR.06,13
#RR.05,06
#Display Report with the no Site Number and /or Subject Number selected doesn't display the report
Scenario: Site Number and Subject Number filters are required and Report will be displayed only when site/Subject number is selected
      Given Patient "patient 1" with status "Screened" and patient number "S-10001-101" is associated with "Initial Site"
   	    And Patient "patient 2" with status "Enrolled" and patient number "S-10001-102" is associated with "Initial Site" 
        And Patient "patient 3" with status "Screened" and patient number "S-10000-103" is associated with "Site 1"
   	    And Patient "patient 4" with status "Enrolled" and patient number "S-10000-104" is associated with "Site 1"      
        And "Subject Information Audit Report" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Subject Information Audit Report" from the Report List
        And "Display the report" button is disabled
        And "site Number" dropdown is "Visible"
       When I click on "site Number" dropdown
       Then the following choices are displayed in "site Number" dropdown
            | Value                |
            | Please select a site |
            | All Sites            |
            | Initial Site         |
            | Site 1               |
        And I click at "Site 1" from "site Number" dropdown
        And "Display the report" button is disabled
        And "Subject Number" dropdown is "Visible"
       When I click on "Subject Number" dropdown
       Then the following choices are displayed in "Subject Number" dropdown
            | Value                          |
            | Please select a Subject number |
            | S-10000-103                    |
            | S-10000-104                    |
        And I click at "S-10000-103" from "Subject Number" dropdown
        And "Display the report" button is enabled
       When I click on "site Number" dropdown
       Then the following choices are displayed in "site Number" dropdown
            | Value                |
            | Please select a site |
            | All Sites            |
            | Initial Site         |
            | Site 1               |
        And I click at "Initial Site" from "site Number" dropdown
        And "Subject Number" dropdown is "Visible"
       When I click on "Subject Number" dropdown
       Then the following choices are displayed in "Subject Number" dropdown
            | Value                          |
            | Please select a Subject number |
            | S-10001-101                    |
            | S-10001-102                    |
        And I click at "S-10001-101" from "Subject Number" dropdown 
       When I click on "Display the report" button
       Then "Subject Information Audit Report (Subject number: S-10001-101)" report title is displayed on screen
#Since subject is created directly using Database so Changed By column will be showing as SYSTEM
        And following data is displayed in "Subject Information Audit Report" Grid
            | Protocol             | Site Number | Subject Number | Subject Attribute              | Old Value | New Value               | Change Reason Type | Changed By   | Changed Date | Correction Reason | DCF Number | Asset Tag |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-101    | Current Status                 |           | Screened                | New                | SYSTEM       | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-101    | Enrolled Date                  |           | Current Date            | New                | SYSTEM       | Current Date |                   |            |           | 
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-101    | Is Handheld Training Completed |           | No                      | New                | SYSTEM       | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-101    | Language                       |           | English (United States) | New                | SYSTEM       | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-101    | Subject Number                 |           | S-10001-101             | New                | SYSTEM       | Current Date |                   |            |           | 
           

@MockStudyBuilder
#5
#SIAR.11
Scenario: Subject status changes made on the portal as part of subject management is displayed in report.
      Given "Subject Information Audit Report" report visibility status is "Enabled"
        And I click on "Subject" link on top navigation bar
        And I am on "Subject Information" page
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 303                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
        And I click on "No, subject will use a provisioned device" button
        And I click on "Language" dropdown
        And I select "English(United States)" from "Language" dropdown
        And I click on "Create" button
        And I click "Ok" button in the popup 
        And I select Subject "S-10001-303"
        And I click on "Change Subject Status" button
        And I click at "Screen Failed" from "Patient status" dropdown
        And I click on "Save Patient Status" button
        And I click "Ok" button in the popup
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Subject Information Audit Report" from the Report List
        And "site Number" dropdown is "Visible"
        And I click at "Initial Site" from "site Number" dropdown
        And I click at "S-10001-303" from "Subject Number" dropdown
       When I click on "Display the report" button
       Then "Subject Information Audit Report (Subject number: S-10001-303)" report title is displayed on screen
        And following data is displayed in "Subject Information Audit Report" Grid
            | Protocol             | Site Number | Subject Number | Subject Attribute              | Old Value | New Value                  | Change Reason Type | Changed By    | Changed Date | Correction Reason | DCF Number | Asset Tag |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Current Status                 |           | Screened                   | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Date of Birth                  |           | (dd-MMMM-yyyy)Current Date | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Enrolled Date                  |           | Current Date               | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Gender                         |           | Female                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Height                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Is Handheld Training Completed |           | No                         | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Language                       |           | English (United States)    | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | PIN                            |           | ######                     | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Subject Number                 |           | S-10001-303                | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Weight                         |           | 100                        | New                | PortalE2EUser | Current Date |                   |            |           |
            | YPrime_eCOA-E2E-Mock | 10001       | S-10001-303    | Current Status                 | Screened  | Screen Failed              | Update             | PortalE2EUser | Current Date |                   |            |           | 
