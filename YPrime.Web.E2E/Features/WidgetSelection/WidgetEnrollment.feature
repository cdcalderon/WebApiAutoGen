
@WidgetEnrollment
Feature:Widget Enrollment


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
     And I have added "4" patients to grid starting with "patient 1" number "S-20000-001" and associated with "Site 2"
     And Diary Entry "diary 5" is associated with "patient 1"
     And Diary Entry "diary 6" is associated with "patient 2"
     And Diary Entry "diary 7" is associated with "patient 3"
     And Diary Entry "diary 8" is associated with "patient 4"
     And I update the patient details as follows 
        | Subject Number | Subject Status             | Enrollment date | Compliance | Handheld Training Complete | Tablet Training Complete | Last Diary Date | Last Sync Date |
		| S-20000-001   | Screen Failed               | Yesterday       | True       | True                       | True                     | 21-Dec-2021     | 20-Dec-2021    |
		| S-20000-002   | Early Withdrawl             | 10-Feb-2019     | False      | False                      | False                    | 10-Jan-2020     | 1-Jan-2021     |
		| S-20000-003   | Randomization Ineligibility | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |
        | S-20000-004   | Treatment Completed         | 22-Jan-2020     | False      | True                       | False                    | 03-Mar-2021     | 25-Feb-2021    |
     And User "PortalE2EUser" with role "YP" has access to site "10000"
     And User "ypadmin1@yprime.com" with role "YP" has access to site "10000" 
     And User "ypadmin1@yprime.com" with role "YP" has access to site "20000"
    
@MockStudyBuilder
#1
#ENRL.02 :‘Total’ will be the default display beneath the donut graph without any selection or user prompt.
#ENRL.03 :‘Total’ is defined as the count of unique Subject identifiers available on portal webpage.  Total will always match the sum of Subjects listed within the “all Subjects” tab.
#ENRL.11 : The title of the widget will always be displayed justified left and be noted as, “ENROLLMENT”
Scenario: Total Count is equal the sum of Subjects listed within the “all Subjects” tab and title of the widget should be Enrollment
     Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And I click on "At a Glance" link on the top navigation bar
     When I am on At a Glance page
     Then "Enrollment" graph is displayed 
     And "Enrollment" text is "visible"
     And "Total - 4" is "visible" below the graph
  

@MockStudyBuilder
#2
#ENRL.09 :Depending on role and access right, this widget will display on a Subject or study level. Only sites that a user has access to will display within this widget.
Scenario: Check the scope of User having access to site
     Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And I click on "At a Glance" link on the top navigation bar
     When I am on At a Glance page
     Then "Total - 4" is "visible" below the graph
     And I click on the user icon
     And I click on " Logout" button
     And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
     And I am on "Subject Management" page
     And I click on "At a Glance" link on the top navigation bar
     When I am on At a Glance page
     Then "Total - 8" is "visible" below the graph
     

@MockStudyBuilder
#3
#ENRL.10:  Only subject statuses that are used within the study will be displayed within the widget (upon subject status assignment).
#ENRL.12:  Upon hover over of the pie chart, “TOTAL” will be updated to display the subject status type the user is over with the number subject in that status, “subject status - X” 
Scenario: Subject status count is displayed upon mouse hover on graph 
     Given I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
     And I am on "Subject Management" page
     And I click on "At a Glance" link on the top navigation bar
     And I am on At a Glance page
     When I hover on "screened" "Screened - 1" message is displayed
     And I hover on "enrolled" "Enrolled - 1" message is displayed
     And I hover on "completed" "Completed - 1" message is displayed
     And I hover on "providedwrittenconsent" "Provided Written Consent - 1" message is displayed
     And I hover on "screenfailed" "Screen Failed - 1" message is displayed
     And I hover on "earlywithdrawl" "Early Withdrawl - 1" message is displayed
     And I hover on "treatmentcompleted" "Treatment Completed - 1" message is displayed
     And I hover on "randomizationineligibility" "Randomization Ineligibility - 1" message is displayed
     And The Subject status "discontinued" is not "visible" on the graph





    

