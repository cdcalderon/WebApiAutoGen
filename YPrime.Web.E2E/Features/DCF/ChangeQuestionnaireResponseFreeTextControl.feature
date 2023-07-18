@Portal


Feature: Change Questionnaire Response FreeText Control

Background: 
Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide  
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Patient "patient 1" with patient number "S-10001-013" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-013" has completed "Free Text Questionnaire" questionnaire for question "Please make sure to enter your symptoms" and value "Headache"

@MockStudyBuilder


@ID:4944 @ID:5791
#1
Scenario: Verify that the user is not able to enter over the max character configured.
    Given I am on "Create Data Correction" page
    And max value is set with value "15" for "Please make sure to enter your symptoms" question for questionnaire "Free Text Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-013" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Free Text Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                   | Current Value | Requested Value           | Fieldtype    |
        | Please make sure to enter your symptoms | Headache      | Please provide a response | inputtextbox |
    And I enter "My Symptoms are" for "Please make sure to enter your symptoms" question
    And "My Symptoms are" is displayed for "Please make sure to enter your symptoms" question

@ID:4944 @ID:5791
@MockStudyBuilder
#2
Scenario: Data persisted when the user clicks back and next button on data correction page. 
    Given I am on "Create Data Correction" page
    And max value is set with value "15" for "Please make sure to enter your symptoms" question for questionnaire "Free Text Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-013" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Free Text Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                   | Current Value | Requested Value           | Fieldtype    |
        | Please make sure to enter your symptoms | Headache      | Please provide a response | inputtextbox |
    And I enter "My Symptoms are" for "Please make sure to enter your symptoms" question
    And "Next" button is disabled
    And I enter "Free Text" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the existing question correction fields
        | Label                                   | Current Value | Requested Value | Fieldtype |
        | Please make sure to enter your symptoms | Headache      | My Symptoms are | text      |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
        | Label                                   | Current Value | Requested Value | Fieldtype    |
        | Please make sure to enter your symptoms | Headache      | My Symptoms are | inputtextbox |
    And "Free Text" is displayed in "Reason For Correction" inputtextbox field
