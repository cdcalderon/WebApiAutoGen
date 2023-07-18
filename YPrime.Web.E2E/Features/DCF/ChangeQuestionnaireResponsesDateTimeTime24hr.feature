@Portal


Feature: Change Questionnaire Responses DateTime Time 24hr

Background:
    Given I have logged in with user "PortalE2EUser"
    And Site "India Site" is assigned to Country "India" and has site number "20001"
    And User "PortalE2EUser" with role "YP" has access to site "20001"
    And Patient "patient 1" with patient number "S-20001-013" is associated with "India Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "India Site"
    And Software Release "Config release 39" has been created with Software Version "1.0.0.0" and Configuration Version "39.0-6.10" and assigned to Study Wide
    And Subject "S-20001-013" has completed "Date Questionnaire" questionnaire for question "What time or date did your symptoms start?" and value "" 
    And Subject "S-20001-013" has completed "Date Questionnaire" questionnaire for question "What time did your symptoms start?" and value ""
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
	
@ID:5192 @ID:5791
#3
Scenario: Verify that if Site is equal to India, 24hour format is displayed for DateTime and Time picker Control 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "India Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-20001-013" from "Subject" dropdown
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
        | Label                                      | Current Value | Requested Value      | Fieldtype      | DateTimeFormat       |
        | What time or date did your symptoms start? |               | (Current date time)  | datetimepicker | dd/MMMM/yyyy HH:mm   |
        | What time did your symptoms start?         |               | (Current time)       | timepicker     |     HH:mm            |