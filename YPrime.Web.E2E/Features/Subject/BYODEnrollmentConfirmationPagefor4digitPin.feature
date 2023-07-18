@ignore @manual
Feature: BYOD Enrollment Confirmation page for 4 digit pin
    PBI 88769

    Background:
        Given I have a study configured using "yprime_e2e_subject_4digit_pin_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites" dropdown
        And I select "Site 3" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page

    #1
    Scenario: Verify that confirmation page is displayed if the user selects YES when creating a subject and the default pin is 4 digit
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
        And I click on "Create button"
        And a "Confirmation" popup is displayed
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
            | Patient ID | S-10001-<SubjectID> |
            | Site ID:   | 10001               |
        And "Default PIN Code :123456" text is displayed
        And "You have elected to use your own mobile device while participating in this Study. Below you will find information regarding this study as well as instructions on how to get started." text is displayed
        And "email icon" is displayed
        And "print icon" is displayed
        And "Getting Started" text is displayed
        And "1" is displayed for "Download Yprime eCOA from your app/play store or scan a QR code below" text
        And "2" is displayed for "Enter Enrollment ID shown above and select Enroll" text
        And "3" is displayed for "Review details and select Confirm Enrollment" text
        And "If any of the information displayed on your device does not match the above information, please reach out to your medical professional for assistance" text is displayed
        And "QR Code" is displayed for Google Play
        And "QR Code" is displayed for App Store
        When I click on "X icon"
        Then popup is dismissed
