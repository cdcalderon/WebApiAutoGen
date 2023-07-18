@Portal

Feature: DCF Change Questionnaire Response For Temp Control with Celsius

Background:
    Given Site "India Site" is assigned to Country "India" and has site number "20001"
    And User "PortalE2EUser" with role "YP" has access to site "20001"
    And Subject "S-20001-004" exists in the "Patients" table
	And Patient "patient 1" with patient number "S-20001-004" is associated with "India Site"
    And I have logged in with user "PortalE2EUser"
    And Software Release "Config release 16" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And "Phone" Device "YP-E2E-Device" is assigned to Site "India Site"
    And Subject "S-20001-004" has completed "Temperature Spinner Questionnaire" questionnaire for question "Please record your highest temperature for the day" and value "26.7 °C"
    And I click on "Add New DCF" button
   
@ID:5938
@ID:688
@MockStudyBuilder
#1
#C- Min value: 35.0 Max value: 40.6
#F- Min value: 95.0 Max value: 105.0
Scenario: Verify that min max answers are stored in °C in DB, It should be displayed in °C for the Change questionnaire response when configured(India)
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "India Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-20001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                              | Current Value | Requested Value | Fieldtype      |
        | Please record your highest temperature for the day | 26.7°C       |                 | NumberSpinner |
    And "°C" suffix is displayed for question "Please record your highest temperature for the day"
    And I enter "41.0" for "Please record your highest temperature for the day" question
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And the validation message "entry is outside of the min/max range, please enter a value between 35.0°C and 40.6°C" is displayed for question "Please record your highest temperature for the day"
    And I enter "35.6" for "Please record your highest temperature for the day" question
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                                    |  Value          | Fieldtype |
        | Please record your highest temperature for the day       | 35.6°C          | text      |
    And I click on "Submit" button
    And "Electronic Signature" popup is displayed
    And "User Name" has value "PortalE2EUser"
    And I enter "S1thr3yaQV-%" for "Password"
    And I click "Ok" button in the popup
    And I am on "Data Correction Confirmation" page
    And "Success" popup is displayed with message "Correction has been added successfully."
    And I click "Ok" button in the popup
    And "35.6000" is saved in DB for "Change questionnaire responses" DCF type
