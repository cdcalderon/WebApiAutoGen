@DCFStatusReport
Feature: DCF Status Report

#NLA-135
#Validate DCF Status Report at Analytics & Report page
Background:
	Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
	And User "PortalE2EUser" with role "YP" has access to site "10001"
	And Patient "patient 1" with status "Screened" and patient number "S-10001-125" is associated with "Initial Site"
	And Patient "patient 1" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Female         |
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |
	And Patient Visit "Visit 1" is associated with "patient 1"
	And Subject "S-10001-125" has completed "Questionnaire Forms" questionnaire for question "In general, How would you say your health is:" and choice "Not better"
	And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label                      | Value                |       
        | Diary Date                 |   Current Date       |
        | Started Time               |   Current Date       |
        | Completed Time             |   Current Date       |
        | Transmitted Time           |   Current Date       |
        |  Visit Name                |   Visit 1            |
	And I create a Data Correction with details as follows
		| Patient No  | DCF Type                         | DCF Status                     | DCF Action                     | DCF Opened Date | DCF Closed Date | Translation Key                                      | Reason For Correction    | Table Name       | Column Name              |
		| S-10001-125 | Change subject Information       | Completed                      | ApproveCorrection              | 30-Jan-2022     | 04-Feb-2022     | Height                                               | Subject Attribute Update | PatientAttribute | AttributeValue           |
		| S-10001-125 | Remove a subject                 | Rejected                       | RejectCorrection               | 05-Feb-2022     | 08-Feb-2022     | RemoveSubjectDisplay                                 | Remove Subject           | Patient          | PatientStatusTypeId      |
		| S-10001-125 | Merge subjects                   | Pending                        | PendingCorrection              | Current date    | N/A             | Subject Number                                       | Merge Subject            | Patient          | PatientNumber            |
		| S-10001-125 | Change questionnaire responses   | Completed                      | ApproveCorrection              | Current date    | Current date    | lblDate                                              | Update ques response     | Answer           | ChoiceId                 |
		| S-10001-125 | Change questionnaire information | Pending                        | PendingCorrection              | Current date    | N/A             | <p>In general, How would you say your health is:</p> | Update ques info         | DiaryEntry       | DiaryDate                |
		| S-10001-125 | Add Paper Questionnaire          | Rejected                       | RejectCorrection               | Current date    | Current date    | <p>In general, How would you say your health is:</p> | Add paper ques           | Answer           | ChoiceId                 |
		| S-10001-125 | Change subject Visit             | NeedsMoreInformationCorrection | NeedsMoreInformationCorrection | Current date    | N/A             | VisitStatusColon                                     | Visit date  Info         | PatientVisit     | PatientVisitStatusTypeId |

@MockStudyBuilder
#1
# RR.01 Availability and visibility of the report.
Scenario: If permission is disabled "DCF Status Report" report link will not be displayed
	Given I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page
	And "DCF Status Report" report visibility status is "Disabled"
	And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
	When I am on "Reports" page
	Then "DCF Status Report" report link is "Not Visible"

@MockStudyBuilder
#2
# RR.01 Availability and visibility of the report.
# REQ#DCFSR.03 This report will display a listing of all DCFs ever raised for a study.
# REQ#DCFSR.04 DCF Number: If user has permissions to “View DCFs”, the DCF number will be displayed as a selectable link. The link will take the user to the actual DCF.
# REQ#DCFSR.04 Site: Displays the site number for which the correction was raised.
# REQ#DCFSR.04 Subject: Displays the subject number for whom the correction was raised.
# REQ#DCFSR.04 DCF Type: Displays the type of change requested from the DCF types configured for the study.
# REQ#DCFSR.04 DCF Status: Will display current DCF status
# REQ#DCFSR.04 DCF Opened Date: Will display date the DCF was raised within the Portal system (UTC) DDMMMYYYY.
# REQ#DCFSR.04 DCF Closed Date: Will display the date that the DCF was moved to a status of “completed/rejected” (DDMMMYYYY). If DCF is in a status other than completed, this field will display, “N/A”.
# REQ#DCFSR.04 Pending Approver Group: If DCF is in a status of Pending, NeedsMoreInformation or InProgress, this field will populate with the name of the pending approver group. If the DCF is in a status of “completed” this field will display, “N/A”
# REQ#DCFSR.04 Completed Approvals: Displays a list of the individuals who approved the DCF to date.
# REQ#DCFSR.04 # of Days DCF Open: For DCFs with a status of “Completed/Rejected” the field will display the value of DCF Opened Date subtracted by the DCF Closed Date. For a DCF in any other status, the field will display DCF Opened Date subtracted by Current date.
Scenario: Verify DCF Status Report visibility and DCF Status Report page.
	Given I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page
	And "CAN VIEW THE LIST OF DCFS." permission is "Enabled"
	And "DCF Status Report" report visibility status is "Enabled"
	And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
	When I click on "DCF Status Report" from the Report List
	Then "DCF Status Report" is "Visible"
	# DCF Number Format are validated in backend code in number format (0569) as it is auto generated and dynamic during dcf creation
	And following data is displayed in "DCF Status Report" Grid
		| DCF Number   | Site  | Subject     | DCF Type                         | DCF Status             | DCF Opened Date | DCF Closed Date | Pending Approver Group | Completed Approvals | # of Days DCF Open |
		| DCF Report 1 | 10001 | S-10001-125 | Change subject Information       | Completed              | 30-Jan-2022     | 04-Feb-2022     | N/A                    | PortalE2EUser       | 6                  |
		| DCF Report 2 | 10001 | S-10001-125 | Remove a subject                 | Rejected               | 05-Feb-2022     | 08-Feb-2022     | N/A                    | N/A                 | 4                  |
		| DCF Report 3 | 10001 | S-10001-125 | Merge subjects                   | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| DCF Report 4 | 10001 | S-10001-125 | Change questionnaire responses   | Completed              | Current Date    | Current Date    | N/A                    | PortalE2EUser       | 1                  |
		| DCF Report 5 | 10001 | S-10001-125 | Change questionnaire information | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| DCF Report 6 | 10001 | S-10001-125 | Add Paper Questionnaire          | Rejected               | Current Date    | Current Date    | N/A                    | N/A                 | 1                  |
		| DCF Report 7 | 10001 | S-10001-125 | Change subject Visit             | Needs More Information | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
	And the following DCF number will "Display" as hyperlink for "DCF Type"
		| DCFNumber    | DCFType                          |
		| DCF Report 1 | Change subject Information       |
		| DCF Report 2 | Remove a subject                 |
		| DCF Report 3 | Merge subjects                   |
		| DCF Report 4 | Change questionnaire responses   |
		| DCF Report 5 | Change questionnaire information |
		| DCF Report 6 | Add Paper Questionnaire          |
		| DCF Report 7 | Change subject Visit             |
	And I click on the "DCF Report 1 link" for DCF Type "Change subject Information"
	And I navigate to "DCF Report 1" actual details for DCF Type "Change subject Information"

@MockStudyBuilder
#3
# REQ#DCFSR.04 DCF Number: If user does not have “View DCFs” permission, the hyperlink will not be displayed.
Scenario: Verify DCF Number hyperlink visibility.
	Given I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page
	And "CAN VIEW THE LIST OF DCFS." permission is "Disabled"
	And "DCF Status Report" report visibility status is "Enabled"
	And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
	When I click on "DCF Status Report" from the Report List
	Then "DCF Status Report" is "Visible"
	And following data is displayed in "DCF Status Report" Grid
		| DCF Number   | Site  | Subject     | DCF Type                         | DCF Status             | DCF Opened Date | DCF Closed Date | Pending Approver Group | Completed Approvals | # of Days DCF Open |
		| DCF Report 1 | 10001 | S-10001-125 | Change subject Information       | Completed              | 30-Jan-2022     | 04-Feb-2022     | N/A                    | PortalE2EUser       | 6                  |
		| DCF Report 2 | 10001 | S-10001-125 | Remove a subject                 | Rejected               | 05-Feb-2022     | 08-Feb-2022     | N/A                    | N/A                 | 4                  |
		| DCF Report 3 | 10001 | S-10001-125 | Merge subjects                   | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| DCF Report 4 | 10001 | S-10001-125 | Change questionnaire responses   | Completed              | Current Date    | Current Date    | N/A                    | PortalE2EUser       | 1                  |
		| DCF Report 5 | 10001 | S-10001-125 | Change questionnaire information | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| DCF Report 6 | 10001 | S-10001-125 | Add Paper Questionnaire          | Rejected               | Current Date    | Current Date    | N/A                    | N/A                 | 1                  |
		| DCF Report 7 | 10001 | S-10001-125 | Change subject Visit             | Needs More Information | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
	And the following DCF number will "Not Display" as hyperlink for "DCF Type"
		| DCFNumber    | DCFType                          |
		| DCF Report 1 | Change subject Information       |
		| DCF Report 2 | Remove a subject                 |
		| DCF Report 3 | Merge subjects                   |
		| DCF Report 4 | Change questionnaire responses   |
		| DCF Report 5 | Change questionnaire information |
		| DCF Report 6 | Add Paper Questionnaire          |
		| DCF Report 7 | Change subject Visit             |
	And I click on the "DCF Report 1" for DCF Type "Change subject Information"
	And I am on "Reports" page

@MockStudyBuilder
#4
# RR.02 System will display the following functionality on all reports:Report Title, Search,Visibility ,Export
# RR.03 All reports will include a “hamburger” icon that when selected will allow a user to remove columns from the report display by deselecting them from “Visibility”, all columns will always display be default.
Scenario: Verify title on page is correct, Search boxes, export, hamburger menu and coloumn visibility are working fine
	Given I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page
	And "DCF Status Report" report visibility status is "Enabled"
	And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
	When I click on "DCF Status Report" from the Report List
	Then "DCF Status Report" report title is displayed on screen
	And I enter "Completed" in "Search_DCFStatus" textbox field
	And "2" records are displayed in "DCF Status Report" data grid
	And following data is displayed in "DCF Status Report" Grid
		| DCF Number   | Site  | Subject     | DCF Type                       | DCF Status | DCF Opened Date | DCF Closed Date | Pending Approver Group | Completed Approvals | # of Days DCF Open |
		| DCF Report 1 | 10001 | S-10001-125 | Change subject Information     | Completed  | 30-Jan-2022     | 04-Feb-2022     | N/A                    | PortalE2EUser       | 6                  |
		| DCF Report 4 | 10001 | S-10001-125 | Change questionnaire responses | Completed  | Current Date    | Current Date    | N/A                    | PortalE2EUser       | 1                  |
	And I click on "Hamburger Menu" button for DCF Report
	And grid menu for DCF Report is displayed with the following functionality for export
		| ButtonName |
		| Excel      |
		| CSV        |
		| PDF        |
		| Print      |
	And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_DCF Status Report" in ".pdf" format file to save in Export Evidence folder for DCF Report
	And hamburger menu is displayed with the following functionality for Visibility
		| ButtonName             |
		| DCF Number             |
		| Site                   |
		| Subject                |
		| DCF Type               |
		| DCF Status             |
		| DCF Opened Date        |
		| DCF Closed Date        |
		| Pending Approver Group |
		| Completed Approvals    |
		| # of Days DCF Open     |
	And I click on "DCF Number" link
	And I click on the background screen
	And "DCF Number" grid column is not visible
	And I enter " " in "Search_DCFStatus" textbox field
	And following data is displayed in "DCF Status Report" Grid
		| Site  | Subject     | DCF Type                         | DCF Status             | DCF Opened Date | DCF Closed Date | Pending Approver Group | Completed Approvals | # of Days DCF Open |
		| 10001 | S-10001-125 | Change subject Information       | Completed              | 30-Jan-2022     | 04-Feb-2022     | N/A                    | PortalE2EUser       | 6                  |
		| 10001 | S-10001-125 | Remove a subject                 | Rejected               | 05-Feb-2022     | 08-Feb-2022     | N/A                    | N/A                 | 4                  |
		| 10001 | S-10001-125 | Merge subjects                   | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| 10001 | S-10001-125 | Change questionnaire responses   | Completed              | Current Date    | Current Date    | N/A                    | PortalE2EUser       | 1                  |
		| 10001 | S-10001-125 | Change questionnaire information | Pending                | Current Date    | N/A             | Yp                     | N/A                 | 1                  |
		| 10001 | S-10001-125 | Add Paper Questionnaire          | Rejected               | Current Date    | Current Date    | N/A                    | N/A                 | 1                  |
		| 10001 | S-10001-125 | Change subject Visit             | Needs More Information | Current Date    | N/A             | Yp                     | N/A                 | 1                  |