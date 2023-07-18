@ignore
@Portal


Feature: Site User Access



@ID:4266
#1
Scenario: Verify that if the user has access to Site 1 and Site 2, Site 2 access is removed then the data(grid) from Site 2 shouldn’t be displayed. 
	Given I have logged in with user "PortalE2EUser"
	And User "PortalE2EUser" with new role "YP" has access to site "10000"
	And User "PortalE2EUser" with new role "YP" has access to site "20000"
	And Subject with subject number "S-10000-125" is associated to site "10000"
	And I click on "Subject" link on the top navigation bar
	And I click on "Subject 10000-125"
	And I am on Subject "S-10000-125" page
	And User "PortalE2eUser" with role "YP" access to site "10000" is disabled
	When I navigate to "Subject" "S-10000-125" page 
	Then Page title is "403 - Forbidden: Access is denied"


@ID:4266
#2
Scenario: Verify that if the user has access to Site 1 and Site 2, Site 2 access is removed from the reports dropdown and the patient data is not displayed. 
	Given I have logged in with user "PortalE2EUser"
	And User "PortalE2EUser" with new role "YP" has access to site "10000"
	And User "PortalE2EUser" with new role "YP" has access to site "20000"
	And Subject with subject number "S-10000-125" is associated to site "10000"
	And I click on "Anayltics & Reports" link on the top navigation bar
	And I click on "Reports" link
	And I click on "Answer Audit Report"
	And I select site "10000" from "Please Select a Site" dropdown
	And I select Subject "S-10000-125" from "Please Select a Subject Number" dropdown
	And I click on "Display Report" button
	And following data is displayed in "Answer Audit Report" Grid
     | Protol    | Site Number | Subject Number | Diary Date   | Questionnaire | Questions     | Old Value | New Value | Change Reason Type | Change by | Correction Reason | DCF Number | Asset tag     |
     | eCOA_BYOD | 10000       | S-10000-125    | Current Date | Daily Diary   | Level Of Pain |           | 4         | New                |           |                   | #001       | YP-E2E-Device |      
	And User "PortalE2EUser" with role "YP" access to site "10000" is disabled
	And I click on "Anayltics & Reports" link on the top navigation bar
	And I click on "Reports" link
	And I click on "Answer Audit Report"
	When I click on "Please Select a Site" dropdown 
	Then site "10000" is not displayed in "Please Select a Site" dropdown 


@ID:4266
#3
Scenario: Verify that if the user has access to Site 1 and Site 2, Site 2 access is removed then the navigation to the individual records(patient, DCF) 403 error should be displayed when the user clicks the link. 
	Given I have logged in with user "PortalE2EUser"
	And User "PortalE2EUser" with new role "YP" has access to site "10000"
	And User "PortalE2EUser" with new role "YP" has access to site "20000"
	And Subject with subject number "S-10000-126" is associated to site "10000"
	And I click on "Subject" link on the top navigation bar
	And I click on "Subject 10000-125"
	And I am on Subject "S-10000-125" page
    And I am on "Data Correction" page
	And I click on "Add New DCF" button
    And I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10000-125" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype     |
        | Date of Questionnaire Completion:                  |                 | datepicker    |
        | Visit Name:                                        |                 | dropdown      |
        | Please record your highest temperature for the day |                 | NumberSpinner |
    And User "PortalE2EUser" with role "YP" access to site "10000" is disabled
	And I navigate to "Subject" "S-10000-125" page 
	And Page title is "403 - Forbidden: Access is denied"
	When I navigate to "Data Correction"  "S-10000-125" page
	Then Page title is "403 - Forbidden: Access is denied"
	

@ID:4266
#4
Scenario: Scenario: Verify that if the user has access to Site 1 and Site 2, Site 2 access is removed then the navigation to the individual records(device management) 403 error should be displayed when the user clicks the link. 
	Given I have logged in with user "PortalE2EUser"
	And User "PortalE2EUser" with new role "YP" has access to site "10000"
	And User "PortalE2EUser" with new role "YP" has access to site "2000
	And Subject with subject number "S-10000-126" is associated to site "10000"
	And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 10000"
	And "BYOD" Device "YP-E2EBYOD" is assigned to Site "Site 10000"
	And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 10000"
	And "1" Devices is displayed for "Phone"
	And "1" Devices is displayed for "Tablet"
	And "3" Devices is displayed for "Total Devices"
	And I am on "At a Glance" page
	And I click on "Hamburger icon" in "Device Inventory" widget
	And I click on View Devices button under Hamburger icon
	And I am on "Device Management" page
	And following data is displayed in the grid
		| Site Name  | Device Name | Device Type | Release Name    | Software Version | Configuration Version | Last Reported Software Version | Last Reported configuration Version | Last Data Sync |
		| Site 10000 | YP-E2EBYOD  | BYOD        | Initial Release | 1.0.0.0          | 1.0-0.4               | 1.0.0.0                        | 1.0-0.4                             | (Current Date) |
	And I click on "YP-E2EBYOD" link
	And I am on "Device Details" page
	And following data is displayed on Device Details page
		| Field                               | Value          |
		| Site Id                             | 10000          |
		| Device Type Name                    | BYOD           |
		| Assigned Software Version           | 1.0.0.0        |
		| Last Reported Software Version      | 1.0.0.0        |
		| Assigned Configuration Version      | 1.0-0.4        |
		| Last Reported Configuration Version | 1.0-0.4        |
		| Last Data Sync Date                 | (Current Date) |
	And User "PortalE2EUser" with role "YP" access to site "10000" is disabled
	When I navigate to "Device Management" for "YP-E2EBYOD" page
	Then Page title is "403 - Forbidden: Access is denied"




