@AverageDiaryDurationReport
Feature: AverageDiaryDurationReport
#NLA-123
	

Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Site "Site 2" is assigned to Country "United States" and has site number "20000"
  	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And Patient "patient 1" with patient number "S-10000-001" is associated with "Site 1"
    And Patient "patient 2" with patient number "S-10000-002" is associated with "Site 1"
    And Patient "patient 3" with patient number "S-20000-003" is associated with "Site 2"
    And Patient "patient 4" with patient number "S-20000-004" is associated with "Site 2"
    And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 1"
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
    And "Phone" Device "YP-E2E-Device1" is assigned to Site "Site 2"
	And "Phone" Device "YP-E2E-Device1" is assigned to Software Release "Initial Release"
    And "Tablet" Device "YP-E2E-Device2" is assigned to Site "Site 1"   
    And "Tablet" Device "YP-E2E-Device2" is assigned to Software Release "Initial Release"
    And "Tablet" Device "YP-E2E-Device3" is assigned to Site "Site 2"   
    And "Tablet" Device "YP-E2E-Device3" is assigned to Software Release "Initial Release"
	And Subject "S-10000-001" is assigned to "YP-E2E-Device" Device
    And Subject "S-10000-002" is assigned to "YP-E2E-Device1" Device
    And Subject "S-20000-003" is assigned to "YP-E2E-Device2" Device
    And Subject "S-20000-004" is assigned to "YP-E2E-Device3" Device
    And Subject "S-10000-001" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Good"
    And Subject "S-10000-002" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Good"
    And Subject "S-20000-004" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Good"
    And Subject "S-10000-002" has completed "NRS Choices Questionnaire" questionnaire for question "Please select the percentage that shows how much relief you have received." and choice "100%"
    And Subject "S-20000-003" has completed "Page Navigation Questionnaire" questionnaire for question "7. Did you need to take medication?" and choice "Yes"
    And Questionnaires will be completed at time by subjects as follows 
    | Subject     | Patient   | Forms                        | Started Time                       | Completed Time                     |
    | S-10000-001 | patient 1 | Questionnaire Forms          | 2022-02-21 10:10:00.0000000 +00:00 | 2022-02-21 10:15:00.0000000 +00:00 |
    | S-10000-002 | patient 2 | Questionnaire Forms          | 2022-02-21 10:00:00.0000000 +00:00 | 2022-02-21 10:04:00.0000000 +00:00 |
    | S-20000-004 | patient 4 | Questionnaire Forms          | 2022-02-21 11:05:00.0000000 +00:00 | 2022-02-21 11:07:00.0000000 +00:00 |
    | S-10000-002 | patient 2 | NRS Choices Questionnaire    | 2022-02-21 10:00:00.0000000 +00:00 | 2022-02-21 10:05:00.0000000 +00:00 |
    | S-20000-003 | patient 3 | Page Navigation Questionnaire| 2022-02-21 11:05:00.0000000 +00:00 | 2022-02-21 11:08:00.0000000 +00:00 |


 
@MockStudyBuilder
#1 RR.01
Scenario: If permission is disabled "Average Diary Duration (Unblinded)" report link will not be displayed
      Given I am logged in as "PortalE2EUser"
      And I am on "At a Glance" page
      And "Average Diary Duration (Unblinded)" report visibility status is "Disabled"
      And I click on "Analytics & Reports" link on top navigation bar
      And I click on "Reports" link
      When I am on "Reports" page
      Then "Average Diary Duration (Unblinded)" link is "NOT VISIBLE"

@MockStudyBuilder
#2 ADD.02, ADD.03, ADD.04, ADD.05, ADD.07
Scenario:  Verify Average Diary Duration (Unblinded) visibility and Average Diary Duration (Unblinded) page.
        Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Average Diary Duration (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        When I click on "Average Diary Duration (Unblinded)" from the Report List
        Then bar graph displays row containing AverageTime for each completed Questionnaire
         | X-Axis                        | Y-Axis      |
         | NRS Choices Questionnaire     | 5.0         |
         | Page Navigation Questionnaire | 3.0         |
         | Questionnaire Forms           | 4.0         |
        And following data is displayed in "Average Diary Duration (Unblinded)" Grid
         | Questionnaire Name            | Average Duration (Minutes)|
         | NRS Choices Questionnaire     | 5                         |
         | Page Navigation Questionnaire | 3                         |
         | Questionnaire Forms           | 3.67                      |
        
     
@MockStudyBuilder
#3 RR.02, RR.03 ,ADD.06
Scenario: Verify title on page is correct, Search boxes, export, hamburger menu and coloumn visibility are working fine
        Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And "Average Diary Duration (Unblinded)" report visibility status is "Enabled"
        And I click on "Analytics & Reports" link on top navigation bar
        And I click on "Reports" link
        And I click on "Average Diary Duration (Unblinded)" from the Report List
        And "Average Diary Duration (Unblinded)" report title is displayed on screen
        When I click on "Hamburger Menu" button 
	    Then hamburger menu is displayed with the following functionality for export
		| ButtonName |
		| Excel      |
		| CSV        |  
		| PDF        |
		| Print      |
        And hamburger menu is displayed with the following functionality for Visibility
		| ButtonName                 |
		| Questionnaire Name         |
		| Average Duration (Minutes) |
        And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Average Diary Duration (Unblinded)" in ".pdf" format file to save in Export Evidence folder
        And I click on "Average Duration (Minutes)" link
	    And I click on the background screen
	    And "Average Duration (Minutes)" grid column is not visible
        And following data is displayed in "Average Diary Duration (Unblinded)" Grid
        | Questionnaire Name            |
        | NRS Choices Questionnaire     |
        | Page Navigation Questionnaire |
        | Questionnaire Forms           |
        And I enter "NRS Choices Questionnaire" in "Search_QuestionnaireName" textbox field
	    And "1" records are displayed in "Average Diary Duration (Unblinded)" data grid
        And following data is displayed in "Average Diary Duration (Unblinded)" Grid
        | Questionnaire Name        | 
        | NRS Choices Questionnaire |

