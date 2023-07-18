@portal
Feature: Change Subject Visit Validation Message

Background:
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
    And I have logged in with user "PortalE2EUser"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And Patient "patient 1" with patient number "S-10001-023" is associated with "Initial Site"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button

@ID:5802 @MockStudyBuilder
#1
Scenario: Verify that validation message is displayed if visit activation date is greater then the visit date. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I select "(Current date) -1" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And I enter "Change Subject Visit" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message "Visit Activation Date cannot be greater than Visit Date, please check the information you have entered is valid" is displayed

@ID:5802 @MockStudyBuilder
#2
Scenario: Verify that validation message is not displayed if visit activation date is equal or less than the visit date. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I enter "Change Subject Visit" in "Reason For Correction" inputtextbox field
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And "Next" button is enabled
    And I click on "Next" button
    Then the validation message is not displayed
    And I click on "Back" button
    And I select "(Current date) -1" from existing "Visit Activation Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed

