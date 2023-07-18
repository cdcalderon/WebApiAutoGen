@Portal

Feature: AddPaperDCF FreeText Control

Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 17" has been created with Software Version "0.0.0.1" and Configuration Version "16.0-1.0"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-005" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"

@ID:3843 @ID:5791
#7
Scenario: Verify that user is not able to enter value outside the max configured value for free text control. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "Free Text Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                   | Value                     | Fieldtype    |
        | Date of Questionnaire Completion:       | Please provide a response | datepicker   |
        | Visit Name:                             | Please provide a response | dropdown     |
        | Please make sure to enter your symptoms | Please provide a response | inputtextbox |
    When I enter "My Symptoms are My Symptoms are" for "Please make sure to enter your symptoms" question in PaperDCF
    Then "My Symptoms are" is displayed for "Please make sure to enter your symptoms" question in PaperDCF

@ID:3843 @ID:5791
#8
Scenario: Data is persisted when the user clicks back and next on the data correction page for free text control.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "Free Text Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                   | Value                     | Fieldtype    |
        | Date of Questionnaire Completion:       | Please provide a response | datepicker   |
        | Visit Name:                             | Please provide a response | dropdown     |
        | Please make sure to enter your symptoms | Please provide a response | inputtextbox |
    And I enter "My Symptoms are" for "Please make sure to enter your symptoms" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "Add Paper DCF" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data field table
             | Label                                   | Value                   | Fieldtype |
             | Questionnaire:                          | Free Text Questionnaire | text      |
             | Date of Questionnaire Completion        | (Current Date)          | text      |
             | Visit Name                              |                         | text      |
             | Please make sure to enter your symptoms | My Symptoms are         | text      |
     When I click on "Back" button
     Then I am back on "Create Data Correction" page
     And the given "Initial Site" is displayed for "Site Name" dropdown
     And the given "S-10001-005" is displayed for "Subject" dropdown
     And the given "Add Paper Questionnaire" is displayed for "Type Of Correction" dropdown
     And "Add Paper DCF" is displayed in "Reason For Correction" inputtextbox field
     And the given "Free Text Questionnaire" is displayed for "Select a Questionnaire" dropdown
     And the following data is displayed in the data correction field
            | Label                                   | Value                     | Fieldtype    |
            | Date of Questionnaire Completion:       | (Current Date)            | datepicker   |
            | Visit Name:                             | Please provide a response | dropdown     |
            | Please make sure to enter your symptoms | My Symptoms are           | inputtextbox |
