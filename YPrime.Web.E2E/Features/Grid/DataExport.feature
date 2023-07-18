@DataExport
Feature: DataExport
	

Background: 
        Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
        And Site "Site 2" is assigned to Country "United States" and has site number "20000"
        And Patient "patient 1" with status "Screened" and patient number "S-10000-123" is associated with "Site 1"
   	    And Patient "patient 2" with status "Enrolled" and patient number "S-10000-124" is associated with "Site 1"
        And Patient "patient 3" with status "Screened" and patient number "S-10000-125" is associated with "Site 2"
   	    And Patient "patient 4" with status "Enrolled" and patient number "S-10000-126" is associated with "Site 2"
        And User "PortalE2EUser" with role "YP" has access to site "10000"
        And User "PortalE2EUser" with role "YP" has access to site "20000"
        And User "ypadmin1@yprime.com" with role "YP" has access to site "10000"
 
@MockStudyBuilder
#1
#EXP.02
 Scenario: Validate Placeholder values for all fields
  Given I am logged in as "PortalE2EUser"
  And I am on "At a Glance" page
  And I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  Then "New Export" displayed in "Export Name" placeholder
  And "All Sites" dropdown has placeholder "All Sites"
  And "Patient" dropdown has placeholder "All Patients"
  And "Select Date" displayed in "From" placeholder
  And "Select Date" displayed in "To" placeholder
  And "PortalE2EUser" is displayed in "User" textbox field


@MockStudyBuilder
#2
#EXP.03
#EXP.02
Scenario: Create new Export,validate Completed Export Grid ,run export and visibility of columns in completed Export grid
  Given I am logged in as "PortalE2EUser"
  And I am on "At a Glance" page
  And I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  And I enter "Export test" in "Export Name" textbox field
  And I click on "All Sites" dropdown
  And I select "Site 1" from "All Sites" dropdown
  And I click on "Patient" dropdown
  And I select "All Patients" from "Patient" dropdown
  And I select "01-January-2022" from "From" datepicker
  And I select "02-January-2022" from "To" datepicker
  When I click on "Create" button
  Then "Info" popup is displayed with message "Export succesfully created."
  And I click "Ok" button in the popup
  And "1" entries under the table grid displayed in "Completed Exports" 
  And I wait for "10" seconds
  And following data is displayed in "Completed Export" Grid
   | Export Name | Site Name | Subject Number | From              | To                | Export Status | Created Time     | Started Time     | Completed Time     | Action                   |
   | Export test | Site 1    |                | 01-Jan-2022 00:00 | 02-Jan-2022 23:59 | Complete      | Created DateTime | Started DateTime | Completed DateTime | Download \| Run Export   |
 And I click on "Download" link
 And "Export test*.zip" file is downloaded
 And I click on "Run Export" link
 And "Info" popup is displayed with message "Successfully created export."
 And I click "Ok" button in the popup
 And following data is displayed in "Completed Export" Grid
    | Export Name | Site Name | Subject Number | From              | To                | Export Status | Created Time     | Started Time     | Completed Time     | Action                 |
    | Export test | Site 1    |                | 01-Jan-2022 00:00 | 02-Jan-2022 23:59 | Complete      | Created DateTime | Started DateTime | Completed DateTime | Download \| Run Export |
  And I click on "Hamburger" button
  And I click on "Export Name" link
  And I click on "Subject Number" link
  And "Subject Number" column is not visible in "Completed Export" grid
  And "Export Name" column is not visible in "Completed Export" grid
 
 
@MockStudyBuilder
#3
Scenario: Validate data entry error messages on Data Export page and create export without To and From dates
 Given I am logged in as "PortalE2EUser" 
  And I am on "At a Glance" page
  And  I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  When I click on "Create" button
  Then "The Export Name field is required." error is displayed
  And I enter "Export_Test_Without_To_From" in "Export Name" textbox field
  And I click on "Create" button
  And "Info" popup is displayed with message "Export succesfully created."
  And I click "Ok" button in the popup
  And following data is displayed in "Completed Export" Grid
    | Export Name                      | Site Name    | Subject Number | From            | To              | 
    | Export_Test_Without_To_From      |              |                |All              |All              | 
   And I enter "Export_Test_Without_To_From" in "Export Name" textbox field
   And I click on "Create" button
   And "An export already exists with the name 'Export_Test_Without_To_From'" error is displayed


@MockStudyBuilder
#4
Scenario: Validate site and patient dropdowns as per the user access
  Given I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
  And I am on "At a Glance" page
  And  I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  And I enter "Export test" in "Export Name" textbox field
  And I click on "All Sites" dropdown
  And "Site 2" is not displayed for "All Sites" dropdown
  And I click on "All Sites" dropdown
  And I click on "Patient" dropdown
  And "S-10000-125" is not displayed for "Patient" dropdown
  And "S-10000-126" is not displayed for "Patient" dropdown

@MockStudyBuilder
#5
 Scenario: Export created by one user is not visible to other user
  Given I am logged in as "PortalE2EUser"
  And I am on "At a Glance" page
  And  I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  And I enter "Export test for User1" in "Export Name" textbox field
  And I click on "All Sites" dropdown
  And I select "Site 1" from "All Sites" dropdown
  And I click on "Create" button
  And "Info" popup is displayed with message "Export succesfully created."
  When I click "Ok" button in the popup
  Then following data is displayed in "Completed Export" Grid
    | Export Name             | Site Name |
    | Export test for User1   | Site 1    |   
  And I click on the user icon
  And I click on " Logout" button
  And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
  And I am on "At a Glance" page
  And I click on "Manage Study" link on the top navigation bar
  And I click on "Data Export" link
  And I am on "Data Export" page
  And "0" entries under the table grid displayed in "Completed Exports" 
 