@HandheldEmailWebBackupURL
Feature: HandheldEmailWebBackupURL
#NLA-163
#Validate Handheld Email WebBackup URL

Background:
	Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And Patient "patient 1" with patient number "S-10001-104" is associated with "Initial Site"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
    And I have a study with BYOD as "Disabled"

#1
#REQ-EWBU001.20 ,EWBU001.30
@MockStudyBuilder
Scenario Outline: Verify Email Web Backup URL (Subject Handheld)’ button Visibility
	Given "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "<CanActivateWebBackUp>"
    #Assigned asset tag 
    And Subject "S-10001-104" is assigned to "YP-E2E-Device" Device
	And I update "WebBackupHandheldEnabled" key with value "<Days>" days
	And I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page    
    And I select Subject "S-10001-104"
    When I am on Subject "S-10001-104" page
	Then "Email Web Backup URL (Subject Handheld)" button is "<Visibility>"

Examples:
	| CanActivateWebBackUp | Days | Visibility  |
	| Disabled             | 3    | Not Visible |
	| Disabled             | 0    | Not Visible |
	| Enabled              | 0    | Not Visible |
	| Enabled              | 3    | Visible     |


#2
#REQ-EWBU001.35
@MockStudyBuilder
Scenario: Verify ‘Email Web Backup URL (Subject Handheld)’ button will be disabled if the subject is in inactive status
    Given "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "Enabled"
    #Assigned asset tag 
    And Subject "S-10001-104" is assigned to "YP-E2E-Device" Device
    And I update "WebBackupHandheldEnabled" key with value "3" days
	And I am logged in as "PortalE2EUser"
	And I am on "Subject Management" page    
    And I select Subject "S-10001-104"
    And I am on Subject "S-10001-104" page
    And I click on "Change Subject Status" button
    And I click on "Patient Status" dropdown
    And I select "Screen Failed" from "Patient Status" dropdown
    And "Screen Failed" is displayed for "Patient Status" dropdown
    And I click on "Save Patient Status" button
    And "Success" popup is displayed with message "Subject S-10001-104 has been updated successfully."
    When I click "Ok" button in the popup
    Then "Email Web Backup URL (Subject Handheld)" button is not enabled
    And I hover on "Email Web Backup URL (Subject Handheld)" button and "Subject status is inactive, this function is disabled" message is displayed
   

#3
#REQ-EWBU001.40
@MockStudyBuilder
Scenario: Verify ‘Email Web Backup URL (Subject Handheld)’ button will be disabled if asset tag is not associated to the subject
   Given "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "Enabled"
   And I update "WebBackupHandheldEnabled" key with value "3" days
   And I am logged in as "PortalE2EUser"
   And I am on "Subject Management" page    
   And I select Subject "S-10001-104"
   When I am on Subject "S-10001-104" page
   Then "Email Web Backup URL (Subject Handheld)" button is not enabled
   And "Email Web Backup URL (Subject Handheld)" tooltip displays message as "The subject does not have an asset tag assigned" 


#4
#REQ-EWBU001.50, EWBU001.60,REQ-EWBU001.70, EWBU001.100 
@MockStudyBuilder
Scenario: Verify Web Backup URL email box and error message should be displayed if user enter wrong email address
    Given "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "Enabled"
    #Assigned asset tag 
    And Subject "S-10001-104" is assigned to "YP-E2E-Device" Device
    And I update "WebBackupHandheldEnabled" key with value "3" days
	And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page   
    And I select Subject "S-10001-104"
    And I am on Subject "S-10001-104" page
    And "Email Web Backup URL (Subject Handheld)" button is displayed
    When I click on "Email Web Backup URL (Subject Handheld)" button
    Then "Send Patient Web Backup URL" is displayed in the page  
    When I click on "Send" button in Send Patient Web Backup Email
    Then "Please enter a valid email address." text is displayed
    And I enter "Random" in  Web Backup Patient Email inputtextbox field
    When I click on "Send" button in Send Patient Web Backup Email
    Then "Please enter a valid email address." text is displayed
    And I enter "abc@yprime.com, xyz@yprime.com" in  Web Backup Patient Email inputtextbox field
    When I click on "Send" button in Send Patient Web Backup Email
    Then "Please enter a valid email address." text is displayed
    And I enter "abc@yprime.com" in  Web Backup Patient Email inputtextbox field
    And "YPrime_eCOA-E2E-Mock Web Backup URL" is displayed in Web Backup Subject inputtextbox field
    And "To access Web Backup, click the hyperlink below. This link will expire on currentday+3. Click Here to Access your Study Questionnaires" is displayed in email body
    When I click on "Send" button in Send Patient Web Backup Email
    Then "abc@yprime.com" email addresses is not stored in Data base 
    And  I click on "Email Web Backup URL (Subject Handheld)" button
    When I click on "Click Here to Access your Study Questionnaires" link in new tab 
    Then I navigate to Device emulation page

#5
@MockStudyBuilder
Scenario: Verify Send Patient Web Backup URL with German Translated Display
    # read TranslationEndpoint.json
    Given "CAN ACTIVATE WEB-BACKUP (HANDHELD)" permission is "Enabled"
    And I have a study with BYOD as "Enabled"
    And I update "WebBackupHandheldEnabled" key with value "3" days
	And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page 
    And Id "WebBackupBodyKey" and languageId "de-Germany" is set with localText "German Translated To access Web Backup, click the hyperlink below. This link will expire on <=ExpireDate=>. <a href='<=EncryptedURL=>'>Click Here to Access your Study Questionnaires</a>" in "TranslationEndpoint" configuration
    And Id "WebBackupTitleKey" and languageId "de-Germany" is set with localText "German Translated <=StudyName=> Web Backup URL" in "TranslationEndpoint" configuration
    And I am on "Add New Subject" page
    And I enter the following data
            | Label                 | Value                         | FieldType    |
            | Subject Number        | 125                           | Numberinput  |
            | Gender                | Female                        | Radio Button |
            | Date of Birth         | CurrentDate                   | datepicker   |
            | Weight                | 100                           | Numberinput  |
            | Height                | 100                           | Numberinput  |
    And I click on "Yes, subject will use their personal device" button
    And I click on "Language" dropdown
    And I select "German (Germany)" from "Language" dropdown
    And I click on "Create" button
    And I click on "X" icon on the "Enrollment Information" page
    And I am on "Subject Management" page
    And I select Subject "S-10001-125"
    And I am on Subject "S-10001-125" page
    And "Email Web Backup URL (Subject Handheld)" button is displayed
    When I click on "Email Web Backup URL (Subject Handheld)" button
    Then "German Translated To access Web Backup, click the hyperlink below. This link will expire on currentday+3. Click Here to Access your Study Questionnaires" is displayed in email body
    And "German Translated YPrime_eCOA-E2E-Mock Web Backup URL" is displayed in Web Backup Subject inputtextbox field