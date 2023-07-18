@ignore @manual
Feature: Change Questionnaire Response DCF
    PBI 87861

    Background:
        Given I am logged in as "PortalE2EUser"
        And I am on "At a Glance" Page
        And I click on "Manage DCF's" button
        And I am on "Data Correction" page
        And I click on "Add New DCF" button
        And Subject "S-10001-002" has completed diary entry "Radio Button Questionnaire 26 October 2020" for questionnaire "Questionnaire Forms" with "1.0.1-17.0-1.0"
        And "Good" response is entered for "Radio Button Questionnaire 26 October 2020" for "How would you rate your health in general?" question

    #1
    Scenario: When creating a change questionnaire response DCF and the subject has completed the questionnaire, then questionnaire displayed will be the questionnaire with the config version , the questionnaire is completed on and not the latest config version.
        Given Software Release "1" has been created with Software Version "1.0.1" and Configuration Version "17.0-1.0"
        And I am on "Create Data Correction" page
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-002" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Questionnaire Response" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms 26 October 2020" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          |                 | dropdown  |
        And I click on dropdown in data correction field for "How would you rate your health in general?" question
        And the following choices are displayed in dropdown for "How would you rate your health in general?" question
            | Choices |
            | Good    |
            | Bad     |
            | Fair    |
            | Worse   |


    #2
    Scenario: When creating a change questionnaire responses DCF and a new study wide config is available, and the subject has completed the questionnaire, the questionnaire displayed will be the questionnaire with the config version; the questionnaire is completed on and not the latest config version.
        Given Software Release "1" has been created with Software Version "1.0.1" and Configuration Version "10.0-1.0"
        And Software Release "3" has been created with Software Version "1.0.3" and Configuration Version "11.0-1.0"
        And "Study Wide" is enabled for Software Release "3"
        And I am on "Create Data Correction" page
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-002" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Questionnaire Response" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms 26 October 2020" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          |                 | dropdown  |
        And I click on dropdown in data correction field for "How would you rate your health in general?" question
        And the following choices are displayed in dropdown for "How would you rate your health in general?" question
            | Choices |
            | Good    |
            | Bad     |
            | Fair    |


    #3
    Scenario: Data is persisted when the user clicks Next and Back button on Data Correction page.
        Given Software Release "1" has been created with Software Version "1.0.1" and Configuration Version "10.0-1.0"
        And Software Release "3" has been created with Software Version "1.0.3" and Configuration Version "11.0-1.0"
        And "Study Wide" is enabled for Software Release "3"
        And I am on "Create Data Correction" page
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-002" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Questionnaire Response" from "Type Of Correction" dropdown
        And "Next" button is disabled
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms 26 October 2020" from "Select a Questionnaire" dropdown
        And the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          |                 | dropdown  |
        And I click on dropdown in data correction field for "How would you rate your health in general?" question
        And the following choices are displayed in dropdown for "How would you rate your health in general?" question
            | Choices |
            | Good    |
            | Bad     |
            | Fair    |
        And I select "Bad" for "How would you rate your health in general?" question
        And I enter "Add Change Questionnaire DCF" text in "Reasons for Correction inputtextbox" field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          | Bad             | text      |
        When I click on "Back" button
        Then I am on "Create Data Correction" page
        And  the following data is displayed in the data correction field
            | Label                                      | Value                         | Current Value | Requested Value | Fieldtype |
            | Site Name                                  | Initial Site                  |               |                 | dropdown  |
            | Subject                                    | S-10001-003                   |               |                 | dropdown  |
            | Type Of Correction:                        | Change Questionnaire Response |               |                 | dropdown  |
            | Select a Questionnaire                     | Questionnaire Forms           |               |                 | dropdown  |
            | How would you rate your health in general? |                               | Good          | Bad             | dropdown  |
        And "Add Change Questionnaire DCF" text is displayed in "Reasons for Correction inputtextbox" field

@ignore
@ID:6711
    #4
    Scenario:Verify that when the user selects the following "Approve," validation message is displayed, and verify that when approval is selected, DCF is submitted. When the decline is selected, DCF is not submitted. Once the new paper dcf is submitted, the config version should be displayed on the questionnaire management page.
        Given Software Release "1" has been created with Software Version "1.0.1" and Configuration Version "10.0-1.0"
        And Software Release "3" has been created with Software Version "1.0.3" and Configuration Version "11.0-1.0"
        And "Study Wide" is enabled for Software Release "3"
        And I am on "Create Data Correction" page
        And I click on "Site Name" dropdown
        And I select "Initial Site" from "Site Name" dropdown
        And I click on "Subject" dropdown
        And I select "S-10001-002" from "Subject" dropdown
        And I click on "Type Of Correction" dropdown
        And I select "Change Questionnaire Response" from "Type Of Correction" dropdown
        And I click on "Select a Questionnaire" dropdown
        And I select "Questionnaire Forms 26 October 2020" from "Select a Questionnaire" dropdown
        Then the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          |                 | dropdown  |
        And I click on dropdown in data correction field for "How would you rate your health in general?" question
        And the following choices are displayed in dropdown for "How would you rate your health in general?" question
            | Choices |
            | Good    |
            | Bad     |
            | Fair    |
        And I select "Bad" for "How would you rate your health in general?" question
        And I enter "Add Change Questionnaire DCF" text in "Reasons for Correction inputtextbox" field
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Submit Data Correction" page
        And the following data is displayed in the data correction field
            | Label                                      | Current Value | Requested Value | Fieldtype |
            | How would you rate your health in general? | Good          | Bad             | text      |
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
        And I click on "Change questionnaire responses" for Subject "S-10001-002"
        And I am on "Approve/Deny Data Correction" page
        And I click on "Approve" button
        And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
        And I click on "Submit" button
        And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Approving   |
        And I click on "Approve" button in the popup
        And I am on "Approved Correction" page
        And a pop up is displayed with "Success" message : "Correction has been made successfully"
        And I click "Ok" button
        And I click on "Subject" link on top naviagtion bar
        And I select "Subject S-10001-002"
        And I am on "Subject S-10001-002" page
        And I click on "Questionnaire tab"
        When I click on "Radio Button Questionnaire"
        Then "1.0.1-10.0-1.0" is displayed between "Asset Tag" and "Started Time" field on "Daily Dairy Questionnaire" questionnaire page
        And "Bad" value is displayed in "Response" for "How would you rate your health in general?"