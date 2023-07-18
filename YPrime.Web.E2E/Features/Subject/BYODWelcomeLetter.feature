@BYODWelcomeLetter
Feature: BYODWelcomeLetter

Verification of Welcome Letter

#NLA-154
Background: 
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Language "German" is assigned to "Initial Site"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And I have a study with BYOD as "Enabled"
    # read StudySettingCustomEndpoint.json
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And Key "StudySponsor" is set with value "YPrime" in "StudySettingStudyCustomEndpoint" configuration
    And Key "Protocol" is set with value "eCOA-E2E-Mock" in "StudySettingStudyCustomEndpoint" configuration
    And I click on "All Sites" dropdown
    And I select "Initial Site" from "All Sites" dropdown
    And I click on "Add New Subject" button
    
#1
#NLA-154
#SubEnrollCon.01, 02, 03, 04, 05, 06
@MockStudyBuilder
Scenario: Verify welcome letter when BYOD is enabled
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
    When I am on "Enrollment Information" page
    Then "Add New Subject" button in the background is "Not Visible" and greyed out
    And "x" cross is displayed in right corner of page
    And "Welcome to" is displayed in the page
    And "YPrime YPrime_eCOA-E2E-Mock" is displayed in the page
    And "Study Information" is displayed in the page
    Then following data is displayed in "Study Information" section
        | Label              | Value               |
        | Sponsor:           | YPrime              |
        | Study Name:        | YPrime_eCOA-E2E-Mock|
        | Protocol:          | eCOA-E2E-Mock       |
        | Study Description: | Study Description:  |
    And "Your Information" text is displayed
    And "Enrollment ID:" value is not empty
    Then following data is displayed in "Your Information" section
        | Label          | Value                |
        | Enrollment ID: | <NotNull>            |
        | Site ID:       | 10001 (Initial Site) |
        | Patient ID:    | S-10001-123          |  
    And "Default PIN Code: 123456" text is displayed
    Then I click on "Email icon" button
    And "Email Preference" popup is displayed for email
    And I click on "Cancel" icon on the "Email Preference" popup
    And "Print icon" button is displayed

#2
#NLA-154
#SubEnrollCon.07
@MockStudyBuilder
Scenario: Verify Enrollement Letter with Translated Display
    # read TranslationEndpoint.json
    Given Id "ConfirmationEmailTitle" and languageId "de-Germany" is set with localText "German Translated Welcome to" in "TranslationEndpoint" configuration
    And Id "ConfirmationEmailStudyInformationHeader" and languageId "de-Germany" is set with localText "German Translated Study Information" in "TranslationEndpoint" configuration
    And Id "ConfirmationEmailYourInformationHeader" and languageId "de-Germany" is set with localText "German Translated Your Information" in "TranslationEndpoint" configuration
    And Id "ConfirmationEmailGettingStartedHeader" and languageId "de-Germany" is set with localText "German Translated Getting Started" in "TranslationEndpoint" configuration
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
    When I am on "Enrollment Information" page
    Then "German Translated Welcome to" is displayed in the page
    And "German Translated Study Information" is displayed in the page
    And "German Translated Your Information" is displayed in the page
    And "German Translated Getting Started" is displayed in the page