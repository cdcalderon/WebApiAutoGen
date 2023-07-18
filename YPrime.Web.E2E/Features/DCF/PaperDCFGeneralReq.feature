@Portal


Feature: Paper DCF General Req

Background:     
    Given Software Release "Config release 35" has been created with Software Version "1.0.0.0" and Configuration Version "35.0-6.10" and assigned to Study Wide
    And I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-006" exists in the "Patients" table
    And Patient "patient 1" with patient number "S-10001-006" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"


@ID:4985 @ID:5791
#1
Scenario: Verify that default placeholder text is displayed for the Visit name and Date of the questionnaire completion field, and the user can move forward with or without entering a response to questions.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-006" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Diary Questionnaire" from "Select a Questionnaire" dropdown
    And  "Please provide a response" displayed in "Date of Questionnaire Completion" placeholder
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker    |
        | Visit Name:                                                      | Please provide a response | dropdown      |
        | Please record your highest temperature for today                 | Please provide a response | NumberSpinner |
        | How are you feeling at this moment?                              | Please provide a response | dropdown      |
        | Please record your pain in the scale of 0-100                    | Please provide a response | dropdown      |
        | Please tap on the ONE box that best describes your health TODAY. | Please provide a response | dropdown      |
        | Please select all symptoms that apply today.                     |                           | multiselect   |
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And "°F" suffix is displayed for question "Please record your highest temperature for today" in PaperDCF
    And I scroll to minimum allowed value for question "Please record your highest temperature for today" in PaperDCF
    And "80.0" is displayed for "Please record your highest temperature for today" question in PaperDCF
    And "Next" button is disabled
    And I enter "Temp Control" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then I am on "Submit Data Correction" page

@ID:4985 @ID:5791
#2
Scenario: Verify that error message is displayed when date of questionnaire completion field  is left blank 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-006" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Diary Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker    |
        | Visit Name:                                                      | Please provide a response | dropdown      |
        | Please record your highest temperature for today                 | Please provide a response | NumberSpinner |
        | How are you feeling at this moment?                              | Please provide a response | dropdown      |
        | Please record your pain in the scale of 0-100                    | Please provide a response | dropdown      |
        | Please tap on the ONE box that best describes your health TODAY. | Please provide a response | dropdown      |
        | Please select all symptoms that apply today.                     |                           | multiselect   |
    And "°F" suffix is displayed for question "Please record your highest temperature for today" in PaperDCF
    And I scroll to minimum allowed value for question "Please record your highest temperature for today" in PaperDCF
    And "80.0" is displayed for "Please record your highest temperature for today" question in PaperDCF
    And "Next" button is disabled
    And I enter "Date of Questionnaire Completion" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message is displayed for "Date of Questionnaire Completion" question

@ID:4985 @ID:5791
#3
Scenario: Verify that the question and choice value is displayed in the order as configured in SB 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-006" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    When I select the given "Diary Questionnaire" from "Select a Questionnaire" dropdown
    Then the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker    |
        | Visit Name:                                                      | Please provide a response | dropdown      |
        | Please record your highest temperature for today                 | Please provide a response | NumberSpinner |
        | How are you feeling at this moment?                              | Please provide a response | dropdown      |
        | Please record your pain in the scale of 0-100                    | Please provide a response | dropdown      |
        | Please tap on the ONE box that best describes your health TODAY. | Please provide a response | dropdown      |
        | Please select all symptoms that apply today.                     |                           | multiselect   |
   When I click on "dropdown" in data correction field for "How are you feeling at this moment?" question
   Then the following choices are displayed in dropdown for "How are you feeling at this moment?" question
        | Choices         |
        | Good            |
        | Bad             |
        | Almost better   |
        | Much Better     |
        | About the Same  |
        | Somewhat better |
    When I click on "dropdown" in data correction field for "Please record your pain in the scale of 0-10" question
    Then the following choices are displayed in dropdown for "Please record your pain in the scale of 0-10" question
        | Choices |
        | 0%      |
        | 10%     |
        | 20%     |
        | 30%     |
        | 40%     |
        | 50%     |
        | 60%     |
        | 70%     |
        | 80%     |
        | 90%     |
        | 100%    |
    When I click on "dropdown" in data correction field for "Please tap on the ONE box that best describes your health TODAY" question
    Then the following choices are displayed in dropdown for "Please tap on the ONE box that best describes your health TODAY" question
        | Choices                                             |
        | Please Select                                       |
        | I have no problems washing or dressing myself       |
        | I have slight problems washing or dressing myself   |
        | I have moderate problems washing or dressing myself |
    And the following choices are displayed for "Please select all symptoms that apply today." question
        | Value    |
        | Headache |
        | Nausea   |
        | Fatigue  |
            
@ID:4985 @ID:5791
#4
Scenario: Verify that user is able to select only previous date and current date in the date of questionnaire completion field 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-006" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Diary Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the data correction field
        | Label                                                            | Value                     | Fieldtype     |
        | Date of Questionnaire Completion:                                | Please provide a response | datepicker    |
        | Visit Name:                                                      | Please provide a response | dropdown      |
        | Please record your highest temperature for today                 | Please provide a response | NumberSpinner |
        | How are you feeling at this moment?                              | Please provide a response | dropdown      |
        | Please record your pain in the scale of 0-100                    | Please provide a response | dropdown      |
        | Please tap on the ONE box that best describes your health TODAY. | Please provide a response | dropdown      |
        | Please select all symptoms that apply today.                     |                           | multiselect   |
    Then I select "(Current date) +1" from "Date of Questionnaire Completion:" "datepicker"
    And the following data is displayed in the data correction field
            | Label                                   | Value           | Fieldtype    |
            | Date of Questionnaire Completion:       |                 | datepicker   |
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And the following data is displayed in the data correction field
            | Label                                   | Value           | Fieldtype    |
            | Date of Questionnaire Completion:       | (Current date)  | datepicker   |
    And I select "(Current date) -1" from "Date of Questionnaire Completion:" "datepicker" 
    And the following data is displayed in the data correction field
            | Label                                   | Value               | Fieldtype    |
            | Date of Questionnaire Completion:       | (Current date) -1   | datepicker   |
