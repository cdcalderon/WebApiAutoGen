@Portal


Feature: Change Questionnaire Response Multiselect Checkbox Control

Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 26" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-012" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-012" has completed "MultiSelect Questionnaire" questionnaire for question "How much of the time have you had any of the following problems?" and value "Most of the time" adding to existing diary entry
    And Subject "S-10001-012" has completed "MultiSelect Questionnaire" questionnaire for question "Please tap on the boxes to best describe your health today?" and value "Bad" adding to existing diary entry


@MockStudyBuilder
@ID:4945
#1
Scenario: Verify that all configured value is displayed and when clear all responses is selected it clears all the other responses previously selected. 
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And clear other responses is "enabled" for "Good" choice for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-012" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                            | Current Value     | Requested Value | Fieldtype    |
        | How much of the time have you had any of the following problems? | Most of the time  |                 | multiselect  |
        | Please tap on the boxes to best describe your health today?      | Bad               |                 | multiselect  |
    And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And I select "Never" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "Never" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire" 
    And I select "Always" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "Always" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    When I select "All the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    Then "Most of the time" is not selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "Always" is not selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "All the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"

@MockStudyBuilder
@ID:4945
#2
Scenario: Verify that when clear all response are not selected it doesn't clear the other responses. 
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-012" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                            | Current Value     | Requested Value | Fieldtype    |
        | How much of the time have you had any of the following problems? | Most of the time  |                 | multiselect  |
        | Please tap on the boxes to best describe your health today?      | Bad               |                 | multiselect  |
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

@MockStudyBuilder
@ID:4945
#3
Scenario: Verify that when 2 questions are configured and clear responses are configured for both questions they clear responses independently.  
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And clear other responses is "enabled" for "Good" choice for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-012" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                            | Current Value    | Requested Value | Fieldtype    |
        | How much of the time have you had any of the following problems? | Most of the time |                 | multiselect  |
        | Please tap on the boxes to best describe your health today?      | Bad              |                 | multiselect  |
    And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And the following choices are displayed for "Please tap on the boxes to best describe your health today" question
         | Value  |
         | Good   |
         | Bad    |
         | Worse  |
         | Better |
    And I select "Most of the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    When I select "Good" from "Please tap on the boxes to best describe your health today" checkbox for questionnaire "MultiSelect Questionnaire" 
    Then "Most of the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "Good" is selected for "Please tap on the boxes to best describe your health today" question for questionnaire "MultiSelect Questionnaire"

@MockStudyBuilder
@ID:4945
#4
Scenario: Data is persisted when the user clicks back and next button on data correction page.
    Given I am on "Create Data Correction" page
    And clear other responses is "enabled" for "All the time" choice for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And clear other responses is "enabled" for "Good" choice for "Please tap on the boxes to best describe your health today?" question for questionnaire "MultiSelect Questionnaire"
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-012" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "MultiSelect Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                                            | Current Value    | Requested Value      | Fieldtype    |
        | How much of the time have you had any of the following problems? | Most of the time |                      | multiselect  |
        | Please tap on the boxes to best describe your health today?      | Bad              |                      | multiselect  |
    And the following choices are displayed for "How much of the time have you had any of the following problems?" question
         | Value            |
         | All the time     |
         | Most of the time |
         | Never            |
         | Always           |
    And the following choices are displayed for "Please tap on the boxes to best describe your health today" question
         | Value  |
         | Good   |
         | Bad    |
         | Worse  |
         | Better |
    And I select "All the time" from "How much of the time have you had any of the following problems?" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "All the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And I select "Good" from "Please tap on the boxes to best describe your health today" checkbox for questionnaire "MultiSelect Questionnaire" 
    And "All the time" is selected for "How much of the time have you had any of the following problems?" question for questionnaire "MultiSelect Questionnaire"
    And "Good" is selected for "Please tap on the boxes to best describe your health today" question for questionnaire "MultiSelect Questionnaire"
    And "Next" button is disabled
    And I enter "Change questionnaire responses" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
         | Label                                                            | Current Value     | Requested Value  | Fieldtype |
         | How much of the time have you had any of the following problems? | Most of the time  | All the time     | text      |
         | Please tap on the boxes to best describe your health today?      | Bad               | Good             | text      |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
         | Label                                                            | Current Value      | Requested Value  | Fieldtype |
         | How much of the time have you had any of the following problems? | Most of the time   | All the time     | multiselect      |
         | Please tap on the boxes to best describe your health today?      | Bad                | Good             | multiselect      |
    And "Change questionnaire responses" is displayed in "Reason For Correction" inputtextbox field