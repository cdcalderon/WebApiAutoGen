Feature: Change Questionnaire Response DCF NRS Choice Control

Background:
    Given I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-004" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Software Release "Config release 16" has been created with Software Version "0.0.0.1" and Configuration Version "18.0-1.0"
    And Subject "S-10001-004" has completed "NRS Choices Questionnaire" questionnaire for question "Please select the percentage that shows how much relief you have received." and choice "70%"


@ID:44 @ID:423 @ID:5791 
#1
Scenario: Verify that when Change Questionnaire Response DCF type is selected, the NRS choices control field will be a dropdown and all the choices configured is displayed 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "NRS Choices Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
            | Label                                                                      | Current Value | Requested Value           | Fieldtype |
            | Please select the percentage that shows how much relief you have received. | 70%           | Please provide a response | dropdown  |
    And dropdown for "Please select the percentage that shows how much relief you have received." existing question has placeholder "Please provide a response"
    When I click on "dropdown" in data correction field for "Please select the percentage that shows how much relief you have received." existing question
    Then the following choices are displayed in dropdown for "Please select the percentage that shows how much relief you have received." existing question
            | Choices |
            | 0%      |
            | 10%     |
            | 20%     |
            | 30%     |
            | 40%     |
            | 50%     |
            | 60%     |
            | 70%     |
            | 80%     |
            | 90%     |
            | 100%    |

@ID:44 @ID:5791
#2
Scenario: Data is persisted when the user clicks Next and Back button on Data Correction page.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "NRS Choices Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
            | Label                                                                      | Current Value | Requested Value           | Fieldtype |
            | Please select the percentage that shows how much relief you have received. | 70%           | Please provide a response | dropdown  |
    And I click on "dropdown" in data correction field for "Please select the percentage that shows how much relief you have received." existing question
    And the following choices are displayed in dropdown for "Please select the percentage that shows how much relief you have received." existing question
            | Choices |
            | 0%      |
            | 10%     |
            | 20%     |
            | 30%     |
            | 40%     |
            | 50%     |
            | 60%     |
            | 70%     |
            | 80%     |
            | 90%     |
            | 100%    |
    And "Next" button is disabled
    And I select "90% " as new value from "Please select the percentage that shows how much relief you have received." "dropdown"
    And I enter "Change questionnaire responses" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    Then I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
            | Label                                                                      | Current Value | Requested Value | Fieldtype |
            | Please select the percentage that shows how much relief you have received. | 70%           |       90%       | text      |
    When I click on "Back" button
    Then I am back on "Create Data Correction" page
    And "Initial Site" is displayed for "Site Name" dropdown
    And "S-10001-004" is displayed for "Subject" dropdown
    And "Change questionnaire responses" is displayed for "Type Of Correction" dropdown
    And the given "NRS Choices Questionnaire" is displayed for "Select a Questionnaire" dropdown
    And "Change questionnaire responses" is displayed in "Reason For Correction" inputtextbox field
    And the following data is displayed in the existing question correction fields
            | Label                                                                      | Current Value | Requested Value | Fieldtype |
            | Please select the percentage that shows how much relief you have received. | 70%           |       90%       | dropdown  |