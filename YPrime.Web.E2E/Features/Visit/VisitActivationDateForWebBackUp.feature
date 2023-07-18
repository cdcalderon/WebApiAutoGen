Feature: Capture Visit Activation Date For WebBackUp
	
Background: 
	Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
    And Patient "patient 1" with patient number "S-10001-004" is associated with "Site 1"
    And "Tablet" Device "YP-E2E-Device" is assigned to Site "Site 1"
    And Subject "S-10001-104" is assigned to "YP-E2E-Device" Device
    And I enable Web Backup until "currentday+5" for the site "100000"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And Diary Entry "Questionnaire Forms" is associated with "patient 1"
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "In general, How would you say your health is:" and choice "Not better"
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Good"
    And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   eCOA App           |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        |  Visit Name                                                               |   Screening Visit    |
    And User "PortalE2EUser" with role "YP" has access to site "100000"
    And "CAN ACTIVATE VISITS IN PORTAL." permission is "Enabled"
	And "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled" 
	And "CAN ACCESS WEB-BACKUP BUTTON (TABLET)" permission is "Enabled" 



@ID:4084
@MockStudyBuilder
#1
Scenario: Verify that visit activation date is populated when the user clicks the web backup button.
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10001-004"
    And I am on Subject "S-10001-004" page
    And I click on "Visits" tab
    And I click on "(Subject Tablet Backup)" partial text link 
    And "HardStopWarningModal" specific popup is displayed
    And I click on "Yes" button
    And "ActivateVisitModal" specific popup is displayed
    And I click on "Ok" button in Activate Visit Modal
    And "SendWebBackUpEmailModal" specific popup is displayed
    And I enter "test123@yprime.com" in Web Backup Email inputtextbox field
    And I click on "Send" button in Web Backup Email Modal
    And popup is dismissed
    And I click on "Visits" tab
    And current date for site number "100000" is displayed 
    

@ID:4084
@MockStudyBuilder
#2
Scenario: Verify that when the user clicks on the “Yes” button on the popup, the visit activation date is populated. However, verify that the visit activation date is not populated when the user clicks on the “No” button on the popup.
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10001-004"
    And I am on Subject "S-10001-004" page
    And I click on "Visits" tab
    And I click on "(Subject Tablet Backup)" partial text link 
    And "HardStopWarningModal" specific popup is displayed
    And I click on "Yes" button
    And "ActivateVisitModal" specific popup is displayed
    And I click on "Ok" button in Activate Visit Modal
    And "SendWebBackUpEmailModal" specific popup is displayed
    And I enter "test123@yprime.com" in Web Backup Email inputtextbox field
    And I click on "Cancel" button in Web Backup Email Modal
    And popup is dismissed
    And current date for site number "100000" is not displayed 
    And I click on "(Subject Tablet Backup)" partial text link
    And "HardStopWarningModal" specific popup is displayed
    And I click on "Yes" button
    And "ActivateVisitModal" specific popup is displayed
    And I click on "Ok" button in Activate Visit Modal
    And "SendWebBackUpEmailModal" specific popup is displayed
    And I enter "test123@yprime.com" in Web Backup Email inputtextbox field
    And I click on "Send" button in Web Backup Email Modal
    And popup is dismissed
    And I click on "Visits" tab
    And current date for site number "100000" is displayed
 
@ID:4084
@MockStudyBuilder
#3
Scenario: Verify that when the visit activation date is populated by clicking Activate Visit button and if the user clicks on the web back up link the visit activation date should not be overwritten. 
    Given I am logged in as "PortalE2EUser"
    And "21-Jan-2022" activation date is set for "Screening Visit" for "patient 1"
    And I am on "Subject Management" page
    And I select Subject "S-10001-004"
    And I am on Subject "S-10001-004" page
    And I click on "Visits" tab
    And "21-Jan-2022" text is displayed in the page
    And I click on "(Subject Tablet Backup)" partial text link
    And "SendWebBackUpEmailModal" specific popup is displayed
    And I enter "test123@yprime.com" in Web Backup Email inputtextbox field
    And I click on "Send" button in Web Backup Email Modal
    And popup is dismissed
    And "21-Jan-2022" text is displayed in the page


