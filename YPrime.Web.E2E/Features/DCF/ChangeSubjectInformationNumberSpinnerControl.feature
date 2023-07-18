@Portal

Feature: Change Subject Information Number Spinner Control

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


@ID:5939
@ID:4958 @MockStudyBuilder
#1
#Min Value for weight - 0
#Max Value for weight - 120
#Min Value for height - 0.0
#Max Value for height - 200.0
#Decimal value 1
Scenario: Verify that error message is displayed when user enters value manually outside the min max range and clicks next button for Number Spinner and also verify that suffix is displayed as configured and not editable.
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
         | Subject Number | 015                     | Please provide a response | inputtextbox  |
         | [N/A]          | (Current Date)          | Please provide a response | datepicker    |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         | Please provide a response | datepicker    |
         | Weight         | 120                     | Please provide a response | numberspinner |
         | Height         | 4.6                     | Please provide a response | numberspinner |
    And "Lb" suffix is displayed for attribute "Weight"
    And "Height" suffix is displayed for attribute "Height" 
    And I enter "126" for "Weight" question 
    And I enter "208.6" for "Height" question 
    And I enter "Subject Attribute" in "Reason For Correction" inputtextbox field
    When I click on "Next" button
    Then the validation message "Weight entry is outside of the min/max range, please enter a value between 0 and 120." is displayed
    And the validation message "Height entry is outside of the min/max range, please enter a value between 0.0 and 200.0." is displayed

@ID:4958 @ID:5791 @MockStudyBuilder
#2
#Min Value for weight - 0
#Max Value for weight - 125
#Min Value for height - 0 
#Max Value for height - 8
#Decimal value 1
Scenario: Data is persisted when user clicks next and back button on data correction page
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
         | Subject Number | 015                     | Please provide a response | inputtextbox  |
         | [N/A]          | (Current Date)          | Please provide a response | datepicker    |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         | Please provide a response | datepicker    |
         | Weight         | 120                     | Please provide a response | numberspinner |
         | Height         | 4.6                     | Please provide a response | numberspinner |
    And "Lb" suffix is displayed for attribute "Weight"
    And "Height" suffix is displayed for attribute "Height" 
    And I enter "101" for "Weight" question 
    And I enter "7.9" for "Height" question 
    And I enter "Subject Attribute" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label          | Current Value           | Requested Value | Fieldtype |
        | Subject Number | 015                     |                 | text      |
        | [N/A]          | (Current Date)          |                 | text      |
        | [N/A]          | English (United States) |                 | text      |
        | Current Status | Screened                |                 | text      |
        | Gender         | Male                    |                 | text      |
        | Date of Birth  |                         |                 | text      |
        | Weight         | 120 Lb                  | 101 Lb          | text      |
        | Height         | 4.6 Height              | 7.9 Height      | text      |
    When I click on "Back" button           
    Then I am back on "Create Data Correction" page
    And "Initial Site" is displayed for "Site Name" dropdown
    And "S-10001-015" is displayed for "Subject" dropdown
    And "Change subject Information" is displayed for "Type Of Correction" dropdown
    And "Subject Attribute" is displayed in "Reason For Correction" inputtextbox field
    And the following data is displayed in the attribute data correction field
         | Label          | Current Value           | Requested Value           | Fieldtype     |
         | Subject Number | 015                     | Please provide a response | inputtextbox  |
         | [N/A]          | (Current Date)          | Please provide a response | datepicker    |
         | [N/A]          | English (United States) | Please provide a response | dropdown      |
         | Current Status | Screened                | Please provide a response | dropdown      |
         | Gender         | Male                    | Please provide a response | dropdown      |
         | Date of Birth  |                         | Please provide a response | datepicker    |
         | Weight         | 120                     | 101                       | numberspinner |
         | Height         | 4.6                     | 7.9                       | numberspinner |
    And "Lb" suffix is displayed for attribute "Weight"
    And "Height" suffix is displayed for attribute "Height"
    And "Subject Attribute" is displayed in "Reason For Correction" inputtextbox field







    
    
    

