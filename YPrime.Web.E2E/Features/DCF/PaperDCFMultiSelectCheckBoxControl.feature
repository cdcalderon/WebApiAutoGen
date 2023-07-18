@Portal


Feature: Paper DCF Multi Select CheckBox Control

Background:  
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide  
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-005" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"

@MockStudyBuilder
@ID:3846 @ID:5791
#1
Scenario: Verify that all the configured value is displayed and when clear all responses is selected it clears all the other responses previously selected. 
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker |
        | Visit Name:                                                      | Please provide a response | dropdown   |
        | How much of the time have you had any of the following problems? |                           | multiselect   |
    And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And I select "Most of the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire"
    And "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire" 
    And I select "Always" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire"
    And "Always" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    When I select "All the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire"
    Then "Most of the time" is not selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "Always" is not selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "All the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
  

@ID:3846 @ID:5791
@MockStudyBuilder
#2
Scenario: Verify that when clear all response are not selected it doesn't clear the other responses. 
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value             | Fieldtype  |
        | Date of Questionnaire Completion:                                | Please provide a response                  | datepicker |
        | Visit Name:                                                      | Please provide a response     | dropdown   |
        | How much of the time have you had any of the following problems? |                   | multiselect   |
   And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And I select "All the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "All the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    When I select "Always" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire"
    Then "Always" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "All the time" is not selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"

@ID:3846 @ID:5791
@MockStudyBuilder
#3
Scenario: Verify that when 2 questions are configured and clear responses are configured for both questions choices they clear responses independently.
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And clear other responses is "enabled" for "Good" choice for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker |
        | Visit Name:                                                      | Please provide a response | dropdown   |
        | How much of the time have you had any of the following problems? |                           | multiselect   |
        | Please tap on the boxes to best describe your health today?      |                           | multiselect   |
      And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
     And the following choices are displayed for "Please tap on the boxes to best describe your health today?" question
         | Value  |
         | Good   |
         | Bad    |
         | Worse  |
         | Better |
    And I select "Most of the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    When I select "Good" from "Please tap on the boxes to best describe your health today?" checkbox for questionnaire "MultiSelect Questionnaire" 
    Then "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "Good" is selected for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"


@ID:3846 @ID:5791
@MockStudyBuilder
#4
Scenario: Data is persisted when user clicks Next and back button on Data Correction page 
    Given I am on "Create Data Correction" page
     And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And clear other responses is "enabled" for "Good" choice for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-005" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker |
        | Visit Name:                                                      | Please provide a response | dropdown   |
        | How much of the time have you had any of the following problems? |                           | multiselect   |
        | Please tap on the boxes to best describe your health today?      |                           | multiselect   |
    And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And the following choices are displayed for "Please tap on the boxes to best describe your health today?" question
         | Value  |
         | Good   |
         | Bad    |
         | Worse  |
         | Better |
    And I select "Most of the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And I select "Good" from "Please tap on the boxes to best describe your health today?" checkbox for questionnaire "MultiSelect Questionnaire"
    And "Good" is selected for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And  "Please provide a response" displayed in "Date of Questionnaire Completion" placeholder
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "Paper DCF" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data field table
            | Label                                                            | Value                      | Fieldtype |
            | Questionnaire:                                                   | MultiSelect Questionnaire  | text      |
            | Date of Questionnaire Completion                                 | (Current Date)             | text      |
            | Visit Name                                                       |                            | text      |
            | How much of the time have you had any of the following problems? | Most of the time           | text      |
            | Please tap on the boxes to best describe your health today?      | Good                       | text      |
    When I click on "Back" button
    Then I am back on "Create Data Correction" page
    And "Initial Site" is displayed for "Site Name" dropdown
    And "S-10001-005" is displayed for "Subject" dropdown
    And "Add Paper Questionnaire" is displayed for "Type Of Correction" dropdown
    And "MultiSelect Questionnaire" is displayed for "Select a Questionnaire" dropdown
    And "Paper DCF" is displayed in "Reason For Correction" inputtextbox field
    And the following data is displayed in the data correction field

            | Label                                                            | Value                     | Fieldtype  |
            | Date of Questionnaire Completion:                                | (Current Date)            | datepicker |
            | Visit Name:                                                      | Please provide a response | dropdown   |
            | How much of the time have you had any of the following problems? | Most of the time          | multiselect   |
            | Please tap on the boxes to best describe your health today?      | Good                      | multiselect   |
