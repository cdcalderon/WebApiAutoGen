@ignore
Feature: Data Capture
	#Add new columns in every dbo table in eCOA

@ID:577
Scenario: When new paper dcf is submitted, then the Correction table will show the user performing the action
        Given Software Release "Initial Release" has been created with Software Version "1.0.0" and Configuration Version "1.0-1.0"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
        And Subject "S-10001-003" is assigned to "YP-E2E-Device" Device
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And "Next" button is disabled
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        And the following data is displayed in the data correction field
            | Label                             | Value         | Fieldtype  |
            | Date of Questionnaire Completion: |               | datepicker |
            | Visit Name:                       | Please Select | dropdown   |
            | How would you say your health is: | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "How would you say your health is:" question
        And the following choices are displayed in dropdown for "How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
        And I select "Much Better" from "How would you say your health is:" "dropdown"
        And I click on "datepicker" in data correction field for "Date of Questionnaire Completion:" question
        And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
        And I enter "Paper DCF" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
            | Label                                         | Value                                         | Fieldtype |
            | Questionnaire:                                | Questionnaire Forms                           | text      |
            | Date of Questionnaire Completion              | (Current Date)                                | text      |
            | Visit Name                                    |                                               | text      |
            | How would you say your health is:             | Much Better                                   | text      |
        And I click on "Submit" button
        And I click on "Approve" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Approving   |
        And I click "Decline" button in the popup
        And I am on "Submit Data Correction" page
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Approving   |
        And I click on "Approve" button in the popup
        And I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."
        And I click "Ok" button in the popup
        When the POST request is made and the response is successful
	    Then Correction table has new record for following data
	      | username      | FirstName  | LastName | DCF# | Date/Time Stamp with UTC | AuditUserID      | LastModified | LastModifiedbyDatabaseUser |
	      | PortalE2EUser | PortalUser | YPUser   | 0067 | (Current Date) -4:00     | YP-Study-User-ID | Current date | Null                       |


@ID:577
Scenario: When data sync is successful, Device table includes the following record. 
    Given API request contains 
           | Device Type | Patient | AssetTag    | Questionnaire |
           | Phone       |         | YP-E2EPhone | Daily Dairy   |
    And the request is made to "sync client data" endpoint  
    When the POST request is made and the response is successful
    Then Device table has new record for following data
           | AuditUserID | LastModified | LastModifiedbyDatabaseUser |
           | DeviceID    | Current date | Null                       |  
