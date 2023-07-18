@ignore @manual
Feature: Change Subject Information DCF
    PBI 87861

    Background:
        Given I have logged in with user "PortalE2EUser"
        And "working" Configuration Version has the following "Subject Information" attributes:
            | Label         | Field Type   |
            | Subject       | Numberinput  |
            | Gender        | Radio Button |
            | Date of Birth | datepicker   |
            | Weight        | Numberinput  |
            | Height        | Numberinput  |
        And I am on "At a Glance" page
        And I click on "Subject" link on the top naviagtion bar
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I retrieve unused subject ID
        # Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I enter the following data
            | Label         | Value          | FieldType    |
            | Subject       | <SubjectID>    | Numberinput  |
            | Gender        | Male           | Radio Button |
            | Date of Birth | (Current Date) | datepicker   |
            | Weight        | 120            | Numberinput  |
            | Height        | 200            | Numberinput  |
        And I click on "Yes, subject will use their personal device" button
        And I click on "Create" button
        And I am on "At a Glance" page
        And I click on "Manage DCF's" button
        And I am on "Data Correction" page
        And I click on "Add New DCF" button
        And I am on "Create Data Correction" page

    #1
    Scenario: When creating change subject information dcf if a subject is assigned to a device, the attribute displayed will be the latest config version for the subject's device.
        Given Software Release "Release 4" has been created with Software Version "1.0.6" and Configuration Version "6.0-01.00"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Release 4"
        And Subject "<SubjectID>" is assigned to "YP-E2E-Device" Device
        And "Male" gender is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>   |                 | datepicker   |
            | Current Status | Screened      |                 | dropdown     |
            | Gender         | Male          |                 | Radio Button |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #2
    Scenario: When creating change subject information dcf, if a subject is not assigned to a device, then the attribute displayed will be the latest config version at the site level.
        Given Software Release "5" has been created with Software Version "1.0.7" and Configuration Version "7.0-01.00"
        And Site "Initial Site" is assigned to Software Release "5"
        And "Male" gender is entered for "<SubjectID>"
        And "(Current Date)" date of birth is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        When I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value  | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>    |                 | datepicker   |
            | Current Status | Screened       |                 | dropdown     |
            | Gender         | Male           |                 | Radio Button |
            | Date Of Birth  | (Current Date) |                 | datepicker   |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #3
    Scenario: When creating change subject information dcf, if a subject is not assigned to a device and, if there is no config version assigned at the site level, then the attribute displayed will be the latest config version at the country level.
        Given Software Release "6" has been created with Software Version "1.0.8" and Configuration Version "8.0-01.00"
        And Country "United States" is assigned to Software Release "6"
        And "Male" gender is entered for "<SubjectID>"
        And "(Current Date)" date of birth is entered for "<SubjectID>"
        And "100" weight is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        When I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value  | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>    |                 | datepicker   |
            | Current Status | Screened       |                 | dropdown     |
            | Gender         | Male           |                 | Radio Button |
            | Date Of Birth  | (Current Date) |                 | datepicker   |
            | Weight         | 100            |                 | Numberinput  |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #4
    Scenario:When creating change subject information dcf, if a subject is not assigned to a device and if there is no config version assigned at the country level, then the questionnaire displayed will be the latest config version at the study level.
        Given Software Release "7" has been created with Software Version "1.0.9" and Configuration Version "9.0-01.00"
        And "Study Wide" is enabled for Software Release "7"
        And "Male" gender is entered for "<SubjectID>"
        And "10/10/2020" date of birth is entered for "<SubjectID>"
        And "100" Weight is entered for "<SubjectID>"
        And "200" Height is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        When I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value  | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>    |                 | datepicker   |
            | Current Status | Screened       |                 | dropdown     |
            | Gender         | Male           |                 | Radio Button |
            | Date Of Birth  | (Current Date) |                 | datepicker   |
            | Weight         | 100            |                 | Numberinput  |
            | Height         | 200            |                 | Numberinput  |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #5
    Scenario: If subject is not assigned to a device and config version is assigned at the site level but the config version at the country level is higher.
        Given Software Release "6" has been created with Software Version "1.0.8" and Configuration Version "8.0-01.00"
        And Country "United States" is assigned to Software Release "6"
        And Software Release "5" has been created with Software Version "1.0.7" and Configuration Version "7.0-01.00"
        And Site "Initial Site" is assigned to Software Release "5"
        And "Male" gender is entered for "<SubjectID>"
        And "10/10/2020" date of birth is entered for "<SubjectID>"
        And "100" Weight is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value  | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>    |                 | datepicker   |
            | Current Status | Screened       |                 | dropdown     |
            | Gender         | Male           |                 | Radio Button |
            | Date Of Birth  | (Current Date) |                 | datepicker   |
            | Weight         | 100            |                 | Numberinput  |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #6
    Scenario: If a subject is not assigned to a device and config version is assigned at the site level and the config version at the country level but the config version at study level is higher.
        Given Software Release "7" has been created with Software Version "1.0.9" and Configuration Version "9.0-01.00"
        And "Study Wide" is enabled for Software Release "7"
        And Software Release "6" has been created with Software Version "1.0.8" and Configuration Version "8.0-01.00"
        And Country "United States" is assigned to Software Release "6"
        And Software Release "5" has been created with Software Version "1.0.7" and Configuration Version "7.0-01.00"
        And Site "Initial Site" is assigned to Software Release "5"
        And "Male" gender is entered for "<SubjectID>"
        And "10/10/2020" date of birth is entered for "<SubjectID>"
        And "100" Weight is entered for "<SubjectID>"
        And "200" Height is entered for "<SubjectID>"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on " Type Of Correction dropdown"
        Then the following data is displayed in the data correction field
            | Label          | Current Value  | Requested value | Fieldtype    |
            | Subject Number | <SubjectID>    |                 | datepicker   |
            | Current Status | Screened       |                 | dropdown     |
            | Gender         | Male           |                 | Radio Button |
            | Date Of Birth  | (Current Date) |                 | datepicker   |
            | Weight         | 100            |                 | Numberinput  |
            | Height         | 200            |                 | Numberinput  |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"


    #7
    Scenario: Data is persisted when the user clicks Next and Back button on Data Correction page.
        Given Software Release "Release 4" has been created with Software Version "1.0.6" and Configuration Version "6.0-01.00"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Release 4"
        And Subject "<SubjectID>" is assigned to "YP-E2E-Device" Device
        And "Male" gender is entered for "<SubjectID>"
        And "Next" button is disabled
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value | Requested value | Fieldtype  |
            | Subject Number | <SubjectID>   |                 | datepicker |
            | Current Status | Screened      |                 | dropdown   |
            | Gender         | Male          |                 | Choices    |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"
        And I select "Female" for "Gender"
        And I enter "Change Subject Information" text in "Reasons for Correction"
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data correction field
            | Label          | Current Value | Requested value | Fieldtype |
            | Subject Number | <SubjectID>   |                 | text      |
            | Current Status | Screened      |                 | text      |
            | Gender         | Male          | Female          | text      |
        And "Male" choice is selected in data correction field for "Gender"
        And "Female" choice is selected in data correction field for "Gender"
        When I click on "Back" button
        Then I am on "Create Data Correction" page
        And  the following data is displayed in the data correction field
            | Label               | Value                      | Current Value | Requested Value | Fieldtype    |
            | Site Name           | Initial Site               |               |                 | dropdown     |
            | Subject             | <SubjectID>                |               |                 | dropdown     |
            | Type Of Correction: | Change Subject Information |               |                 | dropdown     |
            | Gender              |                            | Male          |                 | Radio Button |
        And "Female" choice is selected in data correction field for "Gender"
        And "Change Subject Information" text is displayed in "Reasons for Correction inputtextbox" field


@ignore
@ID:6711
#8
Scenario: Verify that when the user selects the following "Reject," validation message is displayed, and verify that when approval is selected, DCF is submitted. When the decline is selected, DCF is not submitted.
        Given Software Release "Release 4" has been created with Software Version "1.0.6" and Configuration Version "6.0-01.00"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Release 4"
        And Subject "<SubjectID>" is assigned to "YP-E2E-Device" Device
        And "Male" gender is entered for "<SubjectID>"
        And "Next" button is disabled
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "<SubjectID>" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Subject Information" from "Type Of Correction" dropdown
        Then the following data is displayed in the data correction field
            | Label          | Current Value | Requested value | Fieldtype  |
            | Subject Number | <SubjectID>   |                 | datepicker |
            | Current Status | Screened      |                 | dropdown   |
            | Gender         | Male          |                 | Choices    |
        And "Screened" status is selected for data correction field
        And "Male" choice is displayed in data correction field for "Gender"
        And "Female" choice is displayed in data correction field for "Gender"
        And I select "Female" for "Gender"
        And I enter "Change Subject Information" text in "Reasons for Correction"
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data correction field
            | Label          | Current Value | Requested value | Fieldtype |
            | Subject Number | <SubjectID>   |                 | text      |
            | Current Status | Screened      |                 | text      |
            | Gender         | Male          | Female          | text      |
        And "Male" choice is selected in data correction field for "Gender"
        And "Female" choice is selected in data correction field for "Gender"
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Submitting  |
        And I click "Decline" button in the popup
        And I am on "Submit Data Correction" page
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Submitting  |
        And I click on "Approve" button in the popup
        And I am on "Approved Correction" page
        And a pop up is displayed with "Success" message : "Correction has been made successfully"
        And I click "Ok" button
        And I am on "Approved Correction" page
        And I click on "At a Glance" link on the top navigation bar
        And I click on "Change subject information" for Subject "S-10001-002"
        And I am on "Approve/Deny Data Correction" page
        And I click on "Reject" button
        And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Rejecting   |
        And I click "Decline" button in the popup
        And I am on "Approve/Deny Data Correction" page
        And I click on "Reject" button
        And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Rejecting   |
        When I click on "Approve" button in the popup
        Then I am on "Approved Correction" page
        And a pop up is displayed with "Success" message : "Correction has been made successfully"
      



        





