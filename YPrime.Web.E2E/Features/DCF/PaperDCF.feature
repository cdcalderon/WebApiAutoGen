
Feature: Paper DCF
    PBI 87861

    Background:
        Given I have logged in with user "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Manage DCF's" button
        And I am on "Data Correction" page
        And I click on "Add New DCF" button
        And Subject "S-10001-003" exists in the "Patients" table
        And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
        And I am on "Create Data Correction" page

    #1
    Scenario: When adding a new paper dcf, if a subject is assigned to a device, then the questionnaire displayed will be the latest config version for the subject's device.
        Given Software Release "Initial Release" has been created with Software Version "1.0.0" and Configuration Version "1.0-1.0"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
        And Subject "S-10001-003" is assigned to "YP-E2E-Device" Device
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                             | Value         | Fieldtype  |
            | Date of Questionnaire Completion: |               | datepicker |
            | Visit Name:                       | Please Select | dropdown   |
            | How would you say your health is: | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "How would you say your health is:" question
        And the following choices are displayed in dropdown for "How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |

    #2
    Scenario: When adding a new paper dcf, if a subject is not assigned to a device, then the questionnaire displayed will be the latest config version at the site level.
        Given Software Release "Release 1" has been created with Software Version "1.0.1" and Configuration Version "2.0-1.0"
        And Site "Initial Site" is assigned to Software Release "Release 1"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                         | Value         | Fieldtype  |
            | Date of Questionnaire Completion:             |               | datepicker |
            | Visit Name:                                   | Please Select | dropdown   |
            | In general, How would you say your health is: | Please Select | dropdown   |
            | How would you rate your health in general?    | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question
        And the following choices are displayed in dropdown for "In general, How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
            | Not better      |
            | Worse           |
            | Much Worse      |

    #3
    Scenario: When adding a new paper dcf, if a subject is not assigned to a device and, if there is no config version assigned at site level, then the questionnaire displayed will be the latest config version at the country level.
        Given Software Release "Release 2" has been created with Software Version "1.0.2" and Configuration Version "3.0-1.0"
        And Country "United States" is assigned to Software Release "Release 2"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                         | Value         | Fieldtype  |
            | Date of Questionnaire Completion:             |               | datepicker |
            | Visit Name:                                   | Please Select | dropdown   |
            | In general, How would you say your health is: | Please Select | dropdown   |
            | How would you rate your health in general?    | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question
        And the following choices are displayed in dropdown for "In general, How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
            | Not better      |
            | Worse           |
            | Much Worse      |

    #4
    Scenario: When adding a new paper dcf, if a subject is not assigned to a device and if there is no config version assigned at country level then questionnaire displayed will be the latest config version at study level.
        Given Software Release "Release 3" has been created with Software Version "1.0.3" and Configuration Version "4.0-1.0"
        And "Study Wide" is enabled for Software Release "Release 3"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                         | Value         | Fieldtype  |
            | Date of Questionnaire Completion:             |               | datepicker |
            | Visit Name:                                   | Please Select | dropdown   |
            | In general, How would you say your health is: | Please Select | dropdown   |
            | How would you rate your health in general?    | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question
        And the following choices are displayed in dropdown for "In general, How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
            | Not better      |
            | Worse           |
            | Much Worse      |

    #5
    Scenario: If subject is not assigned to a device and config version is assigned at the site level but the config version at the country level is higher.
        Given Software Release "Release 2" has been created with Software Version "1.0.3" and Configuration Version "3.0-1.0"
        And Country "United States" is assigned to Software Release "Release 2"
        And Software Release "Release 1" has been created with Software Version "1.0.2" and Configuration Version "2.0-1.0"
        And Site "Initial Site" is assigned to Software Release "Release 1"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                         | Value         | Fieldtype  |
            | Date of Questionnaire Completion:             |               | datepicker |
            | Visit Name:                                   | Please Select | dropdown   |
            | In general, How would you say your health is: | Please Select | dropdown   |
            | How would you rate your health in general?    | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question
        And the following choices are displayed in dropdown for "In general, How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
            | Not better      |
            | Worse           |
            | Much Worse      |

    #6
    Scenario: If a subject is not assigned to a device and config version is assigned at the site level and the config version at the country level but the config version at study level is higher.
        Given Software Release "Release 2" has been created with Software Version "1.0.3" and Configuration Version "3.0-1.0"
        And Country "United States" is assigned to Software Release "Release 2"
        And Software Release "Release 1" has been created with Software Version "1.0.2" and Configuration Version "2.0-1.0"
        And Site "Initial Site" is assigned to Software Release "Release 1"
        And Software Release "Release 3" has been created with Software Version "1.0.3" and Configuration Version "4.0-1.0"
        And "Study Wide" is enabled for Software Release "Release 3"
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        When I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                         | Value         | Fieldtype  |
            | Date of Questionnaire Completion:             |               | datepicker |
            | Visit Name:                                   | Please Select | dropdown   |
            | In general, How would you say your health is: | Please Select | dropdown   |
            | How would you rate your health in general?    | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question
        And the following choices are displayed in dropdown for "In general, How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
            | Not better      |
            | Worse           |
            | Much Worse      |

    #7
    Scenario: Data is persisted when the user clicks Next and Back button on Data Correction page.
        Given Software Release "Initial Release" has been created with Software Version "1.0.0" and Configuration Version "1.0-1.0"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
        And Subject "S-10001-003" is assigned to "YP-E2E-Device" Device
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And "Next" button is disabled
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        And the following data is displayed in the data correction field
            | Label                             | Value         | Fieldtype  |
            | Date of Questionnaire Completion: |               | datepicker |
            | Visit Name:                       | Please Select | dropdown   |
            | How would you say your health is: | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "How would you say your health is:" question
        And the following choices are displayed in dropdown for "How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
        And I select "Much Better" from "How would you say your health is:" "dropdown"
        And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
        And I enter "Paper DCF" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
            | Label                                         | Value                                         | Fieldtype |
            | Questionnaire:                                | Questionnaire Forms                           | text      |
            | Date of Questionnaire Completion              | (Current Date)                                | text      |
            | Visit Name                                    |                                               | text      |
            | How would you say your health is:             | Much Better                                   | text      |
        When I click on "Back" button
        Then I am back on "Create Data Correction" page
        And the given "Initial Site" is displayed for "Site Name" dropdown
        And the given "S-10001-003" is displayed for "Subject" dropdown
        And the given "Add Paper Questionnaire" is displayed for "Type Of Correction" dropdown
        And the given "Questionnaire Forms" is displayed for "Select a Questionnaire" dropdown
        And "Paper DCF" is displayed in "Reason For Correction" inputtextbox field
        And the following data is displayed in the data correction field
            | Label                             | Value          | Fieldtype  |
            | Date of Questionnaire Completion: | (Current Date) | datepicker |
            | Visit Name:                       | Please Select  | dropdown   |
            | How would you say your health is: | Much Better    | dropdown   |

@ignore
@ID:6711
    #8
    Scenario: Verify that when the user selects the following "submit," validation message is displayed, and verify that when approval is selected, DCF is submitted. When the decline is selected, DCF is not submitted. Once the new paper dcf is submitted, the config version should be displayed on the questionnaire management page.
        Given Software Release "Initial Release" has been created with Software Version "1.0.0" and Configuration Version "1.0-1.0"
        And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
        And Subject "S-10001-003" is assigned to "YP-E2E-Device" Device
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-003" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
        And "Next" button is disabled
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
        And the following data is displayed in the data correction field
            | Label                             | Value         | Fieldtype  |
            | Date of Questionnaire Completion: |               | datepicker |
            | Visit Name:                       | Please Select | dropdown   |
            | How would you say your health is: | Please Select | dropdown   |
        And I click on "dropdown" in data correction field for "How would you say your health is:" question
        And the following choices are displayed in dropdown for "How would you say your health is:" question
            | Choices         |
            | Much Better     |
            | About the Same  |
            | Somewhat better |
        And I select "Much Better" from "How would you say your health is:" "dropdown"
        And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
        And I enter "Paper DCF" in "Reason For Correction" inputtextbox field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data field table
            | Label                                         | Value                                         | Fieldtype |
            | Questionnaire:                                | Questionnaire Forms                           | text      |
            | Date of Questionnaire Completion              | (Current Date)                                | text      |
            | Visit Name                                    |                                               | text      |
            | How would you say your health is:             | Much Better                                   | text      |
        And I click on "Approve" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Approving   |
        And I click "Decline" button in the popup
        And I am on "Submit Data Correction" page
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Approving   |
        And I click on "Approve" button in the popup
        And I am on "Data Correction Confirmation" page
        And "Success" popup is displayed with message "Correction has been added successfully."
        And I click "Ok" button in the popup
        And I click on "Subject" link on the top navigation bar
        And I select Subject "S-10001-003"
        And I am on Subject "S-10001-003" page
        And I click on subject "Questionnaires" tab
        When I click on "Questionnaire Forms" diary entry		
        Then I am on "Diary Entry Details" page
        And "1.0-1.0" is displayed in "Version" details field
        And the following data is displayed in the data field table
            | Label                             | Value       | Fieldtype |
            | How would you say your health is: | Much Better | text      |