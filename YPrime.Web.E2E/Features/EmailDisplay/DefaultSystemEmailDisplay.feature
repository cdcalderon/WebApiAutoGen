@DefaultSystemEmailDisplay
Feature: DefaultSystemEmailDisplay

Verify the system email and view email table 

NLA-332

Background: 
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Role Management" link
    And I am on "Role Management" page
	And I click "Set Subscriptions" button for row "YPrime"
    And I "Enable" "Confirmation of Bulk Site Activation" permission
    And I "Enable" "Confirmation of Site Management" permission
    And I click on "Sites" link on top navigation bar
    And I click on "10001" link
    And I am now on "Site Details" page
    And I enter "20007" in "Site Number" inputtextbox field
    And I click on "Save Tab"
    And I click on "Save" button
    And I click on "Sites" link on the top navigation bar
    And I click on "Bulk Edit Sites" button
    And I click on "Master Active toggle" togglebutton
    And I click on "Save" button

@MockStudyBuilder
#SET.02,04
#SER.01,02,03
Scenario: Verify View Email grid, Email content and Resend button functionality
	Given I click on "Manage Study" link on top navigation bar
    And I click on "View Emails" link
    When I am on "Saved Emails" page
    Then following data is displayed in "Saved Emails" Grid
        | Email Subject                                                         | Type  | Site         | Date Sent    |  
        | VAL-YPrime eCOA-E2E-Mock Confirmation of Bulk Site Management         | Alert | N/A          | Current Date |
        | VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007 | Email | Initial Site | Current Date |
    And I click on "VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007" link
    And "Email Details" text is displayed in the page
    And the following text is displayed on the "Saved Email Confirmation" page
        | Label     | Value                                                                 |
        | Date Sent | Current Date                                                          |
        | Template  | Confirmation of Site Management                                       |  
        | Subject   | VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007 |
    And Recipients label has value set from "EmailRecipients" table
    And "Confirmation of Site Management" is displayed in the page
    And "YPrime eCOA-E2E-Mock" is displayed in the page
    And following data is displayed in "emailHeader" section
        | Label                    | Value         |
        | User Name:               | PortalE2EUser |
        | System Transaction Date: | Current Date  |  
    And following data is displayed in "emailBody" section
        | Label                   | Value                     |
        | Site Name:              | Initial Site              |
        | Site Number:            | 20007                     |
        | Site Investigator Name: | Investigator Initial Site |
        | Primary Contact:        | ypadmin                   |
        | Shipping Address:       | test Dr.                  |
        | City, State:            | Malvern, PA               |
        | Zip Code:               | 19355                     |
        | Site Country:           | n/a                       |
        | Phone Number:           | 123-456-7890              |
        | Fax Number:             | n/a                       |
        | Time Zone:              | Eastern Standard Time     |
        | Site Active:            | True                      |
    And Emails "VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007" will be sent to recipients utilizing the BCC method  
    And "The site details have been updated successfully." is displayed in the page
    And "YPrime © 2022 - For YPrime support, contact us at +1-(844)-385-2352 or via email at: " is displayed in the page
    And I enter "PortalE2EUser" in "BCC" inputBox field
    And "VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007" has "1" count in "EmailSent" table
    And I click on "Resend" Button 
    And "VAL-YPrime eCOA-E2E-Mock Confirmation of Site Management - Site 20007" has "2" count in "EmailSent" table
