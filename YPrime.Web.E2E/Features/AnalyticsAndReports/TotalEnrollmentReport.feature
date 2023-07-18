@TotalEnrollmentReport
Feature: TotalEnrollmentReport
	

Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Site "Site 2" is assigned to Country "United States" and has site number "20000"
    And I have added "4" patients to grid starting with "patient 1" number "S-10000-001" and associated with "Site 1"
    And Diary Entry "diary 1" is associated with "patient 1"
	And Diary Entry "diary 2" is associated with "patient 2"
	And Diary Entry "diary 3" is associated with "patient 3"
    And Diary Entry "diary 4" is associated with "patient 4"
    And I update the patient details as follows
		| Subject Number | Subject Status           | Enrollment date | Compliance | Handheld Training Complete | Tablet Training Complete | Last Diary Date | Last Sync Date |
		| S-10000-001   | Screened                 | Yesterday       | True       | True                       | True                     | 21-Dec-2021     | 20-Dec-2021    |
		| S-10000-002   | Enrolled                 | 10-Feb-2019     | False      | False                      | False                    | 10-Jan-2020     | 1-Jan-2021     |
		| S-10000-003   | Provided Written Consent | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |
        | S-10000-004   | Completed                | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |  
     And I have added "6" patients to grid starting with "patient 1" number "S-20000-001" and associated with "Site 2"
     And Diary Entry "diary 5" is associated with "patient 1"
     And Diary Entry "diary 6" is associated with "patient 2"
     And Diary Entry "diary 7" is associated with "patient 3"
     And Diary Entry "diary 8" is associated with "patient 4"
     And Diary Entry "diary 9" is associated with "patient 5"
     And Diary Entry "diary 10" is associated with "patient 6"
     And I update the patient details as follows 
        | Subject Number | Subject Status             | Enrollment date | Compliance | Handheld Training Complete | Tablet Training Complete | Last Diary Date | Last Sync Date |
		| S-20000-001   | Screen Failed               | Yesterday       | True       | True                       | True                     | 21-Dec-2021     | 20-Dec-2021    |
		| S-20000-002   | Early Withdrawl             | 10-Feb-2019     | False      | False                      | False                    | 10-Jan-2020     | 1-Jan-2021     |
		| S-20000-003   | Randomization Ineligibility | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |
        | S-20000-004   | Treatment Completed         | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |
        | S-20000-005   | Discontinued                | 23-Jan-2020     | False      | True                       | False                    | 02-Mar-2021     | 27-Feb-2021    |  
        | S-20000-006   | Bill Test Status1           | 23-Jan-2020     | False      | True                       | False                    | 02-Mar-2021     | 27-Feb-2021    |  
     And User "PortalE2EUser" with role "YP" has access to site "10000"
     And User "PortalE2EUser" with role "YP" has access to site "20000"
     And User "ypadmin1@yprime.com" with role "YP" has access to site "10000"

 
 
@MockStudyBuilder
#1 RR.01
Scenario: If permission is disabled "Total Enrollment (Unblinded)" report link will not be displayed
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And "Total Enrollment (Unblinded)" report visibility status is "Disabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    When I am on "Reports" page
    Then "Total Enrollment (Unblinded)" link is "NOT VISIBLE"

@MockStudyBuilder
#2
#TER.02
#TER.03
#TER.06
#TER.07
# RR.01
Scenario: Verify "Total Enrollment (Unblinded)" Report when permission is "enabled" and check the grid values in the Total Enrollment Grid
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And "Total Enrollment (Unblinded)" report visibility status is "Enabled" 
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
    When I am on "Reports" page
    Then "Total Enrollment (Unblinded)" link is "Visible"
    And I click on "Total Enrollment (Unblinded)" from the Report List
    And "Enrollment" graph is displayed 
    And "Total - 10" is "visible" below the graph  
    And following data is displayed in "Total Enrollment (Unblinded)" Grid
       | Site Name | Total Subject Count | Bill Test Status1 | Completed | Discontinued    |Early Withdrawl|Enrolled         |Provided Written Consent     |Randomization Ineligibility|Screen Failed |Screened| Treatment Completed |
       | All Sites | 10                  | 1                 | 1         |  1              | 1             | 1               | 1                           | 1                         | 1            | 1      |  1                  |
     And I click on the user icon
     And I click on " Logout" button
     And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
     And I am on "At a Glance" page
     And "Total Enrollment (Unblinded)" report visibility status is "Enabled" 
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     And I am on "Reports" page
     And I click on "Total Enrollment (Unblinded)" from the Report List
     And "Enrollment" graph is displayed 
     And "Total - 4" is "visible" below the graph
     And following data is displayed in "Total Enrollment (Unblinded)" Grid
         | Site Name | Total Subject Count | Bill Test Status1 | Completed | Discontinued    |Early Withdrawl|Enrolled         |Provided Written Consent     |Randomization Ineligibility|Screen Failed |Screened|Treatment Completed |
         | All Sites | 4                   | 0                 | 1         |  0              | 0             | 1               | 1                           | 0                         | 0            | 1       |   0                 |
    

@MockStudyBuilder
#3
#TER.05    
#TER.04 
# RR.02, RR.03
Scenario: Subject status count is displayed upon mouse hover on graph and validate export of Total Enrollment Report
     Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And "Total Enrollment (Unblinded)" report visibility status is "Enabled" 
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     When I am on "Reports" page
     And I click on "Total Enrollment (Unblinded)" from the Report List
     Then I hover on "screened" "Screened - 1" message is displayed
     And I hover on "enrolled" "Enrolled - 1" message is displayed
     And I hover on "completed" "Completed - 1" message is displayed
     And I hover on "providedwrittenconsent" "Provided Written Consent - 1" message is displayed
     And I hover on "screenfailed" "Screen Failed - 1" message is displayed
     And I hover on "earlywithdrawl" "Early Withdrawl - 1" message is displayed
     And I hover on "treatmentcompleted" "Treatment Completed - 1" message is displayed
     And I hover on "randomizationineligibility" "Randomization Ineligibility - 1" message is displayed
     And I hover on "billteststatus1" "Bill Test Status1 - 1" message is displayed
     And I hover on "discontinued" "Discontinued - 1" message is displayed
     And I click on "Hamburger" button
     And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Total Enrollment (Unblinded)" in ".pdf" format file to save in Export Evidence folder 
    
     
@MockStudyBuilder
#4 RR.02, RR.03
Scenario: Verify if title on page is correct and search boxes are working fine
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Total Enrollment (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Total Enrollment (Unblinded)" from the Report List
        And "Total Enrollment (Unblinded)" report title is displayed on screen
        When I enter "All Sites" in "Search_Site Name" textbox field
	    Then "1" records are displayed in "Total Enrollment (Unblinded)" data grid
        And following data is displayed in "Total Enrollment (Unblinded)" Grid
        | Site Name | Total Subject Count | Bill Test Status1 | Completed | Discontinued | Early Withdrawl | Enrolled | Provided Written Consent | Randomization Ineligibility | Screen Failed | Screened | Treatment Completed |
        | All Sites | 10                  | 1                 | 1         | 1            | 1               | 1        | 1                        | 1                           | 1             | 1        | 1                   |
        When I click on "Hamburger" button
        Then hamburger menu is displayed with the following functionality for export
		| ButtonName	|
		| Excel			|
		| CSV           |
		| PDF           |
		| Print         |
        When I click on "Bill Test Status1" link
        Then "Bill Test Status1" column is not visible in "Total Enrollment (Unblinded)" grid
        And following data is displayed in "Total Enrollment (Unblinded)" Grid
        | Site Name | Total Subject Count | Completed | Discontinued | Early Withdrawl | Enrolled | Provided Written Consent | Randomization Ineligibility | Screen Failed | Screened | Treatment Completed |
        | All Sites | 10                  | 1         | 1            | 1               | 1        | 1                        | 1                           | 1             | 1        | 1                   |
     




      
  
