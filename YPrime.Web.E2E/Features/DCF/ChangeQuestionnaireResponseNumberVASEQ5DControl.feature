@Portal
Feature: Change Questionnaire Response Number,VAS, and EQ5D Control

Background:
    Given I have logged in with user "PortalE2EUser"
    And Software Release "Config release 41" has been created with Software Version "1.0.0.0" and Configuration Version "44.0-6.11" and assigned to Study Wide
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button
    And Subject "S-10001-007" exists in the "Patients" table
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Subject "S-10001-007" has completed "Number Spinner Questionnaire" questionnaire for question "How much pain are you currently feeling?" and value "9"
    And Subject "S-10001-007" has completed "Number Spinner Questionnaire" questionnaire for question "How many hours did you sleep?" and value "3"
    And Subject "S-10001-007" has completed "Number Spinner Questionnaire" questionnaire for question "What is your current weight?" and value "115.5"
    And Subject "S-10001-007" has completed "Number Spinner Questionnaire" questionnaire for question "Your Health Today" and value "65"

@ID:4941 @ID:5791
#VAS- Min 0 Max 100
#Number Spinner - Min 0- Max 10
#Number Spinner with decimal- Min 10 Max -125 1 decimal place 
#Eq5D- Min 0- Max 100
#1
Scenario: Verify that error message is displayed when user enters value outside the min max range, and also verify that suffix is displayed as configured. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-007" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Number Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                    | Current Value | Requested Value           | Fieldtype     |
        | How much pain are you currently feeling? | 9             | Please provide a response | NumberSpinner |
        | How many hours did you sleep?            | 3             | Please provide a response | NumberSpinner |
        | What is your current weight?             | 115.5         | Please provide a response | NumberSpinner |
        | Your Health Today                        | 65            | Please provide a response | NumberSpinner |
    And "Hours" suffix is displayed for question "How many hours did you sleep?"
    And "Lbs" suffix is displayed for question "What is your current weight?"
    And I enter "101" for "How much pain are you currently feeling?" question
    And I enter "11" for "How many hours did you sleep?" question
    And I enter "125.1" for "What is your current weight?" question
    And I enter "101" for "Your Health Today" question
    And I enter "Spinner" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    When I click on "Next" button
    Then the validation message "entry is outside of the min/max range, please enter a value between 0 and 100." is displayed for question "How much pain are you currently feeling?"
    And the validation message "entry is outside of the min/max range, please enter a value between 0 and 10." is displayed for question "How many hours did you sleep?"
    And the validation message "entry is outside of the min/max range, please enter a value between 10 and 125." is displayed for question "What is your current weight?"
    And the validation message "entry is outside of the min/max range, please enter a value between 0 and 100." is displayed for question "Your Health Today"

@ID:4941 @ID:5791
#VAS- Min 0 Max 100
#Number Spinner - Min 0- Max 10
#Number Spinner with decimal- Min 10 Max -125 1 decimal place 
#Eq5D- Min 0- Max 100
#2
Scenario: Data is persisted when user click next and back button on data correction page.   
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-007" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change questionnaire responses" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Select a Questionnaire" dropdown
    And I select the given "Number Spinner Questionnaire" from "Select a Questionnaire" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                                    | Current Value | Requested Value           | Fieldtype     |
        | How much pain are you currently feeling? | 9             | Please provide a response | NumberSpinner |
        | How many hours did you sleep?            | 3             | Please provide a response | NumberSpinner |
        | What is your current weight?             | 115.5         | Please provide a response | NumberSpinner |
        | Your Health Today                        | 65            | Please provide a response | NumberSpinner |
    And "Hours" suffix is displayed for question "How many hours did you sleep?"
    And "Lbs" suffix is displayed for question "What is your current weight?"
    And I enter "98" for "How much pain are you currently feeling?" question
    And I enter "10" for "How many hours did you sleep?" question
    And I enter "124.9" for "What is your current weight?" question
    And I enter "100" for "Your Health Today" question
    And "Next" button is disabled
    And I enter "Spinner" in "Reason For Correction" inputtextbox field
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label                                    | Current Value  | Requested Value   | Fieldtype |
        | How much pain are you currently feeling? | 9              | 98                | text      |
        | How many hours did you sleep?            | 3 Hours        | 10 Hours          | text      |
        | What is your current weight?             | 115.5 Lbs      | 124.9 Lbs         | text      |
        | Your Health Today                        | 65             | 100               | text      |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields
        | Label                                    | Current Value  | Requested Value   | Fieldtype     |
        | How much pain are you currently feeling? | 9              | 98                | NumberSpinner |
        | How many hours did you sleep?            | 3              | 10                | NumberSpinner |
        | What is your current weight?             | 115.5          | 124.9             | NumberSpinner |
        | Your Health Today                        | 65             | 100               | NumberSpinner |
    And "Hours" suffix is displayed for question "How many hours did you sleep?"
    And "Lbs" suffix is displayed for question "What is your current weight?"
    And "Spinner" is displayed in "Reason For Correction" inputtextbox field





   


