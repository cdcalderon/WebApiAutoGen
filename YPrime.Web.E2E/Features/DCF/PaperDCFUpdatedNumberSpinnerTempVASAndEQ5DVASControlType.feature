@Portal

Feature: Paper DCF Updated Number Spinner,Temp Control Type,VAS and EQ5D-VAS

Background:     
      Given Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "44.0-6.11" and assigned to Study Wide
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-004" exists in the "Patients" table
    And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"

@ID:5940
@ID:3840
#1
Scenario: Verify that error message is displayed when user enters value manually outside the min max range.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "Symptom Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                  |                           | datepicker    |
        | Visit Name:                                        | Please provide a response | dropdown      |
        | How many pills did you take?                       |                           | NumberSpinner |
        | Please record your highest temperature for the day |                           | NumberSpinner |
    And "Count" suffix is displayed for question "How many pills did you take?" in PaperDCF
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    #Min value 0 max value - 10
    #F- Min value:80.0 Max value:100.4
    And I enter "11" for "How many pills did you take?" question in PaperDCF
    And I enter "100.5" for "Please record your highest temperature for the day" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And "Next" button is disabled
    And I enter "Control Type" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message "entry is outside of the min/max range, please enter a value between 0 and 10." is displayed for question "How many pills did you take?"
    And the validation message "entry is outside of the min/max range, please enter a value between 80.0°F and 100.4°F." is displayed for question "Please record your highest temperature for the day"
    

@ID:3840
#2
Scenario: Data is persisted when user clicks Next and Back button on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "Symptom Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                  |                           | datepicker    |
        | Visit Name:                                        | Please provide a response | dropdown      |
        | How many pills did you take?                       |                           | NumberSpinner |
        | Please record your highest temperature for the day |                           | NumberSpinner |
    And "Count" suffix is displayed for question "How many pills did you take?" in PaperDCF
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    #Min value 0 max value - 10
    #F- Min value:80.0 Max value:100.4
    And I enter "9" for "How many pills did you take?" question in PaperDCF
    And I enter "98.9" for "Please record your highest temperature for the day" question in PaperDCF
    And I enter "Temp Control and Number Spinner" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                              | Value                     | Fieldtype  |
        | Date of Questionnaire Completion:                  | (Current Date)            | datepicker |
        | Visit Name:                                        | Please provide a response | dropdown   |
        | How many pills did you take?                       | 9 Count                   | text       |
        | Please record your highest temperature for the day | 98.9°F                    | text       |
    When I click on "Back" button
    Then the following data is displayed in the data correction field
            | Label                                              | Value                     | Fieldtype     |
            | Date of Questionnaire Completion:                  | (Current Date)            | datepicker    |
            | Visit Name:                                        | Please provide a response | dropdown      |
            | How many pills did you take?                       | 9                         | NumberSpinner |
            | Please record your highest temperature for the day | 98.9                      | NumberSpinner |
    And "Temp Control and Number Spinner" is displayed in "Reason For Correction" inputtextbox field



@ID:3840
#3
Scenario: Verify that error message is displayed when user enters value manually outside the min max range for VAS/EQ5D-VAS.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select "VAS/EQ5D-VAS Questionnaire" from "Select a Questionnaire" dropdown
        And the following data is displayed in the data correction field
        | Label                                                      | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                          |                           | datepicker    |
        | Visit Name:                                                | Please provide a response | dropdown      |
        | VAS: How much pain are you currently feeling?              |                           | NumberSpinner |
        | We would like to know how good or bad your health is TODAY |                           | NumberSpinner |
    #Min value 0 max value - 100
    #EQ5D Min value-0 max value - 100
    And I enter "101" for "VAS: How much pain are you currently feeling?" question in PaperDCF
    And I enter "103" for "We would like to know how good or bad your health is TODAY" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And "Next" button is disabled
    And I enter "Control Type" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message "entry is outside of the min/max range, please enter a value between 0 and 100." is displayed for question "VAS: How much pain are you currently feeling?"
    And the validation message "entry is outside of the min/max range, please enter a value between 0 and 100." is displayed for question "We would like to know how good or bad your health is TODAY"