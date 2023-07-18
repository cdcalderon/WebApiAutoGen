@SubjectManagement
Feature: SubjectManagement
	

 Background: 
    Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
        And User "PortalE2EUser" with role "YP" has access to site "100000"
        And Patient "patient 1" with patient number "S-100000-124" is associated with "Site 1"
        
        
@MockStudyBuilder
#1 
#SDV.02-a
#SDV.03
#Subject details will  be displayed if "CAN VIEW PATIENT DETAILS" permission is Enabled
Scenario: Verify visibility of subject details in a BYOD configured study when permission to view subject details is enabled.
      Given I have a study with BYOD as "Enabled"
        And "CAN VIEW SUBJECT DETAILS." permission is "Enabled"
        And "ABILITY TO EDIT THE DETAILS OF A SUBJECT." permission is "Enabled"
        And "CAN RESET THE PIN FOR A SUBJECT." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                                       | Value                                       | FieldType    |
            | Subject Number                              | 125                                         | Numberinput  |
            | Gender                                      | Female                                      | Radio Button |
            | Date of Birth                               | CurrentDate                                 | datepicker   |
            | Weight                                      | 100.00                                      | Numberinput  |
            | Height                                      | 100.00                                      | Numberinput  |
        And I click on "Yes, subject will use their personal device" button
        And I click on "Create" button
        And I click on "X" icon on the "Confirmation" popup
        And I am on "Subject Management" page
        When I select Subject "S-10001-125"
        Then I am on Subject "S-10001-125" page
        And the following button is displayed on the page
              | Value                       |
              | Data Correction             |
              | Change Subject Status       |
              | Reset PIN                   |
              | BYOD Enrollment Information |
         And The following data is displayed in the subject grid
            | Label         | Value                        |
            | Gender        | Female                       |
            | Date of Birth | CurrentDate                  |
            | Weight        | 100.00                       |
            | Height        | 100.00                       |
            | Language      | English (United States)      |
            
            
@MockStudyBuilder
#2
#SDV.02-a
#SDV.03
#Subject details will not be displayed if "CAN VIEW PATIENT DETAILS" permission is disabled
Scenario: Verify visibility of subject details in a BYOD configured study when permission to view subject details is disabled.
       Given I have a study with BYOD as "Enabled"
        And "CAN VIEW SUBJECT DETAILS." permission is "Disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                                       | Value                                       | FieldType    |
            | Subject Number                              | 126                                         | Numberinput  |
            | Gender                                      | Female                                      | Radio Button |
            | Date of Birth                               | CurrentDate                                 | datepicker   |
            | Weight                                      | 100.00                                      | Numberinput  |
            | Height                                      | 100.00                                      | Numberinput  |
        And I click on "Yes, subject will use their personal device" button
        And I click on "Create" button
        And I click on "X" icon on the "Confirmation" popup
        And I am on "Subject Management" page
        When I select Subject "S-10001-126"
        Then I am on Subject "S-10001-126" page
        And "Subject Attributes" is not "Visible"
       
                           
@MockStudyBuilder
#3
#Subject is successfully created and Data correction is asscoiated for 1 of its attribute
#SDV.02-b,c
Scenario: Any subject attribute having data corrections associated with it has a column named “corrections” displayed with the number of corrections 
     Given "CAN VIEW SUBJECT DETAILS." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                                       | Value                                       | FieldType    |
            | Subject Number                              | 127                                         | Numberinput  |
            | Gender                                      | Female                                      | Radio Button |
            | Date of Birth                               | CurrentDate                                 | datepicker   |
            | Weight                                      | 100.00                                      | Numberinput  |
            | Height                                      | 100.00                                      | Numberinput  |
        And I click on "Yes, subject will use their personal device" button
        And I click on "Create" button
        And I click on "X" icon on the "Confirmation" popup
        And I am on "Subject Management" page
        And I create a Data Correction for "Height" associated with Patient "S-10001-127"
        When I select Subject "S-10001-127"
        Then I am on Subject "S-10001-127" page
        And the following text is displayed on the page
               | Value       |
               | Corrections |
        And value associated with "Height" is "1"
        And I click on "Oval" button
        And "Data Correction" link is displayed
        And I click on link "data correction number" for "Height"
        And I am on "Data Correction Confirmation" page

 @MockStudyBuilder
#4
#SDV.02-d,e
# Validating training status for handheld and tablet both
 Scenario: Verify training non completion and completion status for handheld and tablet both
      Given I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        When I select Subject "S-100000-124"
        Then I am on Subject "S-100000-124" page
        And status of "Handheld training status" is "Red" X
        And status of "Tablet training status" is "Red" X
        And I completed "Handheld training" for "Subject"
        And I completed "Tablet training" for "Subject"
        And I sync the data
        And status of "Handheld training status" is "Green" Tick
        And status of "Tablet training status" is "Green" Tick


@MockStudyBuilder
#5
#SDV.05
#SDV.02-f
#Validating the staus and compliance of subject when initially navigate to subject management change.
#VAlidating changed status and compliance after successfully updating both.
#Clicking cancel will not update the current subject status.
 Scenario: Verify that on clicking "Change Subject Status" button and clicking save button after selecting new status from dropdown will update the status of subject
      Given "ABILITY TO EDIT THE DETAILS OF A SUBJECT." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-100000-124"
        And I am on Subject "S-100000-124" page
        And Status "Status" of subject "S-100000-124" is "SCREENED"
        And Status "Compliance status" of subject "S-100000-124" is "Non-compliant"
        And I click on "Change Subject Status" button in Subject Management page
        And "Patient Status" dropdown is "Visible"
        And I click on "Patient Status" dropdown
        And I select "Screen Failed" from "Patient Status" dropdown
        And I click on "Cancel Patient Status" button
        And Status "Status" of subject "S-100000-124" is "SCREENED"
        And I click on "Change Subject Status" button in Subject Management page
        And "Patient Status" dropdown is "Visible"
        And I click on "Patient Status" dropdown
        And I select "Screen Failed" from "Patient Status" dropdown
        When I click on "Save Patient Status" button
        Then "Success" popup is displayed with message "Subject S-100000-124 has been updated successfully."
        And I click "Ok" button in the popup
        And "Status" of subject "S-100000-124" is updated to "SCREEN FAILED"
        And Status "Compliance status" of subject "S-100000-124" is "Compliant"


@MockStudyBuilder
#6
#SDV.04
Scenario: Buttons will not displayed if permission is disabled
       Given "CAN RESET THE PIN FOR A SUBJECT." permission is "disabled"
       And "ABILITY TO EDIT THE DETAILS OF A SUBJECT." permission is "disabled"
        And "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        When I select Subject "S-100000-124"
        Then I am on Subject "S-100000-124" page
        And "Reset PIN" button is not "Visible"
        And "Change Subject Status" button is not "Visible"
        And "Email Web Backup URL (Subject Handheld)" button is not "Visible"

@MockStudyBuilder
#7 
#SDV.04
Scenario Outline: Verify that PIN reset message will appear with "OK" button when user clicks on Update Pin after clicking on Reset Pin and clicking "OK" closes the pop-up.
    Given I have a study configured using pin length of "<Pin Length>"
        And "CAN RESET THE PIN FOR A SUBJECT." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        And I select Subject "S-100000-124"
        And I am on Subject "S-100000-124" page
        And "Reset PIN" button is displayed
        When I click on "Reset PIN" button
        Then Message "<Message>" is displayed below "Subject" grid with "<ActionButtons>"
        And I click on "Update Pin" button
        And "UpdatePin" pop up is displayed with message "<UpdatePinMessage>" and buttons "<Ok>"
        And I click "Ok" button in the popup
        And popup is dismissed
        And Message "<Message>" is not displayed below "Subject" grid
    
    Examples: 
    | Pin Length | Message                          | ActionButtons      | UpdatePinMessage                   | Ok |
    | 4          | RESET PIN NUMBER TO 1234?        | Update Pin, Cancel | Temporary PIN has been reset.      | Ok |
    | 6          | RESET PIN NUMBER TO 123456?      | Update Pin, Cancel | Temporary PIN has been reset.      | Ok |  

    
@MockStudyBuilder
#8
#SDV.04
Scenario Outline: Verify that PIN will not be reset when user clicks "Cancel" button after clicking on Reset Pin .
     Given I have a study configured using pin length of "<Pin Length>"
         And "CAN RESET THE PIN FOR A SUBJECT." permission is "Enabled"
         And I am logged in as "PortalE2EUser"
         And I am on "At a Glance" page
         And I click on "Subject" link on the top navigation bar
         And I am on "Subject Management" page
         And I select Subject "S-100000-124"
         And I am on Subject "S-100000-124" page
         And "Reset PIN" button is displayed
         When I click on "Reset PIN" button
         Then Message "<Message>" is displayed below "Subject" grid with "<ActionButtons>"
         And I click on "Cancel" button
         And Message "<Message>" is not displayed below "Subject" grid

            Examples: 
            | Pin Length | Message                                        | ActionButtons      |
            | 4          | RESET PIN NUMBER TO 1234?                      | Update Pin, Cancel | 
            | 6          | RESET PIN NUMBER TO 123456?                    | Update Pin, Cancel |
     