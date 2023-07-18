@Portal
Feature: Change Subject Visit DCF Update Visit Status

Background:
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001" 
    And I have logged in with user "PortalE2EUser"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4" and assigned to Study Wide
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And Patient "patient 1" with patient number "S-10001-024" is associated with "Initial Site"
    And "Treatment Visit 1" for "patient 1" is in "Complete" status
    And "Treatment Visit 2" for "patient 1" is in "Partial" status
    And "Treatment Visit 3" for "patient 1" is in "In Progress" status
    And "Treatment Visit 4" for "patient 1" is in "Missed" status 
    And "Treatment Visit 5" for "patient 1" is in "Not Started" status
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button

#Visit status is updated from Complete to Not Started
@MockStudyBuilder 
@ID:4848
#1
Scenario: Verify that Visit Date and Visit Activation Date is cleared and greyed out when status is updated from Complete to Not Started and the message is displayed and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Complete" from "Please select a subject visit" dropdown 
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | Complete             | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Not Started" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status back to "Not Started" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And the following data is displayed in the existing question correction fields
         | Label                  | Current Value         | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Complete              | Not Started               |              | dropdown   |           |
         | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | false     |



#Visit Status updated from Complete to Missed
@MockStudyBuilder 
@ID:4848
#2
Scenario: Verify that visit date and visit activation date is cleared and grayed out when status is updated from "Complete to "Missed"
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Complete" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value         | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | Complete              | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Missed" from existing "Visit Status:" "dropdown"
    Then the following data is displayed in the existing question correction fields
         | Label                  | Current Value         | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Complete              | Missed                    |              | dropdown   |           |
         | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | false     |

#Visit Status updated from Complete to In Progress
#3
@MockStudyBuilder 
@ID:4848
Scenario: Verify that remove value toggle is grayed out and user is able to edit or maintain the current value of visit date and visit activation date when Visit Status is updated from “Complete” to "In Progress" and the appropriate message is displayed
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Complete" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | Complete             | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
   When I select "In Progress" from existing "Visit Status:" "dropdown"
   Then the DCF Visit Status message "By changing the visit status back to "In Progress" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
   And I select "(Current date)" from existing "Visit Date:" "datepicker"
   And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |  DateTimeFormat | IsEnabled | IsRemoveEnabled |
        | Visit Status:          | Complete             | In Progress               |              | dropdown   |                 |           |                 |
        | Visit Date:            | (Current Date score) | (Current Date)            |              | datepicker | dd-MMM-yyyy     | true      | false           |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |                 | true       | false           |

#Visit Status is updated from Complete to Partial 
@MockStudyBuilder 
@ID:4848
#4
Scenario: Verify that remove value toggle is grayed out and user is able to edit or maintain the current value of visit date and visit activation date when Visit Status is updated from “Complete” to "Partial" and the appropriate message is displayed
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Complete" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields  
        | Label                 | Current Value | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | Complete      | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | true |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | true |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
   When I select "Partial" from existing "Visit Status:" "dropdown"
   Then I select "(Current date)" from existing "Visit Date:" "datepicker"
   Then the following data is displayed in the existing question correction fields
        | Label                  | Current Value         | Requested Value           | Remove Value | Fieldtype  | DateTimeFormat | IsEnabled | IsRemoveEnabled |
        | Visit Status:          | Complete              | Partial                   |              | dropdown   |                |           |                 |
        | Visit Date:            | (Current Date score)  | (Current Date)            |              | datepicker | dd-MMM-yyyy    | true      | false           |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker |                | true      | false           |

#Visit Status is updated from "Partial" to "Not Started"
@MockStudyBuilder 
@ID:4848
#5
Scenario: Verify that the Visit date and visit activation date is cleared and grayed out when Visit Status is updated from “Partial” to "Not Started" and the appropriate message is displayed. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Partial" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value         | Requested Value           | Remove Value | Fieldtype  |  IsEnabled |
        | Visit Status:          | Partial               | Please provide a response |              | dropdown   |            |
        | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | true       |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | true       |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Not Started" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status back to "Not Started" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Partial              | Not Started               |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | false     |
    

#Visit status is updated from Partial to Complete. 
#Only Site forms are completed
@MockStudyBuilder 
@ID:4848
#6
 Scenario: Verify that the Remove Value option is greyed out and the user is able to edit or maintain the current value for Visit Date and Visit activation date when visit status is updated from Partial to Complete and appropriate message is displayed. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Partial" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields  
        | Label                 | Current Value | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Partial       | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
   When I select "Complete" from existing "Visit Status:" "dropdown"
   Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
   And I select "(Current date)" from existing "Visit Date:" "datepicker"
   And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
         | Visit Status:          | Partial              | Complete                  |              | dropdown   |                |                 |
         | Visit Date:            | (Current Date score) | (Current Date)            |              | datepicker | dd-MMM-yyyy    | false           |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |                | false           |
   
#Visit Status is updated from "Partial" to "In progress"
@MockStudyBuilder 
@ID:4848
#7
Scenario: Verify that the Remove Value option is greyed out and user is able to edit or maintian the current value for Visit Date and Visit Activation Date when visit status is updated from "Partial" to "In progress" and the appropriate message is displayed 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Partial" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields  
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |  
        | Visit Status:          | Partial              | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
   When I select "In Progress" from existing "Visit Status:" "dropdown"
   Then the DCF Visit Status message "By changing the visit status back to "In Progress" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
   And I select "(Current date)" from existing "Visit Date:" "datepicker"
   And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | DateTimeFormat | IsEnabled | IsRemoveEnabled |
        | Visit Status:          | Partial              | In Progress               |              | dropdown   |                |           |                 |
        | Visit Date:            | (Current Date score) | (Current Date)            |              | datepicker |  dd-MMM-yyyy   | true      | false           |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |                | true      | false           |
    
#Visit Status is updated from "Partial" to "In progress"
#Only Site forms are completed
@MockStudyBuilder 
@ID:4848
#8
Scenario: Verify that if visit date or visit activation date is not updated then the default current value will remain as visit date. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Partial" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Partial              | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
   When I select "In Progress" from existing "Visit Status:" "dropdown"
   Then the DCF Visit Status message "By changing the visit status back to "In Progress" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
   And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Partial              | In Progress               |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |



#VIsit Status update from Partial to Missed
@MockStudyBuilder 
@ID:4848
#9
Scenario: Verify that the Visit Date and Visit Activation Date is cleared and greyed out when visit status is updated from "Partial" to "Missed" and appropriate message is displayed. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "Partial" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | Partial              | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Missed" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Missed" upon successful device sync, the visit will no longer be available for completion." is displayed
    And the following data is displayed in the existing question correction fields
         | Label                  | Current Value         | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Partial               | Missed                    |              | dropdown   |           |
         | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | false     |




#Visit Status updated from In Progress to Not Started
@MockStudyBuilder 
@ID:4848
#10
Scenario: Verify that visit date and visit activation date is cleared and grayed out and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value         | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | In Progress           | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Not Started" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status back to "Not Started" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And the following data is displayed in the existing question correction fields
         | Label                  | Current Value         | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | In Progress           | Not Started               |              | dropdown   |           |
         | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | false     |



#Visit status is updated from in Progress  to Complete. 
#If Only Site Forms are completed 
@MockStudyBuilder 
@ID:4848
#11
Scenario: Verify that the visit date is a required field and the visit activation date is not a mandatory field when the visit status is updated from In Progress or Partial to Complete.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
       | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat |
       | Visit Status:          | In Progress          | Complete        |              | dropdown   |                |
       | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    |
       | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    |



#Visit status is updated from In Progress to Not Started 
@MockStudyBuilder 
@ID:4848
#12
Scenario: Verify that the Visit Date and Visit Activation Date is cleared and greyed out when visit status is updated from "In Progress" to "Not Started" and appropriate message is displayed. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
        | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |           |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Not Started" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status back to "Not Started" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And the following data is displayed in the existing question correction fields
         | Label                  | Current Value         | Requested value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | In Progress           | Not Started               |              | dropdown   |           |
         | Visit Date:            | (Current Date score)  | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score)  | Please provide a response |              | datepicker | false     |


#Visit Status updated from In Progress and Complete
@MockStudyBuilder
@ID:4848
#13
Scenario: Verify that Visit Date is Required Field and Visit Activation date is not a mandatory field when visit status is updated from "In Progress" to "Complete" and appropriate message is displayed.
Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown 
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | In Progress          | Complete        |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |



#Visit Status Updated from In Progress to Partial 
@ID:4848 @MockStudyBuilder
#14
Scenario: Verify that Visit Date is Required Field when visit status is updated from "In Progress" to "Partial"
    Given "Treatment Visit" for "patient 1" is in "In Progress" status
    And I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "In Progress" from "Please select a subject visit" dropdown    
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
            | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |           |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Partial" from existing "Visit Status:" "dropdown"
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | In Progress          | Partial         |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |


   
#Visit Status updated from In Progress to Missed 
@ID:4848 @MockStudyBuilder
#15
Scenario: Verify that visit date and visit activation date field is cleared and grayed out and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "In Progress" from "Please select a subject visit" dropdown    
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Missed" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Missed" upon successful device sync, the visit will no longer be available for completion." is displayed
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | In Progress          | Missed                    |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | false     |

@ID:4848 @MockStudyBuilder
#16
Scenario: Verify that Missed status is added in the visit selection dropdown 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    Then I select the given "Missed" from "Please select a subject visit" dropdown
    
@ID:4848 @MockStudyBuilder
#17
Scenario: Verify that “Not Started” status is added to the  questionnaires to the visit dropdown.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    Then I select the given "Not Started" from "Please select a subject visit" dropdown

#Visit Status is updated from ‘Not Started’ to ‘Complete’
@ID:4848 @MockStudyBuilder
#18
Scenario: Verify that Visit Date is required Field and visit activation date field is editable but not manadatory field  when visit status is updated from "Not Started" to "Complete" and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Not Started" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Not Started          | Complete        |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |

#Visit Status updated from Not Started to In Progress
@ID:4848 @MockStudyBuilder
#19
Scenario: Verify that Visit Date is required Field and visit activation date is editable but not mandatory field when visit status is updated from "Not Started" to "In Progress" and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Not Started" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Not Started          | Complete        |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |

#Visit Status updated from Not Started to Partial
@ID:4848 @MockStudyBuilder
#20
Scenario: Verify that Visit Date is Required Field and Visit Activation date is editable but not mandatory field when visit status is updated from "Not Started" to "Partial".
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Not Started" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Partial" from existing "Visit Status:" "dropdown"
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Not Started          | Partial         |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |

#Visit status updated from Not Started to Missed 
@ID:4848 @MockStudyBuilder
#21
Scenario: Verify that Visit Date and Visit Activation date is cleared and grayed out when visit status is updated from "Not Started" to "Missed" and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Not Started" from "Please select a subject visit" dropdown    
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Not Started          | Please provide a response |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Missed" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Missed" upon successful device sync, the visit will no longer be available for completion." is displayed
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Not Started          | Missed                    |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | false     |


#Visit status updated - Missed to Not Started 
@ID:4848 @MockStudyBuilder
#22
Scenario: Verify that visit date and visit activation date is grayed out when visit status is updated from Missed to Not Started and appropriate message will be displayed.
Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Missed" from "Please select a subject visit" dropdown    
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Missed               | Please provide a response |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    When I select "Not Started" from existing "Visit Status:" "dropdown"             
    Then the DCF Visit Status message "By changing the visit status back to "Not Started" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And the following data is displayed in the existing question correction fields 
         | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
         | Visit Status:          | Missed               | Not Started               |              | dropdown   |           |
         | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | false     |
         | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | false     |


#Visit Status - Missed to Complete
@ID:4848 @MockStudyBuilder
#23
Scenario: Verify that Visit Date is Required Field and visit activation field is editable but not mandatory field when visit status is updated from "Missed" to "Complete" and appropriate message is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Missed" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  | IsEnabled |
            | Visit Status:          | Missed               | Please provide a response |              | dropdown   |           |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker | true      |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker | true      |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Missed               | Complete        |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |


#Visit Status updated from "Missed to In Progress"
@ID:4848 @MockStudyBuilder
#24
Scenario: Verify that Visit Date is Required Field and visit activation date is editable but not mandatory field  when visit status is updated from "Missed" to "In Progress" and appropriate message is displayed.  
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Missed" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | Missed               | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "In Progress" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status back to "In Progress" upon successful device sync, any questionnaires that were missed will become available, any questionnaires fully completed will not be reopened. Please contact YPrime Customer Support should you wish to reopen an already completed questionnaire." is displayed
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Missed               | In Progress     |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |

#Visit status updated from Missed to Partial 
@ID:4848 @MockStudyBuilder
#25
Scenario: Verify that Visit Date is Required Field and visit activation date is editable but not mandatory field when visit status is updated from "Missed" to "Partial".
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Missed" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
            | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
            | Visit Status:          | Missed               | Please provide a response |              | dropdown   |
            | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
            | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Partial" from existing "Visit Status:" "dropdown"
    And I enter "Change Visit Status" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    Then the validation message "Visit Date: field is required" is displayed
    When I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I click on "Next" button
    Then the validation message is not displayed
    When I click on "Back" button
    When I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value | Remove Value | Fieldtype  | DateTimeFormat | IsRemoveEnabled |
        | Visit Status:          | Missed               | Partial         |              | dropdown   |                |                 |
        | Visit Date:            | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | false           |
        | Visit Activation Date: | (Current Date score) | (Current Date)  |              | datepicker | dd-MMM-yyyy    | true            |

@ID:4848 @MockStudyBuilder
#Only Site forms completed 
#26
Scenario: Data persisted when user clicks back and next on data correction page.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed    
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value   | Remove Value | Fieldtype  | DateTimeFormat |
        | Visit Status:          | In Progress          | Complete          |              | dropdown   |                |
        | Visit Date:            | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
        | Visit Activation Date: | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
    And I enter "Visit Status updated" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label                 | Current Value        | Requested Value   | Fieldtype | DateTimeFormat |
        | Visit Status          | In Progress          | Complete          | text      |                |
        | Visit Date            | (Current Date score) | (Current Date)    | text      | dd-MMM-yyyy    |
        | Visit Activation Date | (Current Date score) | (Current Date)    | text      | dd-MMM-yyyy    |
    When I click on "Back" button
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value   | Remove Value | Fieldtype  | DateTimeFormat |
        | Visit Status:          | In Progress          | Complete          |              | dropdown   |                |
        | Visit Date:            | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
        | Visit Activation Date: | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
    And "Visit Status updated" is displayed in "Reason For Correction" inputtextbox field


@ignore
@ID:6711
Scenario: Verify that when the user selects the following "Needs more information," validation message is displayed, and verify that when approval is selected, DCF is submitted. When the decline is selected, DCF is not submitted. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-024" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "In Progress" from "Please select a subject visit" dropdown
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | In Progress          | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score) | Please provide a response |              | datepicker |
        | Visit Activation Date: | (Current Date score) | Please provide a response |              | datepicker |
    And I click on "dropdown" in data correction field for "Visit Status:" existing question
    And the following choices are displayed in dropdown for "Visit status" existing question
        | Choices     |
        | Not Started |
        | In Progress |
        | Partial     |
        | Complete    |
        | Missed      |
    And I select "Complete" from existing "Visit Status:" "dropdown"
    Then the DCF Visit Status message "By changing the visit status to "Complete" upon successful device sync, the visit will no longer be available for completion. Please note, a visit activation date and visit date will be required to be provided in order to complete the requested status change." is displayed    
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value        | Requested Value   | Remove Value | Fieldtype  | DateTimeFormat |
        | Visit Status:          | In Progress          | Complete          |              | dropdown   |                |
        | Visit Date:            | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
        | Visit Activation Date: | (Current Date score) | (Current Date)    |              | datepicker | dd-MMM-yyyy    |
    And I enter "Visit Status updated" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Submitting  |
    And I click "Decline" button in the popup
    And I am on "Submit Data Correction" page
    And I click on "Submit" button
    And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Submitting  |        
    And I click on "Approve" button in the popup
    And I am on "Approved Correction" page
    And a pop up is displayed with "Success" message : "Correction has been made successfully"
    And I click "Ok" button
    And I am on "Approved Correction" page
    And I click on "At a Glance" link on the top navigation bar
    And I click on "Change subject visit" for Subject "S-10001-002"
    And I am on "Approve/Deny Data Correction" page
    And I click on "Needs More Information" button
    And I enter "DateTime Time" in "Reason For Correction" inputtextbox field
    And I click on "Submit" button
    And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType                      |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Requiring additional information |   
    And I click "Decline" button in the popup
    And I am on "Approve/Deny Data Correction" page
    And I click on "Needs More Information" button
    And a pop up is displayed
         | popupType  | Message                                                                                       | ActionButtons    | MessageType                      |
         | Validation | I, [First Name] [Last Name], certify that I am [MessageType] [DCF #] on [dd-MMM-yyyy hh:mm tt] | Approve, Decline | Requiring additional information |   
    When I click on "Approve" button in the popup
    Then I am on "Approved Correction" page
    And a pop up is displayed with "Success" message : "Correction has been made successfully"
      









  