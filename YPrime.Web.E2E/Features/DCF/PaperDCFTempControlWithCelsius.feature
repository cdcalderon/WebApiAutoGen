@Portal

Feature: Paper DCF Temp Control With Celsius
	
Background: 
    Given Site "India Site" is assigned to Country "India" and has site number "20001"
    And User "PortalE2EUser" with role "YP" has access to site "20001"
    And Subject "S-20001-004" exists in the "Patients" table
	And Patient "patient 1" with patient number "S-20001-004" is associated with "India Site"
    And I have logged in with user "PortalE2EUser"
    And Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "22.0-03.25" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And "Phone" Device "YP-E2E-Device" is assigned to Site "India Site"   
    And I click on "Add New DCF" button


@ID:689
#1
Scenario: Verify that min max answers are stored in °C in DB, It should be displayed in °C for the Change questionnaire response when configured(India)
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "India Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-20001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype      |
        | Date of Questionnaire Completion:                  |                 | datepicker     |
        | Visit Name:                                        | Please Select   | dropdown       |
        | Please record your highest temperature for the day |                 | NumberSpinner |
    And "°C" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I enter "38.0" for "Please record your highest temperature for the day" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                              | Value          | Fieldtype  |
        | Date of Questionnaire Completion:                  | (Current Date) | datepicker |
        | Visit Name:                                        | Please Select  | dropdown   |
        | Please record your highest temperature for the day | 38.0           | text       |
    And I click on "Submit" button
    And "Electronic Signature" popup is displayed
    And "User Name" has value "PortalE2EUser"
    And I enter "Welcome01!" for "Password"
    And I click "Ok" button in the popup
    And I am on "Data Correction Confirmation" page
    And "Success" popup is displayed with message "Correction has been added successfully."
    And I click "Ok" button in the popup
    And "38.0000" is saved in DB for "Add Paper Questionnaire" DCF type