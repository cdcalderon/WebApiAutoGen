@StudyUserReport
Feature: StudyUserReport

Verify the Data grid functionality for Study User Report
#NLA-126
#1
#SUR.01, 02, 03, 04, 05, RR.02
@MockStudyBuilder
Scenario: Validate user assigned to roles, grid table, export functionality, inactive status and access grant to study user
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And User "PortalE2EUser" with role "Sub-I" has access to site "10000"
    And User "nsharma@yprime.com" with role "CRA" has access to site "10000"
    # created a Test Role with {shortname:TR}
    And User "ypadmin2@yprime.com" with new role "TR" has access to site "10000"
    And User "ypadmin1@yprime.com" with role "SC" has access to site "10000"
    And User "cherrmann@yprime.com" with role "YM" has access to site "10000"
    And User "rcollier@yprime.com" with role "SP" has access to site "10000"
    And User "apeazzoni@yprime.com" with role "YP" has access to site "10000"
	And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And "Study User (Unblinded)" report visibility status is "Enabled"
	When I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    Then I click on "Study User (Unblinded)" from the Report List
    And I click on Show Entries per grid dropdown
    And I select "25" from Show Entries per grid dropdown
	And following data is displayed in "Study User (Unblinded)" Grid
       | Site  | Username                  | First Name | Last Name | Role  |
       | 10000 | PortalE2EUser             | First      | Last      | Sub-I |
       | 10000 | ypadmin2@yprime.com       | YPadmin2   | Test      | TR    |
       | 10000 | cherrmann@yprime.com      | Craig      | Herrmann  | YM    |
       | 10000 | nsharma@yprime.com        | ypadmin    | YP        | CRA   |
       | 10000 | rcollier@yprime.com       | Randy      | Collier   | SP    |
       | 10000 | ypadmin1@yprime.com       | Ypadmin_01 | test      | SC    |
       | 10000 | apeazzoni@yprime.com      | Aaron      | Peazzoni  | YP    |
       | 10001 | ypmockinactive@yprime.com | Mock       | Inactive  | IV    |
    And User access for "ypadmin2@yprime.com" with role "TR" and site "10000" has been removed from study
    And I click on "Study User (Unblinded)" from the Report List
    And following data is not displayed in "Study User (Unblinded)" Grid
       | Site  | Username             | First Name | Last Name | Role  |
       | 10000 | ypadmin2@yprime.com  | YPadmin2   | Test      | TR    |
    And I click on "Hamburger" button
    And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Study User (Unblinded)" in ".pdf" format file to save in Export Evidence folder

#2 RR.01
#NLA126
@MockStudyBuilder
Scenario: Verify that Study User(Unblinded) report link is not visible when permission is disabled
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    When "Study User (Unblinded)" report visibility status is "Disabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    Then "Study User (Unblinded)" report link is "Not Visible"


@MockStudyBuilder
#3 RR.01, RR.02, RR.03
Scenario: Verify if title on page is correct and search boxes are working fine
      Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
        And User "PortalE2EUser" with role "Sub-I" has access to site "10000"
        And I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Study User (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Study User (Unblinded)" from the Report List
        And "Study User (Unblinded)" report title is displayed on screen
        And I enter "PortalE2EUser" in "Search_Username" textbox field
        And I enter "10000" in "Search_Site" textbox field
	    And "1" records are displayed in "Study User (Unblinded)" data grid
        And following data is displayed in "Study User (Unblinded)" Grid
        | Site  | Username      | First Name | Last Name | Role  |
        | 10000 | PortalE2EUser | First      | Last      | Sub-I |
        When I click on "Hamburger" button
        Then hamburger menu is displayed with the following functionality for export
		| ButtonName	|
		| Excel			|
		| CSV           |
		| PDF           |
		| Print         |
        When I click on "First Name" link
        Then "First Name" column is not visible in "Study User (Unblinded)" grid
        And following data is displayed in "Study User (Unblinded)" Grid
        | Site  | Username      | Last Name | Role  |
        | 10000 | PortalE2EUser | Last      | Sub-I |
      
    