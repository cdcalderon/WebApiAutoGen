@ignore
@portal


Feature: Change Subject Visit DCF Visit Option Order

Background: Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-024" exists in the "Patients" table
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Patient "patient 1" with patient number "S-10001-023" is associated with "Site 1"
    And "Tablet" Device "YP-E2E-Device" is assigned to Site "Site 1"
    And Subject "S-10001-024" is assigned to "YP-E2E-Device" Device
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And Diary Entry "NRS Questionnaire" is associated with "patient 1"
    And "Screening Visit" for "patient 1" is in "Completed" status
    And Subject "S-10001-024" has completed "NRS Questionnaire" questionnaire for question "Please indicate the severity of pain over the past 24 hours on a scale of 0 and 10." and value "4"


@ID:5748
#1
Scenario: Verify that the visit options are in the following order Visit Status, Visit Date and Visit Activation Date on the change subject visit dcf page. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label                 | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status          | Completed     | Please provide a response |              | dropdown   |
        | Visit Date            | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Activation Date | Current Date  | Please provide a response |              | datepicker |


 
