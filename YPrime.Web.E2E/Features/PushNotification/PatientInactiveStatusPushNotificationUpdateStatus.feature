Feature: Portal Updates Update Status

#When the patient status is changed to inactive in Portal the push notification will be updated to inactive

	Background:
		Given Site "Site 1" is assigned to Country "United States" and has site number "100000"        
        And User "PortalE2EUser" with role "YP" has access to site "100000"
        And "working" Configuration Version has the following "Subject Information" attributes:
            | Label         | Field Type   |
            | Subject       | Numberinput  |
            | Gender        | Radio Button |
            | Date of Birth | datepicker   |
		And Patient "patient 1" with status "Enrolled" and patient number "S-10001-001" is associated with "Site 1"
        And Patient "patient 2" with status "Enrolled" and patient number "S-10001-002" is associated with "Site 1"
        And Patient "patient 3" with status "Enrolled" and patient number "S-10001-003" is associated with "Site 1"
        And Patient "patient 4" with status "Enrolled" and patient number "S-10001-004" is associated with "Site 1"
        And Patient "patient 4" has the following subject attributes:
            | Label         | Value          |
            | Gender        | Female         |
            | Date of birth | (Current Date) |
        And Patient "patient 5" with status "Enrolled" and patient number "S-10001-004" is associated with "Site 1"
        And Patient "patient 5" has the following subject attributes:
            | Label         | Value          |
            | Gender        | Female         |
            | Date of birth | (Current Date) |

        And DCF workflow approval "No Approval Needed" is "enabled" for DCF type "Change subject Information" for "working" Configuration Version
        And DCF workflow approval "No Approval Needed" is "enabled" for DCF type "Merge subjects" for "working" Configuration Version
        And DCF workflow approval "No Approval Needed" is "enabled" for DCF type "Remove a subject" for "working" Configuration Version


@ID:2511 @MockStudyBuilder
#1
Scenario: Verify that when patient status is set to inactive status in portal then the patients push notifications are cancelled.
        Given I am logged in as "PortalE2EUser"
        And Notification Schedule endpoint is up
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-10001-001"
        And I am on Subject "S-10001-001" page
        And I click on "Change Subject Status" button
        And I click on "Patient Status" dropdown
        And I select "Screen Failed" from "Patient Status" dropdown
        And "Screen Failed" is displayed for "Patient Status" dropdown
        When I click on "Save" button
        Then "Success" popup is displayed with message "Subject S-10001-001 has been updated successfully."
        And Subject "S-10001-001" is in "inactive" status
        And Update status request is made


  @ID:2511 @MockStudyBuilder
  #2
  Scenario: Verify that when patient status is set to inactive status from Change Subject Information DCF type, then patients push notification are cancelled once DCF is completed
        Given I am logged in as "PortalE2EUser"
        And Notification Schedule endpoint is up
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-10001-002"
        And I am on Subject "S-10001-002" page
        And I click on "Data Correction" button
        And I click on "Type Of Correction" dropdown
        And I select "Change subject Information" from "Type Of Correction" dropdown
        And the following data is displayed in the data correction field for "Change subject Information" type of correction
         | Label          | Value          | Fieldtype        |
         | Subject Number | 002            | text             |
         | Current Status | Enrolled       | radiobuttons1    |
        And I select "Screen Failed" for Current Status
        And "Screen Failed" status is selected for data correction field
        And I enter "Subject Status Update" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
         | Label          | Value           | Fieldtype |
         | Subject Number |                 | text      |
         | Current Status | Screen Failed   | text      |
        And I click on "Submit" button
        And "Electronic Signature" popup is displayed
        And "User Name" has value "PortalE2EUser"
        And I enter "Welcome01!" for "Password"
        When I click "Ok" button in the popup
        Then I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."
        And Update status request is made


  @ID:2511 @MockStudyBuilder
  #3
  Scenario: Verify that when patient status is set to inactive status from Remove Subject DCF type, then patients push notifications are cancelled once DCF is completed
        Given I am logged in as "PortalE2EUser"
        And Notification Schedule endpoint is up
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-10001-003"
        And I am on Subject "S-10001-003" page
        And I click on "Data Correction" button
        And I click on "Type Of Correction" dropdown
        And I select "Remove a subject" from "Type Of Correction" dropdown
        And "The selected subject will be removed." text is displayed
        And I enter "Subject Removed" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
            | Label  | Value     | Fieldtype |
            | Remove | Removed   | text      |
        And I click on "Submit" button
        And "Electronic Signature" popup is displayed
        And "User Name" has value "PortalE2EUser"
        And I enter "Welcome01!" for "Password"
        When I click "Ok" button in the popup
        Then I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."
        And Update status request is made


 @ID:2511 @MockStudyBuilder
 #4
 Scenario: Verify that when patient status is set to inactive status from Merge Subject DCF type, then patients push notification are cancelled once DCF is completed.
        Given I am logged in as "PortalE2EUser"
        And Notification Schedule endpoint is up
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-10001-004"
        And I am on Subject "S-10001-004" page
        And I click on "Data Correction" button
        And I click on "Type Of Correction" dropdown
        And I select "Merge subjects" from "Type Of Correction" dropdown
        And the following data is displayed in the data correction field for "Merge subjects" type of correction
         | Label         | Value          | Fieldtype |
         | Gender        | Female         | text      |
         | Date of Birth | (Current Date) | date      |
         | Gender        | Female         | text      |
         | Date of Birth | (Current Date) | date      |
        And "Please select the primary subject to be merged to." text is displayed
        And I select "Subject 1" in data correction field
        And I enter "Merge subjects" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
            | Label         | Value         | Fieldtype |
            | Change Status |               | text      |
            | Change Status | Removed       | text      |
        And I click on "Submit" button
        And "Electronic Signature" popup is displayed
        And "User Name" has value "PortalE2EUser"
        And I enter "Welcome01!" for "Password"
        When I click "Ok" button in the popup
        Then I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."
        And Update status request is made



 @ID:2511 @MockStudyBuilder 
 #5
 Scenario: Verify that when patient status is set to inactive status in portal and the patients push notifications request fails.
        Given I am logged in as "PortalE2EUser"
        And Notification Schedule endpoint is down
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-10001-003"
        And I am on Subject "S-10001-003" page
        And I click on "Change Subject Status" button
        And I click on "Patient Status" dropdown
        And I select "Screen Failed" from "Patient Status" dropdown
        And "Screen Failed" is displayed for "Patient Status" dropdown
        When I click on "Save" button
        Then "Success" popup is displayed with message "Subject S-10001-003 has been updated successfully."
        And Subject "S-10001-003" is in "inactive" status
        And Update status request is made
        And Update status request fails
