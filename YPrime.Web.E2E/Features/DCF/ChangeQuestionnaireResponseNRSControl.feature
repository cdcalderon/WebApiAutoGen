@Portal

Feature: Change Questionnaire Response NRS Control
	
Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 36" has been created with Software Version "0.0.0.1" and Configuration Version "36.0-6.10" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-008" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-008" has completed "NRS Questionnaire" questionnaire for question "Please indicate the pain level from the scale of 0-10." and value "4"
 


@ID:4943
#NRS - MIn 0 Max 10
#1
Scenario: Verify that NRS control is a dropdown with the configured min-max value in Change Questionnaire Responses DCF and also verify that "Please Select" place holder text is displayed. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-008" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                  | Current Value | Requested Value                | Fieldtype     |
        | Please indicate the pain level from the scale of 0-10. | 4             |  Please provide a response     | dropdown      |
    And dropdown for "Please indicate the pain level from the scale of 0-10." existing question has placeholder "Please provide a response"
    When I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." existing question
    Then the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." existing question
         | Choices |
         | 0       |
         | 1       |
         | 2       |
         | 3       |
         | 4       |
         | 5       |
         | 6       |
         | 7       |
         | 8       |
         | 9       |
         | 10      |

@ID:4943
#2
Scenario: Verify that the user can only select one value per question.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-008" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                  | Current Value | Requested Value             | Fieldtype     |
        | Please indicate the pain level from the scale of 0-10. | 4             | Please provide a response   | dropdown |
    And dropdown for "Please indicate the pain level from the scale of 0-10." existing question has placeholder "Please provide a response"
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." existing question
    And the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." existing question
         | Choices |
         | 0       |
         | 1       |
         | 2       |
         | 3       |
         | 4       |
         | 5       |
         | 6       |
         | 7       |
         | 8       |
         | 9       |
         | 10      |
    And I select "7" from existing "Please indicate the pain level from the scale of 0-10." "dropdown"
    And "7" is displayed for "Please indicate the pain level from the scale of 0-10." "dropdown"
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." existing question
    And I select "6" from existing "Please indicate the pain level from the scale of 0-10." "dropdown"
    And "6" is displayed for "Please indicate the pain level from the scale of 0-10." "dropdown"
    And "7" is not displayed for "Please indicate the pain level from the scale of 0-10." "dropdown"


@ID:4943
#3
Scenario: Data is persisted when user click next and back button on data correction page.   
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-008" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
       | Label                                                  | Current Value | Requested Value             | Fieldtype     |
       | Please indicate the pain level from the scale of 0-10. | 4             | Please provide a response   | dropdown |
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." existing question
    And the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." existing question
         | Choices |
         | 0       |
         | 1       |
         | 2       |
         | 3       |
         | 4       |
         | 5       |
         | 6       |
         | 7       |
         | 8       |
         | 9       |
         | 10      |
    And I select "7" from existing "Please indicate the pain level from the scale of 0-10." "dropdown"
    And "7" is displayed for "Please indicate the pain level from the scale of 0-10." "dropdown"
    And I enter "NRS" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                                  | Current Value | Requested Value | Fieldtype |
        | Please indicate the pain level from the scale of 0-10. | 4             | 7               | text      |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
        | Label                                                  | Current Value | Requested Value | Fieldtype     |
        | Please indicate the pain level from the scale of 0-10. | 4             | 7               | dropdown      |
    And "NRS" is displayed in "Reason For Correction" inputtextbox field