@Portal


Feature: Paper DCF TempControl With Fahrenheit
	
Background: 
    Given Software Release "Config release 22" has been created with Software Version "1.0.0.0" and Configuration Version "22.0-03.25" and assigned to Study Wide
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-004" exists in the "Patients" table
    And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
  
@ID:689
#1
#C- Min value: 26.6 Max value:38.0
#F- Min value:80.0 Max value:100.4
Scenario: User should be able to select value within Min Max range 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype     |
        | Date of Questionnaire Completion:                  |                 | datepicker    |
        | Visit Name:                                        | Please Select   | dropdown      |
        | Please record your highest temperature for the day |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I scroll to minimum allowed value for question "Please record your highest temperature for the day" in PaperDCF
    And "80.0" is displayed for "Please record your highest temperature for the day" question in PaperDCF
    When I scroll to maximum allowed value for question "Please record your highest temperature for the day" in PaperDCF
    Then "100.4" is displayed for "Please record your highest temperature for the day" question in PaperDCF


    
@ID:689
#2
#C- Min value: 26.6 Max value:38.0
#F- Min value:80.0 Max value:100.4
Scenario: Verify that error message is displayed when user enters invalid value
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              | Value           | Fieldtype     |
        | Date of Questionnaire Completion:                  |                 | datepicker    |
        | Visit Name:                                        | Please Select   | dropdown      |
        | Please record your highest temperature for the day |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I enter "100.5" for "Please record your highest temperature for the day" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And "Next" button is disabled
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message is displayed for "Please record your highest temperature for the day" question

@ID:689
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
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype      |
        | Date of Questionnaire Completion:                  |                 | datepicker |
        | Visit Name:                                        | Please Select   | dropdown   |
        | Please record your highest temperature for the day |                 | NumberSpinner |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I enter "80.5" for "Please record your highest temperature for the day" question in PaperDCF
    And "80.5" is displayed for "Please record your highest temperature for the day" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                              | Value            | Fieldtype |
        | Date of Questionnaire Completion:                  | (Current Date)   | datepicker |
        | Visit Name:                                        | Please Select    | dropdown   |
        | Please record your highest temperature for the day | 80.5°F                  | text      |  
    When I click on "Back" button
    Then the following data is displayed in the data correction field
            | Label                                              | Value            | Fieldtype      |    
            | Date of Questionnaire Completion:                  | (Current Date)   | datepicker |
            | Visit Name:                                        | Please Select    | dropdown   |
            | Please record your highest temperature for the day | 80.5             | NumberSpinner |
    Then "Temp Control" is displayed in "Reason For Correction" inputtextbox field

@ID:689
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
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    When I select the given "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    Then  the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype      |
        | Date of Questionnaire Completion:                  |                 | datepicker     |
        | Visit Name:                                        | Please Select   | dropdown       |
        | Please record your highest temperature for the day |                 | NumberSpinner  |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF 


@ID:689
#5
Scenario: Verify that min max answers are stored in °C in DB, It should be displayed in °F for the Change questionnaire response when configured(US)
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-004" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select "Temperature Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                              |  Value          | Fieldtype      |
        | Date of Questionnaire Completion:                  |                 | datepicker     |
        | Visit Name:                                        | Please Select   | dropdown       |
        | Please record your highest temperature for the day |                 | NumberSpinner  |
    And "°F" suffix is displayed for question "Please record your highest temperature for the day" in PaperDCF
    And I enter "100.4" for "Please record your highest temperature for the day" question in PaperDCF
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data correction field
        | Label                                              |  Value               | Fieldtype |
        | Date of Questionnaire Completion:                  |  (Current Date)      | datepicker|
        | Visit Name:                                        | Please Select        | dropdown  |
        | Please record your highest temperature for the day | 100.4                | text      |
    And I click on "Submit" button
    And "Electronic Signature" popup is displayed
    And "User Name" has value "PortalE2EUser"
    And I enter "Welcome01!" for "Password"
    And I click "Ok" button in the popup
    And I am on "Data Correction Confirmation" page
    And "Success" popup is displayed with message "Correction has been added successfully."
    And I click "Ok" button in the popup
    And "38.0000" is saved in DB for "Add Paper Questionnaire" DCF type
