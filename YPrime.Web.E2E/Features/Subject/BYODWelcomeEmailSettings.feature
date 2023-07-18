@BYODWelcomeEmailSettings
Feature: BYOD Welcome Email Settings

BYOD Welcome Email Settings

#NLA-156
Background: 
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
      And Language "German" is assigned to "Initial Site"
      And User "PortalE2EUser" with role "YP" has access to site "10001"
      And I have a study with BYOD as "Enabled"
      And I am logged in as "PortalE2EUser"
      And I am on "At a Glance" page
      And I click on "Subject" link on top navigation bar
      And I am on "Subject Information" page
      And I click on "All Sites" dropdown
      And I select "Initial Site" from "All Sites" dropdown
      And I click on "Add New Subject" button
    
#1
#NLA-156
#BYODEMAIL.01,02,08, SET.03
#Verify Welcome Email confirmation button is present & all BYOD enrollment information is present
#Verifying all Emails sent will be displayed in View Emails section and no email address is saved in database
@MockStudyBuilder
Scenario: Verify all BYOD enrollment information is present and all Emails sent will be displayed in View Emails section
    Given I am on "Add New Subject" page
      And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 123                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
      And I click on "Yes, subject will use their personal device" button
      And I click on "Language" dropdown
      And I select "English(United States)" from "Language" dropdown
      And I click on "Create" button
      And I am on "Enrollment Information" page
      And "Email Confirmation" button is displayed
     When I click on "Email Confirmation" button
     Then "Send BYOD Confirmation" is displayed in the page
      And "" is displayed in "Email textbox" inputtextbox field
      And "YPrime S-10001-123 BYOD Enrollment Information " is displayed in "Subject textbox" inputtextbox field
      And "To access your BYOD enrollment information select the attachment" is displayed
      And "PDF Attachment" is displayed
      And I click on "PDF Attachment" button to generate "YPrime S-10001-123 BYOD Enrollment Information " in ".pdf" format file to save in Export Evidence folder
      And I enter "abc@yprime.com" in "Email textbox" inputtextbox field
      And I click on "Send" button
      And "Welcome to" is displayed in the page
      And I refresh page
      And I click on "Manage Study" link on the top navigation bar
      And I click on "Role Management" link
      And I am on "Role Management" page
      And I click "Set Subscriptions" button for row "YPrime"
      And I "Enable" "BYOD Confirmation Email" permission
      And I click on "Manage Study" link on the top navigation bar
     When I click on "View Emails" link
     Then "Saved Emails" text is displayed
      And following data is displayed in "Saved_Emails" Grid
            | Email Subject                                  | Type  | Site         | Date Sent    |
            | YPrime S-10001-123 BYOD Enrollment Information | Email | Initial Site | Current Date |
      And "abc@yprime.com" email addresses is not stored in Data base 

#2
#NLA-156
#BYODEMAIL.05,07,09
#Error message shoubd be displayed if user enter wrong email address
#verify error meaasage displayed in case email failed to sent with valid email address
@MockStudyBuilder
Scenario: Error message shoubd be displayed if user enter wrong email address and in case email failed to sent with valid email address  
    Given I am on "Add New Subject" page
      And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 123                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
      And I click on "Yes, subject will use their personal device" button
      And I click on "Language" dropdown
      And I select "English (United States)" from "Language" dropdown
      And I click on "Create" button
      And I am on "Enrollment Information" page
      And "Email Confirmation" button is displayed
      And I click on "Email Confirmation" button
     When I click on "Send" button
     Then "Email Address is invalid." text is displayed
      And I enter "Random" in "Email textbox" inputtextbox field
     When I click on "Send" button
     Then "Email Address is invalid." text is displayed 
      And I enter "abc@yprime.com, xyz@yprime.com" in "Email textbox" inputtextbox field
     When I click on "Send" button
     Then "Email Address is invalid." text is displayed
      

#3
#NLA-156
#BYODEMAIL.04
#Verify Send BYOD Confirmation with German Translated Display
@MockStudyBuilder
Scenario: Verify Send BYOD Confirmation with German Translated Display
    # read TranslationEndpoint.json
    Given Id "ConfirmationEmailBody" and languageId "de-Germany" is set with localText "German Translated To access your BYOD enrollment information select the attachment" in "TranslationEndpoint" configuration
      And I am on "Add New Subject" page
      And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 123                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
      And I click on "Yes, subject will use their personal device" button
      And I click on "Language" dropdown
      And I select "German (Germany)" from "Language" dropdown
      And I click on "Create" button
      And I am on "Enrollment Information" page
      And "Email Confirmation" button is displayed
     When I click on "Email Confirmation" button
      Then "German Translated To access your BYOD enrollment information select the attachment" is displayed in the page