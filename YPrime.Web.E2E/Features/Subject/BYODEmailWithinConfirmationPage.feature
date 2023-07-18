@ignore @manual
Feature: BYOD Email within Confirmation Page
    PBI 99397


    Background:
        Given I have a study configured using "yprime_e2e_subject_6digit_pin_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites" dropdown
        And I select "Site 4" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page

    #1
    Scenario: Verify that email modal is displayed with the following data in English when the user clicks on the email icon on confirmation page.
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | en-US       | Dropdown     |
        And I click on "Yes, personal device"
        And I click on "Create button"
        And "Confirmation" popup is displayed
        When I click on "Email icon"
        Then following data is displayed
            | Email Field | Value                                                            |
            | To          |                                                                  |
            | Subject     | Yprime S-10001-<SubjectID> BYOD Enrollment Information           |
            | Body        | To access your BYOD enrollment information select the attachment |
        And pdf "Yprime S-10001-<SubjectID> BYOD Enrollment Information" icon will be displayed


    #2
    Scenario: Verify that email modal is displayed with the following data in spanish when the user clicks on the email icon on confirmation page.
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | es-US       | Dropdown     |
        And I click on "Yes, personal device"
        And I click on "Create button"
        And "Confirmation" popup is displayed
        When I click on "Email icon"
        Then following data is displayed
            | Email Field | Value                                      |
            | To          |                                            |
            | Subject     | Yprime S-10001-006 Subject Text in Spanish |
            | Body        | Email Body in Spanish.                     |
        And pdf "Yprime S-10001-<SubjectID> Subject Text in Spanish" icon will be displayed




