@DataGrid
Feature: DataGrid

Verify the Data Grid functionalities
NLA-86

#1
@MockStudyBuilder
#DATGR001.20, 50, 60, 80
Scenario: Verify the Data Grid for dropdown functionality, headers, searchbox, and hamburger button
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And I have added "12" patients to grid starting with "Patient 1" number "S-10000-001" and associated with "Site 1"
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	When "10" records are displayed in "Active Grid Table" data grid
    Then the following choices are displayed in "Active Patient dropdown" dropdown
		| Value |
		| 10	|
		| 25    |
		| 50    |
		| 100   |
	And I click on "Hamburger Menu" button
	And hamburger menu is displayed with the following functionality for export
		| ButtonName	|
		| Excel			|
		| CSV 		    |
		| PDF      		|
		| Print			|
	And hamburger menu is displayed with the following functionality for Visibilty
		| ButtonName				 |
		| Site Name                  |
		| Subject Number             |
		| Subject Status             |
		| Enrollment Date            |
		| Compliance                 |
		| Handheld Training Complete |
		| Tablet Training Complete   |
		| Last Diary Date            |
		| Last Sync Date             |
	And I click on "Site Name" link
	And I click on "PDF" button to generate "YPrime Study Portal" in ".pdf" format file to save in Export Evidence folder
	And I click on background screen
	And "Site Name" column of subject index grid is not "Visible"
	And "S-10000-011" link is "Not Visible"
	And I click on "Next" link
	And "S-10000-011" link is "Visible"
	And I click on "Previous" link
	And I enter "010" in "Subject Number Input" textbox field
	And "1" records are displayed in "Active Grid Table" data grid
	And "S-10000-010" link is "Visible"
	And I clear text field "Subject Number Input"
	
#2	
@MockStudyBuilder
 #DATGR001.40, 70
Scenario: Verify the Data Grid for sorting and total records
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And I have added "3" patients to grid starting with "patient 1" number "S-10000-001" and associated with "Site 1"
	And Diary Entry "diary 1" is associated with "patient 1"
	And Diary Entry "diary 2" is associated with "patient 2"
	And Diary Entry "diary 3" is associated with "patient 3"
	And I update the patient details as follows
		| Subject Number | Subject Status           | Enrollment date | Compliance | Handheld Training Complete | Tablet Training Complete | Last Diary Date | Last Sync Date |
		| S-10000-001    | Screened                 | Yesterday       | True       | True                       | True                     | 21-Dec-2021     | 20-Dec-2021    |
		| S-10000-002    | Enrolled                 | 10-Feb-2019     | False      | False                      | False                    | 10-Jan-2020     | 1-Jan-2021     |
		| S-10000-003    | Provided Written Consent | 22-Jan-2020     | False      | True                       | Flase                    | 03-Mar-2021     | 25-Feb-2021    |
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	When I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page
	Then I click on Headers of "Active Grid Table" to verify the sorting order
		| ColumnName                 |
		| Subject Number             |
		| Subject Status             |
		| Enrollment Date            |
		| Compliance                 |
		| Handheld Training Complete |
		| Tablet Training Complete   |
		| Last Diary Date            |
		| Last Sync Date             |
	And "3" entries under the table grid displayed in "Patients Grid info" 