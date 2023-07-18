@Regression
Feature: Subject Management Regression

Background: 
    Given I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Subject" link on top navigation bar
    And I click on "All Sites" dropdown
    And I select "Initial Site" from "All Sites" dropdown
    And "Add New Subject" button is enabled
    And I click on "Add New Subject" button
    And I am on "Add New Subject" page

#1
Scenario: The subject is successfully created for BYOD subject
    Given I am on "Add New Subject" page
    And I enter "<SubjectId>" in Subject field
    And I click on "Yes, subject will use their personal device" button
    And I enter the following data
        | Label         | Value       | FieldType    |
        | Gender        | Male        | Radio Button |
        | Weight        | 100         | Numberinput  |
        | Height        | 050         | Numberinput  |   
        | Date of Birth | CurrentDate | datepicker   |   
    And I click on "Create" button
    And I close the subject welcome page

#2
Scenario: The subject is successfully created for non-BYOD subject
    Given I am on "Add New Subject" page
    And I enter "<SubjectId>" in Subject field
    And I click on "No, subject will use a provisioned device" button
    And I enter the following data
        | Label         | Value       | FieldType    |            
        | Gender        | Male        | Radio Button |
        | Weight        | 100         | Numberinput  |
        | Height        | 050         | Numberinput  |   
        | Date of Birth | CurrentDate | datepicker   |   
    And I click on "Create" button
    And I click "Ok" button in the popup
        
#3
Scenario: Create BYOD subject, Add DCF and Verify Report
    Given I am on "Add New Subject" page
    And I enter "<SubjectId>" in Subject field
    And I click on "Yes, subject will use their personal device" button
    And I enter the following data
        | Label         | Value       | FieldType    |
        | Gender        | Male        | Radio Button |
        | Weight        | 100         | Numberinput  |
        | Height        | 050         | Numberinput  |   
        | Date of Birth | CurrentDate | datepicker   |   
    And I click on "Create" button
    And I close the subject welcome page      
    And I select Subject "<SubjectId>"
    And I am on "Subject Management" page
    And I click on "Data Correction" button in Subject Management page
    And I click on "Type Of Correction" dropdown
    And I select "Add Paper Questionnaire" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I enter "Paper DCF" in "Reason For Correction" inputtextbox field
    And I click on "Select a Questionnaire" dropdown
    And I select "Questionnaire Forms" from "Select a Questionnaire" dropdown
    And I select "(Current date)" from "Date of Questionnaire Completion:" "datepicker"
    And I click on "dropdown" in data correction field for "In general, How would you say your health is:" question    
    And I select "Much Better" from "In general, How would you say your health is:" "dropdown"
    And I click on "dropdown" in data correction field for "How would you rate your health in general?" question    
    And I select "Good" from "How would you rate your health in general?" "dropdown"       
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the data field table
        | Label                                         | Value                 | Fieldtype |
        | Questionnaire:                                | Questionnaire Forms   | text      |
        | Date of Questionnaire Completion              | (Current Date)        | text      |
        | Visit Name                                    |                       | text      |
        | In general, How would you say your health is: | Much Better           | text      |
        | How would you rate your health in general?    | Good                  | text      |
    And I click on "Submit" button
    And "Electronic Signature" popup is displayed
    And "User Name" has value "PortalE2EUser"
    And I enter "Welcome01!" for "Password"
    And I click "Ok" button in the popup
    And I am on "Data Correction Confirmation" page
    And "Success" popup is displayed with message "Correction has been added successfully."
    And I click "Ok" button in the popup
    And I click on "Subject" link on the top navigation bar
    And I select Subject "<SubjectId>"
    And I am on Subject "<SubjectId>" page
    And I click on subject "Questionnaires" tab
    When I click on "Questionnaire Forms" diary entry		
    Then I am on "Diary Entry Details" page
    And "Paper" is displayed in "Data Source Name" details field
    And "Questionnaire Forms" is displayed in "Questionnaire Display Name" details field
    Then I click on "Analytics & Reports" link on top navigation bar    
    And I click on "DCF Status Report" from the Report List    
	Then Newly completed DCF is displayed in DCF Status Report
		| Site  | Subject     | DCF Type                | DCF Status | DCF Opened Date | DCF Closed Date | Pending Approver | Completed Approvals | Days DCF Open |
		| 10001 | <SubjectId> | Add Paper Questionnaire | Completed  | (Current Date)  | (Current Date)  | N/A              | N/A                 | 1             |
    And I click on "Answer Audit Report" from the Report List        
    And I select "Initial Site" from "Site Number" report dropdown
    And I select "<SubjectId>" from "Subject Number" report dropdown
    And I click on "Display the report" button
    Then Answers displayed in Answer Audit Report
        | Protocol | Site Number | Subject Number | Diary Date     | Questionnaire       | Question                                      | New Value   | Change Reason | Change By     | Change Date    | Correction Reason |
        | eCOA_e2e | 10001       | <SubjectId>    | (Current Date) | Questionnaire Forms | How would you rate your health in general?    | Good        | New           | PortalE2EUser | (Current Date) | Paper DCF         |
        | eCOA_e2e | 10001       | <SubjectId>    | (Current Date) | Questionnaire Forms | In general, How would you say your health is: | Much Better | New           | PortalE2EUser | (Current Date) | Paper DCF         |

#4
Scenario: Create BYOD subject, Change Subject Status and Verify Audit Report
    Given I am on "Add New Subject" page
    And I enter "<SubjectId>" in Subject field
    And I click on "Yes, subject will use their personal device" button
    And I enter the following data
        | Label         | Value       | FieldType    |
        | Gender        | Male        | Radio Button |
        | Weight        | 100         | Numberinput  |
        | Height        | 050         | Numberinput  |   
        | Date of Birth | CurrentDate | datepicker   |   
    And I click on "Create" button
    And I close the subject welcome page      
    And I select Subject "<SubjectId>"
    And I am on "Subject Management" page
    And I click on "Change Subject Status" button in Subject Management page
    And I select "Enrolled" from "Patient Status" Subject Management dropdown
    And I click on "Save" button
    And "Success" popup is displayed
    And I click "Ok" button in the popup
    Then I click on "Analytics & Reports" link on top navigation bar    
    And I click on "Subject Information Audit Report" from the Report List        
    And I select "Initial Site" from "Site Number" report dropdown
    And I select "<SubjectId>" from "Subject Number" report dropdown
    And I click on "Display the report" button
    And I click on "Next" page
    Then Patient status update listed in Subject Information Audit Report
         | Site Number | Subject Number | Subject Attribute              | Old Value | New Value               | Change Reason | Change By      | Change Date    |
         | 10001       | <SubjectId>    | Current Status                 |           | Screened                | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Date of Birth                  |           | (Current Date)          | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Gender                         |           | Male                    | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Height                         |           | 050                     | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Is Handheld Training Completed |           | No                      | New           | PortalE2EUserm | (Current Date) |
         | 10001       | <SubjectId>    | Language                       |           | English (United States) | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | PIN                            |           | ######                  | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Subject Number                 |           | <SubjectId>             | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Weight                         |           | 100                     | New           | PortalE2EUser  | (Current Date) |
         | 10001       | <SubjectId>    | Current Status                 | Screened  | Enrolled                | Update        | PortalE2EUser  | (Current Date) |

#5
Scenario: Create BYOD subject, View Enrollment Information and Reset Pin
    Given I am on "Add New Subject" page
    And I enter "<SubjectId>" in Subject field
    And I click on "Yes, subject will use their personal device" button
    And I enter the following data
        | Label         | Value       | FieldType    |
        | Gender        | Male        | Radio Button |
        | Weight        | 100         | Numberinput  |
        | Height        | 050         | Numberinput  |   
        | Date of Birth | CurrentDate | datepicker   |   
    And I click on "Create" button
    And I close the subject welcome page      
    And I select Subject "<SubjectId>"
    And I am on "Subject Management" page
    And I click on "BYOD Enrollment Information" button in Subject Management page
    And I close the subject welcome page
    And I click on "Reset PIN" button in Subject Management page
    And I click on "Cancel" button in Subject Management page
    And I click on "Reset PIN" button in Subject Management page
    And I click on "Update Pin" button in Subject Management page
    And "Success" popup is displayed with message "Temporary PIN has been reset."
    And I click "Ok" button in the popup