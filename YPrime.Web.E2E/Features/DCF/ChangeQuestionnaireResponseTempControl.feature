@Portal

Feature: DCF Change Questionnaire Response For Temp Control with Fahrenheit

Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 16" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-004" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-004" has completed "Temperature Spinner Questionnaire" questionnaire for question "Please record your highest temperature for the day" and value "80.1 °F"

@ID:688
@MockStudyBuilder
#1
#C- Min value: 35 Max value:40.5
#F- Min value: 95.0 Max value:105.0
Scenario: User should be able to select value within Min Max range 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype     |
        | Please record your highest temperature for the day | 80.1°F        |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"
    And I scroll to minimum allowed value for question "Please record your highest temperature for the day" 
    And "95.0" is displayed for "Please record your highest temperature for the day" question 
    When I scroll to maximum allowed value for question "Please record your highest temperature for the day" 
    Then "105.0" is displayed for "Please record your highest temperature for the day" question 

@ID:5938
@ID:688
@MockStudyBuilder
#2
#C- Min value: 35.0 Max value: 40.6
#F- Min value: 95.0 Max value: 105.0
Scenario: Verify that error message is displayed when user enters invalid value
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype      |
        | Please record your highest temperature for the day | 80.1°F        |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"
    And I enter "110.5" for "Please record your highest temperature for the day" question
    And "Next" button is disabled
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message "entry is outside of the min/max range, please enter a value between 95.0°F and 105.0°F." is displayed for question "Please record your highest temperature for the day"

@ID:688
@MockStudyBuilder
#3
#C- Min value: 26.6 Max value:38.0
#F- Min value:80.0 Max value:100.4
Scenario: Data is persisted when user clicks Next and Back button on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype     |
        | Please record your highest temperature for the day | 80.1°F        |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"
    And I enter "100.2" for "Please record your highest temperature for the day" question
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                              | Current Value | Requested Value | Fieldtype |
        | Please record your highest temperature for the day | 80.1°F        | 100.2           | text      |
    And I click on "Back" button
    And the following data is displayed in the existing question correction fields
            | Label                                              | Current Value | Requested Value | Fieldtype      |            
            | Please record your highest temperature for the day | 80.1°F        | 100.2           | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"
    Then "Temp Control" is displayed in "Reason For Correction" inputtextbox field

@ID:688
@MockStudyBuilder
#4
#C- Min value: 26.6 Max value:38.0
#F- Min value:80.0 Max value:100.4
Scenario: Verify that suffix displayed is based on subject country
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    When I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    Then the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype      |
        | Please record your highest temperature for the day | 80.1°F        |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"   


@ID:688
@MockStudyBuilder
#5
Scenario: Verify that min max answers are stored in °C in DB, It should be displayed in °F for the Change questionnaire response when configured(US)
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype      |
        | Please record your highest temperature for the day | 80.1°F        |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day"
    And I enter "100.4" for "Please record your highest temperature for the day" question
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data field table
        | Label                                              | Value             | Fieldtype |
        | Please record your highest temperature for the day | 100.4°F           | text      |
    And I click on "Submit" button
    And "Electronic Signature" popup is displayed
    And "User Name" has value "PortalE2EUser"
    And I enter "Welcome01!" for "Password"
    And I click "Ok" button in the popup
    And I am on "Data Correction Confirmation" page
    And "Success" popup is displayed with message "Correction has been added successfully."
    And I click "Ok" button in the popup
    And "38.0000" is saved in DB for "Change questionnaire responses" DCF type


