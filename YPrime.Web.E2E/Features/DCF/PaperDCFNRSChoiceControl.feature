Feature: Updated Add Paper DCF 
    
Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 36" has been created with Software Version "0.0.0.1" and Configuration Version "36.0-6.10"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-004" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
  

@ID:44 @ID:5791
#1
Scenario: Verify that when using NRS choices questions in paper dcf the field will be a dropdown and all the choices configured is displayed 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "NRS Choices Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                                      | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                                          | Please provide a response | datepicker |
        | Visit Name:                                                                | Please provide a response | dropdown   |
        | Please select the percentage that shows how much relief you have received. | Please provide a response | dropdown   |
    And dropdown for "Please select the percentage that shows how much relief you have received." question has placeholder "Please provide a response"
    When I click on "dropdown" in data correction field for "Please select the percentage that shows how much relief you have received." question
    Then the following choices are displayed in dropdown for "Please select the percentage that shows how much relief you have received." question
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
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "NRS Choices Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                                      | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                                          | Please provide a response | datepicker |
        | Visit Name:                                                                | Please provide a response | dropdown   |
        | Please select the percentage that shows how much relief you have received. | Please provide a response | dropdown   |
    And I click on "dropdown" in data correction field for "Please select the percentage that shows how much relief you have received." question
    And the following choices are displayed in dropdown for "Please select the percentage that shows how much relief you have received." question
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
     And I select "70%" from "Please select the percentage that shows how much relief you have received." "dropdown"
     And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
     And I enter "Add Paper DCF" in "Reason For Correction" inputtextbox field
     And "Next" button is enabled
     And I click on "Next" button
     And I am on "Submit Data Correction" page
     And the following data is displayed in the data field table
            | Label                                                                      | Value                     | Fieldtype |
            | Questionnaire:                                                             | NRS Choices Questionnaire | text      |
            | Date of Questionnaire Completion                                           | (Current Date)            | text      |
            | Visit Name                                                                 |                           | text      |
            | Please select the percentage that shows how much relief you have received. | 70%                       | dropdown  |
     When I click on "Back" button
     Then I am back on "Create Data Correction" page
     And "Initial Site" is displayed for "Site Name" dropdown
     And "S-10001-004" is displayed for "Subject" dropdown
     And "Add Paper Questionnaire" is displayed for "Type Of Correction" dropdown
     And "NRS Choices Questionnaire" is displayed for "Select a Questionnaire" dropdown
     And "Add Paper DCF" is displayed in "Reason For Correction" inputtextbox field
     And the following data is displayed in the data correction field
            | Label                                                                      | Value          | Fieldtype  |
            | Date of Questionnaire Completion:                                          | (Current Date) | datepicker |
            | Visit Name:                                                                | Please provide a response  | dropdown   |
            | Please select the percentage that shows how much relief you have received. | 70%            | dropdown   |

@ID:3841 @ID:5791
#3
Scenario: Verify that when using NRS control question for paper DCF the field is a dropdown field and min max value configure for NRS is displayed with default Please Select text.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                  | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                      | Please provide a response | datepicker |
        | Visit Name:                                            | Please provide a response | dropdown   |
        | Please indicate the pain level from the scale of 0-10. | Please provide a response | dropdown   |
    And dropdown for "Please indicate the pain level from the scale of 0-10." question has placeholder "Please provide a response"
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." question
    And the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." question
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

@ID:3841 @ID:5791
#4
Scenario: Verify that the user is able to make only one selection 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                  | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                      | Please provide a response | datepicker |
        | Visit Name:                                            | Please provide a response | dropdown   |
        | Please indicate the pain level from the scale of 0-10. | Please provide a response | dropdown   |
    And dropdown for "Please indicate the pain level from the scale of 0-10." question has placeholder "Please provide a response"
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." question
    And the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." question
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
    And I select "7" from "Please indicate the pain level from the scale of 0-10." "dropdown"
    And dropdown for "Please indicate the pain level from the scale of 0-10." question has selected value "7"
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." question
    When I select "2" from "Please indicate the pain level from the scale of 0-10." "dropdown"
    Then dropdown for "Please indicate the pain level from the scale of 0-10." question has selected value "2"
    And dropdown for "Please indicate the pain level from the scale of 0-10." question does not have selected value "7"

@ID:3841 @ID:5791
#5
Scenario: Data is persisted when the user clicks on back and next button on the data correction page for NRS control.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "NRS Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                  | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                      | Please provide a response | datepicker |
        | Visit Name:                                            | Please provide a response | dropdown   |
        | Please indicate the pain level from the scale of 0-10. | Please provide a response | dropdown   |
    And I click on "dropdown" in data correction field for "Please indicate the pain level from the scale of 0-10." question
    And the following choices are displayed in dropdown for "Please indicate the pain level from the scale of 0-10." question
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
     And I select "7" from "Please indicate the pain level from the scale of 0-10." "dropdown"
     And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
     And I enter "Add Paper DCF" in "Reason For Correction" inputtextbox field
     And "Next" button is enabled
     And I click on "Next" button
     And I am on "Submit Data Correction" page
     And the following data is displayed in the data field table
            | Label                                                  | Value             | Fieldtype |
            | Questionnaire:                                         | NRS Questionnaire | text      |
            | Date of Questionnaire Completion                       | (Current Date)    | text      |
            | Visit Name                                             |                   | text      |
            | Please indicate the pain level from the scale of 0-10. | 7                 | dropdown  |
     When I click on "Back" button
     Then I am back on "Create Data Correction" page
     And "Initial Site" is displayed for "Site Name" dropdown
     And "S-10001-004" is displayed for "Subject" dropdown
     And "Add Paper Questionnaire" is displayed for "Type Of Correction" dropdown
     And "NRS Questionnaire" is displayed for "Select a Questionnaire" dropdown
     And "Add Paper DCF" is displayed in "Reason For Correction" inputtextbox field
     And the following data is displayed in the data correction field
            | Label                                                  | Value          | Fieldtype  |
            | Date of Questionnaire Completion:                      | (Current Date) | datepicker |
            | Visit Name:                                            | Please provide a response  | dropdown   |
            | Please indicate the pain level from the scale of 0-10. | 7              | dropdown   |






