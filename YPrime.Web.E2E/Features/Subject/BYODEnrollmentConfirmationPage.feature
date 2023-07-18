@ignore @manual
Feature: BYOD Enrollment Confirmation Page
    PBI 88769

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
    Scenario: Verify that confirmation page is displayed if the user selects YES when creating a subject and the default pin is 6 digit
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | en-US       | Dropdown     |
        And I click on  "Yes, personal device"
        And I click on "Create" button
        And "Confirmation" popup is displayed
        And "Welcome to" text is displayed
        And "Client" label has value "YPrime"
        And "Protocol" label has value "YPrime BYOD"
        And "Study Information" text is displayed
        And following data is displayed
            | Label              | Value                                                                                                                |
            | Sponsor:           | YPrime                                                                                                               |
            | Protocol:          | YPrime BYOD                                                                                                          |
            | Study Name:        | Yprime BYOD                                                                                                          |
            | Study Description: | A bring your own device pilot study to gauge the efficacy of subjects using their own devices in a clinical setting. |
        And "Your Information" text is displayed
        And "Enrollment ID" label is not empty
        And following data is displayed
            | Label      | Value               |
            | Patient ID | S-10002-<SubjectID> |
            | Site ID:   | 10002               |
        And "Default PIN Code :123456" text is displayed
        And "You have elected to use your own mobile device while participating in this Study. Below you will find information regarding this study as well as instructions on how to get started." text is displayed
        And "email icon" is displayed
        And "print icon" is displayed
        And "Getting Started" text is displayed
        And "1" is displayed for "Download Yprime eCOA from your app/play store or scan a QR code below" text
        And "2" is displayed for "Enter Enrollment ID shown above and select Enroll" text
        And "3" is displayed for "Review details and select Confirm Enrollment" text
        And "QR Code" is displayed for Google Play
        And "QR Code" is displayed for App Store
        And "If any of the information displayed on your device does not match the above information, please reach out to your medical professional for assistance" text is displayed
        When I click on "X icon"
        Then popup is dismissed


    #2
    Scenario: Verify that confirmation page is displayed in Spanish if the user selects YES when creating a subject and the default pin is 6 digit
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | es-US       | Dropdown     |
        And I click on  "Yes, personal device"
        And I click on "Create" button
        And "Confirmation" popup is displayed
        And "Welcome to" text is displayed in spanish
        And "Client" label has value "YPrime"
        And "Protocol" label has value "YPrime BYOD"
        And "Study Information" text is displayed in spanish
        And following data is displayed
            | Label              | Value                                                                                                                |
            | Sponsor:           | YPrime                                                                                                               |
            | Protocol:          | YPrime BYOD                                                                                                          |
            | Study Name:        | Yprime BYOD                                                                                                          |
            | Study Description: | A bring your own device pilot study to gauge the efficacy of subjects using their own devices in a clinical setting. |
        And "Your Information" text is displayed in spanish
        And "Enrollment ID" label is not empty
        And following data is displayed in spanish
            | Label      | Value               |
            | Patient ID | S-10002-<SubjectID> |
            | Site ID:   | 10002               |
        And "Default PIN Code :123456" text is displayed in spanish
        And "You have elected to use your own mobile device while participating in this Study. Below you will find information regarding this study as well as instructions on how to get started." text is displayed in spanish
        And "email icon" is displayed
        And "print icon" is displayed
        And "Getting Started" text is displayed in spanish
        And "1" is displayed in spanish for "Download Yprime eCOA from your app/play store or scan a QR code below" spanish text
        And "2" is displayed in spanish for "Enter Enrollment ID shown above and select Enroll" spanish text
        And "3" is displayed in spanish for "Review details and select Confirm Enrollment" spanish text
        And "QR Code" is displayed for Google Play
        And "QR Code" is displayed for App Store
        And "If any of the information displayed on your device does not match the above information, please reach out to your medical professional for assistance" text is displayed in spanish
        When I click on "X icon"
        Then popup is dismissed

    #3
    Scenario: Verify that Print perference popup is displayed when user clicks on Print icon
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | en-US       | Dropdown     |
        And I click on  "Yes, personal device"
        And I click on "Create" button
        And "Confirmation" popup is displayed
        When I click on "Print icon"
        Then "Print Preference" popup is displayed

    #4
    Scenario: Verify that when user clicks on "Cancel" the popup is not displayed and user is on Confirmation page
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | en-US       | Dropdown     |
        And I click on  "Yes, personal device"
        And I click on "Create" button
        And "Confirmation" popup is displayed
        And I click on "Print icon"
        And "Print Preference" popup is displayed
        When I click on "Cancel button"
        Then popup is dismissed
        And "Confirmation" popup is displayed

    #5
    Scenario: Verify that when user clicks "Send button" the print pop up is not displayed and user is on confirmation page
        Given I retreive unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label     | Value       | FieldType    |
            | Subject   | <SubjectID> | Numberinput  |
            | Gender    | Female      | Checkbox     |
            | Weight    | 120         | Numberinput  |
            | Free Text | Pharmacy 1  | Inputtextbox |
            | Language  | en-US       | Dropdown     |
        And I click on  "Yes, personal device"
        And I click on "Create" button
        And "Confirmation" popup is displayed
        And I click on "Print Icon"
        And "Print Preference" popup is displayed
        When I click on "Send button"
        Then popup is dismissed
        And "Confirmation" popup is displayed



