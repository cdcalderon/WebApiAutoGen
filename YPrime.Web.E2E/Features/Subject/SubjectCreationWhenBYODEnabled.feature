@ignore @manual @BYOD
Feature: Subject Creation when BYOD enabled
    PBI 96942

    #1
    Scenario: While creating a subject the following questions need to be displayed when BYOD enabled config is set to true and the validation message is displayed if the user does not select a response.
        Given I have a study configured using "yprime_e2e_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And "Add New Subject button" is enabled
        And I click on "Add New Subject button"
        And I am on "Add New Subject" page
        And the following data is displayed
            | Label                                       | Value | FieldType    |
            | Subject                                     |       | Numberinput  |
            | Gender                                      |       | Checkbox     |
            | Weight                                      |       | Numberinput  |
            | Free Text                                   |       | Inputtextbox |
            | Will the subject use their personal device? |       | Radio Button |
        And the following choices is displayed for "Will the subject use their personal device?"
            | RadioButtonChoicesName | ControlType |
            | Yes, personal device   | RadioButton |
            | No, provisioned device | RadioButton |
        And I enter the following data
            | Label     | Value      | FieldType    |
            | Subject   | 002        | Numberinput  |
            | Gender    | Female     | Checkbox     |
            | Weight    | 120        | Numberinput  |
            | Free Text | Pharmacy 1 | Inputtextbox |
        When I click on "Create button"
        Then a pop up is displayed
            | popupType | Message                                     | ActionButtons |
            | Warning   | Will the subject use their personal device? | Ok            |

    #2
    Scenario: Verify that the subject is created successfully, and the welcome email is displayed if the user selects YES
        Given I have a study configured using "yprime_e2e_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And "Add New Subject button" is enabled
        And I click on "Add New Subject button"
        And I am on "Add New Subject" page
        And I enter the following data
            | Label     | Value      | FieldType    |
            | Subject   | 002        | Numberinput  |
            | Gender    | Female     | Checkbox     |
            | Weight    | 120        | Numberinput  |
            | Free Text | Pharmacy 1 | Inputtextbox |
        And I click on  "Yes, personal device"
        And I click on "Create button"
        And a pop up is displayed
        When I click on "X icon"
        Then a pop up is not displayed

    #3
    Scenario: Verify that the subject is created successfully, and the welcome email is not displayed if the user selects NO
        Given I have a study configured using "yprime_e2e_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And "Add New Subject button" is enabled
        And I click on "Add New Subject button"
        And I am on "Add New Subject" page
        And I enter the following data
            | Label     | Value      | FieldType    |
            | Subject   | 002        | Numberinput  |
            | Gender    | Female     | Checkbox     |
            | Weight    | 120        | Numberinput  |
            | Free Text | Pharmacy 1 | Inputtextbox |
        And I click on  "No, provisioned device"
        And I click on "Create button"
        And  a pop up is displayed
            | popupType | Message                                                                         | ActionButtons |
            | Success   | Subject S-00001-001 has been added successfully with the default Pin of 123456. | Ok            |
        When I click on "Ok buton"
        Then a pop up is not displayed

    #4
    Scenario: Verify that response to questions Will subject use their personal device? is displayed in subject info page.
        Given I have a study configured using "yprime_e2e_byod"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And "Add New Subject button" is enabled
        And I click on "Add New Subject button"
        And I am on "Add New Subject" page
        And I enter the following data
            | Label     | Value      | FieldType    |
            | Subject   | 002        | Numberinput  |
            | Gender    | Female     | Checkbox     |
            | Weight    | 120        | Numberinput  |
            | Free Text | Pharmacy 1 | Inputtextbox |
        And I click on  "Yes, personal device"
        And I click on "Create button"
        And a pop up is displayed
        And I click on "X icon"
        And a pop up is not displayed
        When I click on "Subject 002"
        Then "Yes, personal device" is displayed for "Will the subject use their personal device?"





