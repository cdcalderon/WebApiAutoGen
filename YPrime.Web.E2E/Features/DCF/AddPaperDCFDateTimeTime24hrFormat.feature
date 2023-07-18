@Portal
Feature: Add Paper DCF DateTime Time 24hr Format
	
Background: 
    Given Site "India Site" is assigned to Country "India" and has site number "20001"
    And User "PortalE2EUser" with role "YP" has access to site "20001"
    And Subject "S-20001-011" exists in the "Patients" table
	And Patient "patient 1" with patient number "S-20001-011" is associated with "India Site"
    And I have logged in with user "PortalE2EUser"
    And Software Release "Config release 39" has been created with Software Version "1.0.0.0" and Configuration Version "39.0-6.10" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And "Phone" Device "YP-E2E-Device" is assigned to Site "India Site"   
    And I click on "Add New DCF" button



@ID:5191
#1
Scenario: Verify that if Site is equal to India, 24 hour format is displayed for DateTime and Time picker Control  
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "India Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-20001-011" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "Date Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                      | Value                     | Fieldtype      |
        | Date of Questionnaire Completion:          | Please provide a response | datepicker     |
        | Visit Name:                                | Please provide a response | dropdown       |
        | What time or date did your symptoms start? | Please provide a response | datetimepicker |
        | What time did your symptoms start?         | Please provide a response | timepicker     |
    And I select "(Current date time)" from "What time or date did your symptoms start?" "datetimepicker"
    And I select "(Current time)" from "What time did your symptoms start?" "timepicker"
    Then the following data is displayed in the data correction field
        | Label                                      | Value                     | Fieldtype      | DateTimeFormat       |
        | Date of Questionnaire Completion:          | Please provide a response | datepicker     |                      |
        | Visit Name:                                | Please provide a response | dropdown       |                      |
        | What time or date did your symptoms start? | (Current date time)       | datetimepicker | dd/MMMM/yyyy HH:mm   |
        | What time did your symptoms start?         | (Current time)            | timepicker     |     HH:mm            |


@ignore
@ID:6711
    #8
    Scenario: Verify that when the user selects the following "submitting," validation message is displayed, and verify that when approval is selected, DCF is submitted. When the decline is selected, DCF is not submitted. Once the new paper dcf is submitted, the config version should be displayed on the questionnaire management page for 24hr format.
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
        And I click on "Approve" button
        And a pop up is displayed
         | popupType  | Message                                                                                     | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd/MMMM/yyyy HH:mm] | Approve, Decline | Approving   |
        And I click "Decline" button in the popup
        And I am on "Submit Data Correction" page
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                     | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd/MMMM/yyyy HH:mm] | Approve, Decline | Approving   |
        When I click on "Approve" button in the popup
        Then I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."