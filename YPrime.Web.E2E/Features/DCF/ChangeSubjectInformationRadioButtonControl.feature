@Portal 

Feature: Change Subject Information Radio Button Control

 Background:
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
    And I have logged in with user "PortalE2EUser"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And Patient "patient 1" with patient number "S-10001-015" is associated with "Initial Site"
    And Patient "patient 1" has the following subject attributes:
    | Label  | Value |
    | Gender | Male  |
    | Weight | 120   |
    | Height | 4.6   |
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button



@ID:4959 @MockStudyBuilder
#1
Scenario: Verify that a dropdown is displayed when user clicks on the Radio button field with "Please provide a response" placeholder text.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-015" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And "Next" button is disabled
       And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType     |
         | Subject Number | 015                     |                           | inputtextbox  |
         | [N/A]          | (Current Date)          |                           | inputtextbox  |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         |                           | datepicker    |
         | Weight         | 120                     |                           | numberspinner |
         | Height         | 4.6                     |                           | numberspinner |
     And I click on "dropdown" in data correction field for "Gender" attribute
     And the following choices are displayed in dropdown for "Gender" question
         | Choices |
         | Please provide a response  |
         | Male    |
         | Female  |
    And I select "Please provide a response" from existing "Gender" "dropdown"
    And "Please provide a response" is displayed for "Gender" "dropdown"



@ID:4959 @MockStudyBuilder
#2
Scenario: Verify that the user is able to select only one option.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-015" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And "Next" button is disabled
       And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType     |
         | Subject Number | 015                     |                           | inputtextbox  |
         | [N/A]          | (Current Date)          |                           | inputtextbox  |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         |                           | datepicker    |
         | Weight         | 120                     |                           | numberspinner |
         | Height         | 4.6                     |                           | numberspinner |
     And I click on "dropdown" in data correction field for "Gender" attribute
        And the following choices are displayed in dropdown for "Gender" question
         | Choices |
         | Please provide a response  |
         | Male    |
         | Female  |
         
    And I select "Female" from existing "Gender" "dropdown"
    And "Female" is displayed for "Gender" "dropdown"
    And I click on "dropdown" in data correction field for "Gender" attribute
        And the following choices are displayed in dropdown for "Gender" question
         | Choices |
         | Please provide a response  |
         | Male    |
         | Female  |
    And I select "Please provide a response" from existing "Gender" "dropdown"
    And "Please provide a response" is displayed for "Gender" "dropdown"


@ID:4959 @MockStudyBuilder
#3
Scenario: Data persisted when the user clicks back and next button on data correction page. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-015" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And "Next" button is disabled
       And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType     |
         | Subject Number | 015                     |                           | inputtextbox  |
         | [N/A]          | (Current Date)          |                           | inputtextbox  |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         |                           | datepicker    |
         | Weight         | 120                     |                           | numberspinner |
         | Height         | 4.6                     |                           | numberspinner |
    And I select "016" from existing "Subject Number" "textbox"
    And I enter "Subject Attribute" in "Reason For Correction" inputtextbox field
    And I click on "dropdown" in data correction field for "Gender" attribute
    And the following choices are displayed in dropdown for "Gender" question
         | Choices |
         | Please provide a response  |
         | Male    |
         | Female  |
    And I select "Female" from existing "Gender" "dropdown"
    And "Female" is displayed for "Gender" "dropdown"
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label          | Current Value           | Requested Value | Fieldtype |
        | Subject Number | 015                     | 016             | text      |
        | [N/A]          | (Current Date)          |                 | text      |
        | [N/A]          | English (United States) |                 | text      |
        | Current Status | Screened                |                 | text      |
        | Gender         | Male                    | Female          | text      |
        | Date of Birth  |                         |                 | text      |
        | Weight         | 120 Lb                  |                 | text      |
        | Height         | 4.6 Height              |                 | text      |
        And I click on "Back" button
        And I am on "Create Data Correction" page
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | Fieldtype     |
         | Subject Number | 015                     | 016                       | inputtextbox  |
         | [N/A]          | (Current Date)          |                           | inputtextbox  |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Female                    | dropdown      |
         | Date of Birth  |                         |                           | datepicker    |
         | Weight         | 120                     |                           | numberspinner |
         | Height         | 4.6                     |                           | numberspinner |
     And "Female" is displayed for "Gender" "dropdown"
