@AnalyticsAndReports
Feature: SiteDetailsReport

@MockStudyBuilder
#1 RR.01
Scenario: If permission is disabled "Site Details (Blinded)" report link will not be displayed
    Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And "Site Details (Blinded)" report visibility status is "Disabled"
     When I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     Then I am on "Reports" page
     And "Site Details (Blinded)" report link is "Not Visible"


@MockStudyBuilder
#2 RR.01, SDR.02, SDR.03
#If permission is enabled "Site Details (Blinded)" will be displayed
#4 RR.02, SDR.04
#Upon export the following file name will be displayed: <Sponsor>_<Protocol>_Site_Details_.XXX
Scenario: Verify site Details Report displays a listing of all sites by country based on user role access and Verify file format after exporting report
     Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
     And Patient "patient 1" with patient number "S-10000-004" is associated with "Site 1"
     And Patient Visit "Screening Visit" is associated with "patient 1"
     And Active history is updated for "10000"
     And Site "Site 2" is assigned to Country "India" and has site number "10002" with "inactive" status
     And User "PortalE2EUser" with role "YP" has access to site "10000"
     And User "PortalE2EUser" with role "YP" has access to site "10002"
     And I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And "Site Details (Blinded)" report visibility status is "Enabled"
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     When I am on "Reports" page
     Then "Site Details (Blinded)" report link is "Visible"
     And I click on "Site Details (Blinded)" from the Report List
     And "Site Details (Blinded)" header is displayed
          And sites to which user has access will be displayed
         | SiteNumber |
         | 10000      |
         | 10002      |
    When I click on "Hamburger Menu" button
	  Then hamburger menu is displayed with the following functionality for export
		| ButtonName |
		| Excel      |
		| CSV        |
		| PDF        |
		| Print      |
    And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Site Details (Blinded)" in ".pdf" format file to save in Export Evidence folder
    And the following columns is displayed within Report index grid
      | Label |
      | Country                  |
      | Site Number              |
      | Investigator Name        |
      | Site Status              |
      | Date of First Screening  |
      | Date of Activation       |
      | Date of Deactivation     |
      | Date of Reactivation     |
     And following data is displayed in "Site Report" Grid
     | Country         | Site Number | Investigator Name       | Site Status  | Date of First Screening | Date of Activation   |   Date of Deactivation | Date of Reactivation | 
     | United States   | 10000      | Investigator Site 1      | Active       |  Current Date           |    Current Date      |    Current Date        | Current Date         |
     | India           | 10002      | Investigator Site 2      | Inactive     |                         |                      |                        |                      |
     

@MockStudyBuilder
#3 SDR-Layout-01
#If permission is enabled "Site Details (Blinded)" will be displayed
#Logged in with "ypadmin1@yprime.com" user
Scenario: Site Details Report displays a listing of all sites by country based on user role access
     Given Site "Site 3" is assigned to Country "India" and has site number "10003"
     And Patient "patient 2" with patient number "S-10003-005" is associated with "Site 3"
     And Patient Visit "Screening Visit" is associated with "patient 2"
     And Active history is updated for "10003"
     And Site "Site 4" is assigned to Country "United States" and has site number "10004" with "inactive" status
     And User "ypadmin1@yprime.com" with role "YP" has access to site "10003"
     And User "ypadmin1@yprime.com" with role "YP" has access to site "10004"
     And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
     And I am on "Subject Management" page
     And "Site Details (Blinded)" report visibility status is "Enabled"
     When I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     Then I am on "Reports" page
     And "Site Details (Blinded)" report link is "Visible"
     And I click on "Site Details (Blinded)" from the Report List
     And "Site Details (Blinded)" header is displayed
     And sites to which user has access will be displayed
         | SiteNumber  |
         | 10003       |
         | 10004       |
     And following data is displayed in "Site Report" Grid
     | Country         | Site Number  | Investigator Name   | Site Status | Date of First Screening | Date of Activation  |   Date of Deactivation   | Date of Reactivation  | 
     | India           | 10003        | Investigator Site 3 | Active      | Current Date            | Current Date        |    Current Date          | Current Date          |
     | United States   | 10004        | Investigator Site 4 | Inactive    |                         |                     |                          |                       |

	

@MockStudyBuilder
#5 RR.02, RR.03
Scenario: Verify title on page is correct, Search boxes, hamburger menu and coloumn visibility are working fine
     Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
     And Patient "patient 1" with patient number "S-10000-004" is associated with "Site 1"
     And Patient Visit "Screening Visit" is associated with "patient 1"
     And Active history is updated for "10000"
     And Site "Site 2" is assigned to Country "India" and has site number "10002" with "inactive" status
     And User "PortalE2EUser" with role "YP" has access to site "10000"
     And User "PortalE2EUser" with role "YP" has access to site "10002"
     And I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And "Site Details (Blinded)" report visibility status is "Enabled"
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     When I click on "Site Details (Blinded)" from the Report List
     Then "Site Details (Blinded)" report title is displayed on screen
     When I enter "10000" in "Search_Site Number" textbox field
	   Then "1" records are displayed in "Site Report" data grid
     And following data is displayed in "Site Report" Grid
        | Country         | Site Number  | Investigator Name   | Site Status | Date of First Screening | Date of Activation  |   Date of Deactivation   | Date of Reactivation  | 
        | United States   | 10000        | Investigator Site 1 | Active      | Current Date            | Current Date        |    Current Date          | Current Date          |
     And I click on "Hamburger Menu" button
     When I click on "Investigator Name" link
     Then "Investigator Name" column is not visible in "Site Report" grid
     And following data is displayed in "Site Report" Grid
        | Country         | Site Number  | Site Status | Date of First Screening | Date of Activation  |   Date of Deactivation   | Date of Reactivation  | 
        | United States   | 10000        | Active      | Current Date            | Current Date        |    Current Date          | Current Date          |
