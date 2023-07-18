@Portal


Feature: Change subject Visit DCF Visit Activation Date

 Background:
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
    And I have logged in with user "PortalE2EUser"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And Patient "patient 1" with patient number "S-10001-023" is associated with "Initial Site"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And "CAN VIEW DATA CORRECTIONS" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage DCF's" button
    And I am on "Data Correction" page
    And I click on "Add New DCF" button


@ID:5420 @ID:5791 @ID:44 @MockStudyBuilder
#1
Scenario: Verify that when user clicks on the date field a date picker is displayed.
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown
    And the following data is displayed in the attribute data visit field
        | Label                  | Current Value                    | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                      | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
    And I enter current day for "Visit Activation Date:" "PatientDate_2" DCF

@ID:5420 @ID:5791 @ID:44 @MockStudyBuilder
#2
Scenario: Verify that when user clicks on Remove Value toggle it removes the current value if applicable. 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown
    And the following data is displayed in the attribute data visit field
        | Label                  | Current Value                    | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                      | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
    When toggle for "toggle-remove-2" is "Enabled" in DCF
    Then Current Value for "Visit Activation Date:" in index "1" has a strikethrough 

@ID:5420 @ID:5791 @ID:44 @MockStudyBuilder
#3
Scenario: Verify that the user can't enter the future date and enter date manually in the date fields 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown 
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value                    | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                      | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
    And I select "(Current date) +1" from existing "Visit Date:" "datepicker"
    And I select "(Current date) +1" from existing "Visit Activation Date:" "datepicker"
    And the following data is displayed in the existing question correction fields 
        | Label                  | Current Value                    | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                      | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
    And I enter current day for "Visit Date:" "PatientDate_1" DCF
    And I enter current day for "Visit Activation Date:" "PatientDate_2" DCF
    Then the following data is displayed in the existing question correction fields 
        | Label                  | Current Value                    | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                      | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)   | Please provide a response | toggle       | datepicker |
        
    

@ID:5420 @ID:5791 @ID:44 @MockStudyBuilder
@Ignore
#this is something that we can not automated so this will a mannual test
#4
Scenario: Verify that X is not displayed if no date is selected and X is displayed if date is selected in the requested value field and X clears value when clicked on
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And "Next" button is disabled
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown  
    And the following data is displayed in the existing question correction fields 
    | Label                  | Current Value  | Requested Value           | Fieldtype  |
    | Visit Status:          | Not Started    | Please provide a response | dropdown   |
    | Visit Date:            | (Current Date score) | Please provide a response | datepicker |
    | Visit Activation Date: | (Current Date score) | Please provide a response | datepicker | 
    And X icon is not displayed for DCF datepicker "Visit Date:"
    And X icon is not displayed for DCF datepicker "Visit Activation Date:"
    And I select "(Current date)" from existing "Visit Date:" "datepicker"
    And I select "(Current date)" from existing "Visit Activation Date:" "datepicker"
    And the following data is displayed in the existing question correction fields 
    | Label                  | Current Value  | Requested Value           | Fieldtype  | DateTimeFormat |
    | Visit Status:          | Not Started    | Please provide a response | dropdown   |                |
    | Visit Date:            | (Current Date score) | (Current Date)            | datepicker | dd-MMM-yyyy    |
    | Visit Activation Date: | (Current Date score) | (Current Date)            | datepicker | dd-MMM-yyyy    |
    And X icon is displayed for DCF datepicker "Visit Date:"
    And X icon is displayed for DCF datepicker "Visit Activation Date:"
    When I click on X icon for DCF datepicker "Visit Date:"
    And I click on X icon for DCF datepicker "Visit Activation Date:"
    Then the following data is displayed in the existing question correction fields 
    | Label                  | Current Value  | Requested Value           | Fieldtype  |
    | Visit Status:          | Not Started    | Please provide a response | dropdown   |
    | Visit Date:            | (Current Date score) | Please provide a response | datepicker |
    | Visit Activation Date: | (Current Date score) | Please provide a response | datepicker |
    
@ID:5420 @ID:5791 @ID:44 @MockStudyBuilder
#5
Scenario: Data is persisted when user clicks next and back on data correction page 
    Given I am on "Create Data Correction" page
    And I click on "Site Name" dropdown
    And I select "Initial Site" from "Site Name" dropdown
    And I click on "Subject" dropdown
    And I select "S-10001-023" from "Subject" dropdown
    And I click on "Type Of Correction" dropdown
    And I select "Change subject Visit" from "Type Of Correction" dropdown
    And I click on "Please select a subject visit" dropdown
    And I select the given "Screening Visit" from "Please select a subject visit" dropdown
    And the following data is displayed in the attribute data visit field
        | Label                  | Current Value   | Requested Value           | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started     | Please provide a response |              | dropdown   |
        | Visit Date:            | (Current Date)  | Please provide a response | toggle       | datepicker |
        | Visit Activation Date: | (Current Date)  | Please provide a response | toggle       | datepicker |
    And I enter current day for "Visit Activation Date:" "PatientDate_2" DCF
    And I enter "Updated Visit Activation Date" in "Reason For Correction" inputtextbox field
    And I click on "Next" button
    And I am on "Submit Data Correction" page
    And the following data is displayed in the correction data field table
        | Label                  | Current Value                     | Requested Value                 | Fieldtype |
        | Visit Status           | Not Started                       |                                 | text      |
        | Visit Date             | (Current Date score sumarized)    |                                 | text      |
        | Visit Activation Date  | (Current Date score sumarized)    | (Current Date score sumarized)  | text      |
    And I click on "Back" button
    When I am on "Create Data Correction" page
    Then the following data is displayed in the existing question correction fields
        | Label                  | Current Value                   | Requested Value                          | Remove Value | Fieldtype  |
        | Visit Status:          | Not Started                     | Please provide a response                |              | dropdown   |
        | Visit Date:            | (Current Date score sumarized)  | Please provide a response                | toggle       | datepicker |
        | Visit Activation Date: | (Current Date score sumarized)  | (Current Date score sumarized)           | toggle       | datepicker |
    And "Updated Visit Activation Date" is displayed in "Reason For Correction" inputtextbox field