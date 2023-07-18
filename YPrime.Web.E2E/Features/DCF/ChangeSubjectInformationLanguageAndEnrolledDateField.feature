@Portal

Feature: Change subject Information Language And Enrolled Date Field

Background: 
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 41" has been created with Software Version "1.0.0.0" and Configuration Version "43.0-6.10" and assigned to Study Wide
    And Subject "S-10001-016" exists in the "Patients" table
    And Language "English (United States)" is assigned to "Initial Site"
    And Language "Hindi (India)" is assigned to "Initial Site" 
    And Language "Spanish (Brazil)" is assigned to "Initial Site" 
    And Language "Arabic (Algeria)" is assigned to "Initial Site" 
    And Language "Chinese (Simplified, China)" is assigned to "Initial Site"
    And The following data is saved for the Subject "S-10001-016"
    | Label         | Value                  |
    | Language      | English (United States)|
    | EnrolledDate  | 17/May/2022            |
    | CurrentStatus | Screened               |
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-016" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"

@ID:47 @ID:5791
#1
Scenario: Verify that Language and Enrolled date field are displayed in right order. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    When I select "Change subject Information" from "Type Of Correction" dropdown
    Then the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |
    And "Next" button is disabled
  
@ID:47 @ID:5791
#2
Scenario: Verify that all the active languages are displayed in the dropdown.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |
   When I click on "dropdown" in data correction field for "Language" existing question
   Then the following choices are displayed in dropdown for "Language" existing question
         | Choices                    |
         | Arabic (Algeria)           |
         | Chinese (Simplified, China)|
         | English (United States)    |
         | Hindi (India)              |
         | Spanish (Brazil)           |

@ID:47 @ID:5791
#3
Scenario: Verify that user can’t enter value manually in the  Enrolled Date Field 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |
    And I enter "22/June/2022 02:37 PM" for "Enrolled Date" question
    Then the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |

@ID:47 @ID:5791
#4
Scenario: Verify that Date is displayed in the following DD/MMMM/YYYY format
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    | DateTimeFormat |
         | Subject Number | 016                     | Please provide a response | inputtextbox |                |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |                |
         | Language       | English (United States) | Please provide a response | dropdown     |                |
         | Current Status | Screened                | Please provide a response | dropdown     |                |         
    When I select "(Current date)" from existing "Enrolled Date" "datepicker"
    Then the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    | DateTimeFormat |
         | Subject Number | 016                     | Please provide a response | inputtextbox |                |
         | Enrolled Date  | 17/May/2022             | (Current date)            | datepicker   | dd/MMMM/yyyy   |
         | Language       | English (United States) | Please provide a response | dropdown     |                |
         | Current Status | Screened                | Please provide a response | dropdown     |                |

@ID:47 @ID:5791
#5
Scenario: Verify that user is not able to select future date. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |
    When I select "(Current date) +1" from existing "Enrolled Date" "datepicker"
    Then the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    |
         | Subject Number | 016                     | Please provide a response | inputtextbox |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |
         | Language       | English (United States) | Please provide a response | dropdown     |
         | Current Status | Screened                | Please provide a response | dropdown     |

@ID:47 @ID:5791
#6
Scenario: Data persisted when user clicks back and next button on the data correction page. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-016" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Information" from "Type Of Correction" dropdown
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    | DateTimeFormat |
         | Subject Number | 016                     | Please provide a response | inputtextbox |                |
         | Enrolled Date  | 17/May/2022             | Please provide a response | datepicker   |                |
         | Language       | English (United States) | Please provide a response | dropdown     |                |
         | Current Status | Screened                | Please provide a response | dropdown     |                |
   And "Next" button is disabled
   And I click on "dropdown" in data correction field for "Language" existing question
   And the following choices are displayed in dropdown for "Language" existing question
         | Choices                    |
         | Arabic (Algeria)           |
         | Chinese (Simplified, China)|
         | English (United States)    |
         | Hindi (India)              |
         | Spanish (Brazil)           |
   And I select "Hindi (India)" from existing "Language" "dropdown"
   And I select "(Current date)" from existing "Enrolled Date" "datepicker"
   And I enter "Subject Information" in "Reason For Correction" inputtextbox field
   And "Next" button is enabled
   And I click on "Next" button
   And I am on "Submit Data Correction" page
   And the following data is displayed in the correction data field table
         | Label          | Current Value           | Requested Value | FieldType | DateTimeFormat |
         | Subject Number | 016                     |                 | text      |                |
         | Enrolled Date  | 17/May/2022             | (Current date)  | text      | dd/MMMM/yyyy   |
         | Language       | English (United States) | Hindi (India)   | text      |                |
         | Current Status | Screened                |                 | text      |                |
    When I click on "Back" button
    Then I am back on "Create Data Correction" page
    And "Initial Site" is displayed for "Site Name" dropdown
    And "S-10001-016" is displayed for "Subject" dropdown
    And "Change subject Information" is displayed for "Type Of Correction" dropdown
    And "Subject Information" is displayed in "Reason For Correction" inputtextbox field
    Then the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | FieldType    | DateTimeFormat |
         | Subject Number | 016                     | Please provide a response | inputtextbox |                |
         | Enrolled Date  | 17/May/2022             | (Current date)            | datepicker   | dd/MMMM/yyyy   |
         | Language       | English (United States) | Hindi (India)             | dropdown     |                |
         | Current Status | Screened                | Please provide a response | dropdown     |                |
    And "Subject Information" is displayed in "Reason For Correction" inputtextbox field
   
   
