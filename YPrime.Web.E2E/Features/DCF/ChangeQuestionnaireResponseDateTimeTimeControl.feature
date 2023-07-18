@Portal
Feature: Change Questionnaire Response DateTime Time Control
	
Background:
    Given Software Release "Config release 39" has been created with Software Version "1.0.0.0" and Configuration Version "39.0-6.10" and assigned to Study Wide
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Patient "patient 1" with patient number "S-10001-010" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-010" has completed "Date Questionnaire" questionnaire for question "What time or date did your symptoms start?" and value "" 
    And Subject "S-10001-010" has completed "Date Questionnaire" questionnaire for question "What time did your symptoms start?" and value ""

@ID:5192 @ID:5791
#1
Scenario: Verify that user can’t enter value manually for Time and Date/Time Control
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-010" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Date Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value           | Fieldtype      |
        | What time or date did your symptoms start? |               | Please provide a response | datetimepicker |
        | What time did your symptoms start?         |               | Please provide a response | timepicker     |
    And I enter "22/June/2022 02:37 PM" for "What time or date did your symptoms start?" question
    And I enter "02:37 PM" for "What time did your symptoms start?" question
    Then the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value           | Fieldtype      |
        | What time or date did your symptoms start? |               | Please provide a response | datetimepicker |
        | What time did your symptoms start?         |               | Please provide a response | timepicker     |

@ID:5192 @ID:5791
#2
Scenario: Verify that DateTime Spinner format DD/MMMM/YYYY is displayed as configured and 12 hour format is displayed for DateTime and Time picker controls  
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-010" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Date Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value           | Fieldtype      |
        | What time or date did your symptoms start? |               | Please provide a response | datetimepicker |
        | What time did your symptoms start?         |               | Please provide a response | timepicker     |
    And I select "(Current date time)" as new value from "What time or date did your symptoms start?" "datetimepicker"
    And I select "(Current time)" as new value from "What time did your symptoms start?" "timepicker"
    Then the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value     | Fieldtype      | DateTimeFormat        |
        | What time or date did your symptoms start? |               | (Current date time) | datetimepicker | dd/MMMM/yyyy hh:mm tt |
        | What time did your symptoms start?         |               | (Current time)      | timepicker     | hh:mm tt              |

@ID:5192 @ID:5791
#3
Scenario: Data is persisted when user clicks back and next button on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-010" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Date Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value           | Fieldtype      |
        | What time or date did your symptoms start? |               | Please provide a response | datetimepicker |
        | What time did your symptoms start?         |               | Please provide a response | timepicker     |
    And I select "(Current date time)" as new value from "What time or date did your symptoms start?" "datetimepicker"
    And I select "(Current time)" as new value from "What time did your symptoms start?" "timepicker"
    And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label                                      | Current Value | Requested Value     | Fieldtype | DateTimeFormat        |
        | What time or date did your symptoms start? |               | (Current date time) | text      | dd/MMMM/yyyy hh:mm tt |
        | What time did your symptoms start?         |               | (Current time)      | text      | hh:mm tt              |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
        | Label                                      | Current Value | Requested Value     | Fieldtype      | DateTimeFormat        |
        | What time or date did your symptoms start? |               | (Current date time) | datetimepicker | dd/MMMM/yyyy hh:mm tt |
        | What time did your symptoms start?         |               | (Current time)      | timepicker     | hh:mm tt              |
    And "DateTime Time" is displayed in "Reason For Correction" inputtextbox field
