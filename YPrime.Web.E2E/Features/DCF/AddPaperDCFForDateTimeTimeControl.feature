@Portal
Feature: AddPaperDCF For Date DateTime Time Control
	
Background:     
    Given Software Release "Config release 39" has been created with Software Version "1.0.0.0" and Configuration Version "39.0-6.10" and assigned to Study Wide
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-009" exists in the "Patients" table
    And Patient "patient 1" with patient number "S-10001-009" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"


@ID:5191
#1
Scenario: Verify that user can't enter value manually in the Date/Time and Time Picker 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-009" from "Subject" dropdown
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
    And I enter "22/June/2022 02:37 PM" for "What time or date did your symptoms start?" question in PaperDCF
    When I enter "02:37 PM" for "What time did your symptoms start?" question in PaperDCF
    Then the following data is displayed in the data correction field
        | Label                                      | Value                     | Fieldtype      |
        | Date of Questionnaire Completion:          | Please provide a response | datepicker     |
        | Visit Name:                                | Please provide a response | dropdown       |
        | What time or date did your symptoms start? | Please provide a response | datetimepicker |
        | What time did your symptoms start?         | Please provide a response | timepicker     |


@ID:5191
#2
Scenario: Verify that DateTime Spinner format DD/MMMM/YYYY is displayed as configured and 12 hour format is displayed for DateTime and Time picker controls  
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-009" from "Subject" dropdown
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
        | What time or date did your symptoms start? | (Current date time)       | datetimepicker | dd/MMMM/yyyy hh:mm tt|
        | What time did your symptoms start?         | (Current time)            | timepicker     | hh:mm tt             |

@ID:5191
#3
Scenario: Data Persisted when user clicks Back and Next button on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-009" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "Date Questionnaire" from "Select a Questionnaire" dropdown
    Then the following data is displayed in the data correction field
        | Label                                      | Value                     | Fieldtype      |
        | Date of Questionnaire Completion:          | Please provide a response | datepicker     |
        | Visit Name:                                | Please provide a response | dropdown       |
        | What time or date did your symptoms start? | Please provide a response | datetimepicker |
        | What time did your symptoms start?         | Please provide a response | timepicker     |
    And I select "(Current date time)" from "What time or date did your symptoms start?" "datetimepicker"
    And I select "(Current time)" from "What time did your symptoms start?" "timepicker"
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data field table
        | Label                                      | Value               | Fieldtype | DateTimeFormat       |
        | Questionnaire:                             | Date Questionnaire  | text      |                      |
        | Date of Questionnaire Completion           | (Current Date)      | text      |                      |
        | Visit Name                                 |                     | text      |                      |
        | What time or date did your symptoms start? | (Current date time) | text      | dd/MMMM/yyyy hh:mm tt|
        | What time did your symptoms start?         | (Current time)      | text      | hh:mm tt             |
    When I click on "Back" button
    Then the following data is displayed in the data correction field
        | Label                                      | Value                     | Fieldtype      | DateTimeFormat       |
        | Date of Questionnaire Completion:          | (Current Date)            | datepicker     |                      |
        | Visit Name:                                | Please provide a response | dropdown       |                      |
        | What time or date did your symptoms start? | (Current date time)       | datetimepicker | dd/MMMM/yyyy hh:mm tt|
        | What time did your symptoms start?         | (Current time)            | timepicker     | hh:mm tt             |
    And "DateTime Time" is displayed in "Reason For Correction" inputtextbox field


    



    


     