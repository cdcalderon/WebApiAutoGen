@Portal

Feature: Change Subject Visit DCF Visit Date Field

Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 21" has been created with Software Version "1.0.0.0" and Configuration Version "21.0-03.24" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-023" exists in the "Patients" table
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Patient "patient 1" with patient number "S-10001-023" is associated with "Initial Site"
    And "Tablet" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Subject "S-10001-023" is assigned to "YP-E2E-Device" Device
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And Diary Entry "NRS Questionnaire" is associated with "patient 1"
    And "Screening Visit" for "patient 1" is in "Completed" status
    And Subject "S-10001-023" has completed "NRS Questionnaire" questionnaire for question "Please indicate the severity of pain over the past 24 hours on a scale of 0 and 10." and value "4"
   

@ID:44 @ID:5791
#1
Scenario: Verify that when the user clicks on the visit date field a date picker is displayed 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    When I click on "datepicker" for "Visit Date" question
    Then "datepicker" is displayed for "Visit Date" question
    
@ID:44 @ID:5791
#2
Scenario: Verify that when user clicks on Remove Value toggle it removes the current value if applicable. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    When I click on "Remove Value" toggle for "Visit Date" question
    Then Current Value for "Visit Date" has a strikethrough 

@ID:44 @ID:5791
#3
Scenario: Verify that the user can't enter the future date manually in the visit date field 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    And I click on "datepicker" in data correction field for "Visit Date" question
    And "datepicker" is displayed for "Visit Date" 
    When I enter "(Current date) +1" for "Visit Date" "datepicker"
    Then "(Current Date) +1" is not displayed for "Visit Date" "datepicker" 


@ID:44 @ID:5791
#4
Scenario: Verify that X is not displayed if no date is selected and X is displayed if date is selected in the requested value field.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    And I click on "datepicker" in data correction field for "Visit Date" question
    And "X" icon is not displayed for "Visit Date"
    And I click on "datepicker" in data correction field for "Visit Date" question
    When I select "(Current date)" for "Visit Date" question
    Then "X" icon is displayed for "Visit Date" question

@ID:44 @ID:5791
#5
Scenario: Verify that when user clicks on X in the requested value field its clears the date. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    And I click on "datepicker" in data correction field for "Visit Date" question
    And "X" is not displayed for "Visit Date" question
    And I click on "datepicker" in data correction field for "Visit Date" question
    And I select "(Current date)" for "Visit Date"
    And "X" icon is displayed for "Visit Date" 
    When I click on "X" icon for "Visit Date"
    Then "(Current Date)" is not displayed for "Visit Date" "datepicker" 
    

@ID:44 @ID:5791
#6
Scenario: Data is persisted when user clicks next and back on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change Subject Visit" from "Type Of Correction" dropdown
    And I click on "Screening Visit" "Current Date" "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the data correction field 
        | Label        | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Please provide a response | toggle       | datepicker |
        | Visit Status | Completed     |                           |              | dropdown   |
    And I click on "datepicker" in data correction field for "Visit Date" question
    And I select "(Current date)" for "Visit Date" "datepicker"
    And "(Current Date)" is displayed for "Visit Date" "datepicker" 
    And "Next" button is disabled
    And I enter "Updated Visit Date" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label        | Current Value | Requested Value | Fieldtype |
        | Visit Date   | Current Date  | (Current Date)  | text      |
        | Visit Status | Completed     |                 | text      |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
        | Label        | Current Value | Requested Value | Remove Value | Fieldtype  |
        | Visit Date   | Current Date  | Current Date    | toggle       | datepicker |
        | Visit Status | Completed     |                 |              | dropdown   |
    And "Updated Visit Date" is displayed in "Reason For Correction" inputtextbox field
   












