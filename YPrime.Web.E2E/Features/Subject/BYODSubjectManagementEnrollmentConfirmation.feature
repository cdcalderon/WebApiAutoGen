@SubjectManagement
Feature: BYODSubjectManagementEnrollmentConfirmation
	
 Background: 
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
        And Language "German" is assigned to "Initial Site"
        And Language "English" is assigned to "Initial Site"
        And User "PortalE2EUser" with role "YP" has access to site "10001"
        And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"


@MockStudyBuilder
#1 
#Submgmtbyod.01
#BYOD setting is enabled within the study configuration
#“Can create patient bring your own device code” permission is Enabled
Scenario Outline: Verify visibility of BYOD Enrollment Information button
      Given I have a study with BYOD as "<Enable BYOD Function>"
        And "CAN CREATE A SUBJECT BRING YOUR OWN DEVICE CODE." permission is "<CAN CREATE BYOD Permission>"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Management" page
        When I select Subject "S-10001-004"
        Then I am on Subject "S-10001-004" page
        And "BYOD Enrollment Information" button is "<Visibility>"

     Examples: 
    | Enable BYOD Function       | CAN CREATE BYOD Permission     |  Visibility     |
    | Enabled                    | Enabled                        |  Visible        |
    | Enabled                    | Disabled                       |  Not Visible   |
    | Disabled                   | Enabled                        |  Not Visible    |
    | Disabled                   | Disabled                       |  Not Visible    |

@MockStudyBuilder
#2 
#Submgmtbyod.02
#BYOD setting is enabled within the study configuration
#“Can create patient bring your own device code” permission is Enabled
#if user was already enrolled as a BYOD subject, On Click BYOD Enrollment Information button,the original enrollment ID will be displayed within the Welcome page.   
Scenario: Verify visibility of BYOD Enrollment Information button and Enrollment ID
      Given I have a study with BYOD as "Enabled"
        And "CAN CREATE A SUBJECT BRING YOUR OWN DEVICE CODE." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label            | Value        | FieldType    |
            | Subject Number   | 125          | Numberinput  |
            | Gender           | Female       | Radio Button |
            | Date of Birth    | CurrentDate  | datepicker   |
            | Weight           | 100          | Numberinput  |
            | Height           | 100          | Numberinput  |
        And I click on "Yes, subject will use their personal device" button
        And I click on "Language" dropdown
        And I select "English (United States)" from "Language" dropdown
        And I click on "Create" button
        And I am on "Enrollment Information" page
        And "Enrollment ID" value is not empty
        And I click on "X" icon on the "Enrollment Information" page
        And I am on "Subject Management" page
        When I select Subject "S-10001-125"
        Then I am on Subject "S-10001-125" page
        And "BYOD Enrollment Information" button is displayed
        And I click on "BYOD Enrollment Information" button
        And I am on "Enrollment Information" page
        And "Enrollment ID" value is same to the id present at the time of creation 

@MockStudyBuilder
#3 
#Submgmtbyod.03
#SubEnrollCon.07
#BYOD setting is enabled within the study configuration
#“Can create patient bring your own device code” permission is Enabled
Scenario: Verify Subjects can be enrolled to BYOD by using “BYOD Enrollment Information” button and Enrollement Letter with Translated Display
        
        Given I have a study with BYOD as "Enabled"
        And "CAN CREATE A SUBJECT BRING YOUR OWN DEVICE CODE." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And Id "ConfirmationEmailTitle" and languageId "de-Germany" is set with localText "German Translated Welcome to" in "TranslationEndpoint" configuration
        And Id "ConfirmationEmailStudyInformationHeader" and languageId "de-Germany" is set with localText "German Translated Study Information" in "TranslationEndpoint" configuration
        And Id "ConfirmationEmailYourInformationHeader" and languageId "de-Germany" is set with localText "German Translated Your Information" in "TranslationEndpoint" configuration
        And Id "ConfirmationEmailGettingStartedHeader" and languageId "de-Germany" is set with localText "German Translated Getting Started" in "TranslationEndpoint" configuration
        And I click on "All Sites" dropdown
        And I select "Initial Site" from "All Sites" dropdown
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I enter the following data
            | Label                                       | Value                                    | FieldType    |
            | Subject Number                              | 126                                      | Numberinput  |
            | Gender                                      | Female                                   | Radio Button |
            | Date of Birth                               | CurrentDate                              | datepicker   |
            | Weight                                      | 100                                      | Numberinput  |
            | Height                                      | 100                                      | Numberinput  |
        And I click on "No, subject will use a provisioned device" button
        And I click on "Language" dropdown
        And I select "German (Germany)" from "Language" dropdown
        And I click on "Create" button
        And "Success" popup is displayed with message "Subject S-10001-126 has been added successfully with the default PIN of 123456."
        And I click "Ok" button in the popup
        And I am on "Subject Management" page
        And I select Subject "S-10001-126"
        And I am on Subject "S-10001-126" page
        And "BYOD Enrollment Information" button is displayed
        And I click on "BYOD Enrollment Information" button
        When I am on "Enrollment Information" page
        Then "Enrollment ID" value is not empty
        And "German Translated Welcome to" is displayed in the page
        And "German Translated Study Information" is displayed in the page
        And "German Translated Your Information" is displayed in the page
        And "German Translated Getting Started" is displayed in the page

@MockStudyBuilder
#4 
#Submgmtbyod.05
#BYOD setting is enabled within the study configuration
#“Can create patient bring your own device code” permission is Enabled
Scenario: Verify ‘BYOD Enrollment Information’ button will be disabled if the subject is in inactive status
      Given I have a study with BYOD as "Enabled"
        And "CAN CREATE A SUBJECT BRING YOUR OWN DEVICE CODE." permission is "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I select Subject "S-10001-004"
        And I am on Subject "S-10001-004" page
        And I click on "Change Subject Status" button
        And I click on "Patient Status" dropdown
        And I select "Screen Failed" from "Patient Status" dropdown
        And "Screen Failed" is displayed for "Patient Status" dropdown
        And I click on "Save Patient Status" button
        And "Success" popup is displayed with message "Subject S-10001-004 has been updated successfully."
        And I click "Ok" button in the popup
        And "BYOD Enrollment Information" button is not enabled
        And I hover on "BYOD Enrollment Information" button and "Subject status is inactive, this function is disabled" message is displayed
        And I click on "BYOD Enrollment Information" link
        Then I am on "Subject Management" page