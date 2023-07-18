Feature: Visit Compliance
	NLA-129

@MockStudyBuilder
#1 VCR.02 VCR.03 RR.02
Scenario: Verify "Visit Compliance" Grid and File Format after exporting "Visit Compliance" report
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
	And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And I update active history of Site "10001" for "Visit Compliance" report
	And Patient "patient 1" has the following subject attributes:
		| Label         | Value          |
		| Gender        | Female         |
		| Date of birth | (Current Date) |
		| Height        | 100            |
		| Weight        | 100            |	
    And "Phone" Device "YP-E2E-Device" is assigned to Site "Initial Site"
	And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"    
    And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
	And Subject "S-10001-004" is assigned to "YP-E2E-Device" Device
    And "2" questionnaire has been configured in "Treatment Visit"
    And "2" questionnaire has been configured in "Screening Visit"
    And "2" questionnaire has been configured in "Enrollment Visit"
    And Subject "S-10001-004" has completed "Header Display Name Test" questionnaire for question "Are you experiencing Headache?" and choice "yes"
    And Subject "S-10001-004" has completed "NRS Choices Questionnaire" questionnaire for question "Please select the percentage that shows how much relief you have received." and choice "100%"
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "In general, How would you say your health is:" and choice "Not better"
    And Subject "S-10001-004" has completed "Time Questionnaire 24hr" questionnaire for question "What time of the day is your cough the worst? 24 hr" and choice "12:00"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And Patient Visit "Enrollment Visit" is associated with "patient 1"
    And Patient Visit "Treatment Visit" is associated with "patient 1"
    And I am logged in as "PortalE2EUser"
    And I update Diary Entry "Header Display Name Test" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   Web Diary          |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        | Visit Name                                                               |   Enrollment Visit    |
    And I update Diary Entry "NRS Choices Questionnaire" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   Web Diary          |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        |  Visit Name                                                               |   Treatment visit    |        
     
    And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   Web Diary          |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        |  Visit Name                                                               |   Screening Visit    |
   And I update Diary Entry "Time Questionnaire 24hr" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   Web Diary          |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        | Visit Name                                                                |   Screening Visit    |
    And I am on "At a Glance" page
    And "Visit Compliance (Unblinded)" report visibility status is "Enabled"
    And I click on "Analytics & Reports" link on top navigation bar
    And I click on "Reports" link
	And I am on "Reports" page
	When I click on "Visit Compliance (Unblinded)" link
    Then Page title is "Visit Compliance (Unblinded)"
    When I click on "Hamburger Menu" button
	Then hamburger menu is displayed with the following functionality for export
		| ButtonName |
		| Excel      |
		| CSV        |
		| PDF        |
		| Print      |
    And I click on "PDF" button to generate "YPrime_eCOA-E2E-Mock_Visit Compliance (Unblinded)" in ".pdf" format file to save in Export Evidence folder
    And I click on background screen
    And following data is displayed in "Visit Compliance" Grid
        | Site  | Subject     | Visit            | Visit Date   | Visit Compliance | Average Compliance for Visits | Date of Deactivation | Date of Reactivation | Header Display Name Test | NRS Choices Questionnaire | Number Spinner with 1 decimal digit | Page Navigation Questionnaire | Questionnaire Forms | Time Questionnaire 24hr |
        | 10001 | S-10001-004 | Enrollment Visit | Current Date | 50.00%           | 66.67%                        | Current Date         | Current Date         | Completed                |                           | N/A                                 |                               |                     |                         |
        | 10001 | S-10001-004 | Screening Visit  | Current Date | 100.00%          | 66.67%                        | Current Date         | Current Date         |                          |                           |                                     |                               | Completed           | Completed               |
        | 10001 | S-10001-004 | Treatment Visit  | Current Date | 50.00%           | 66.67%                        | Current Date         | Current Date         |                          | Completed                 |                                     | N/A                           |                     |                         |
   And I click on the background screen
   When I enter "Enrollment visit" in "Search Visit" textbox field
   Then following data is displayed in "Visit Compliance" Grid
        | Site  | Subject     | Visit            | Visit Date   | Visit Compliance | Average Compliance for Visits | Date of Deactivation | Date of Reactivation | Header Display Name Test | NRS Choices Questionnaire | Number Spinner with 1 decimal digit | Page Navigation Questionnaire | Questionnaire Forms | Time Questionnaire 24hr |
        | 10001 | S-10001-004 | Enrollment Visit | Current Date | 50.00%           | 66.67%                        | Current Date         | Current Date         | Completed                |                           | N/A                                 |                               |                     |                         |


@MockStudyBuilder
# RR.01
Scenario: "Visit Compliance (Unblinded)" report link is not displayed if "Visit Compliance (Unblinded)" is disabled
    Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And "Visit Compliance (Unblinded)" report visibility status is "Disabled"
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     When I am on "Reports" page
     Then "Visit Compliance (Unblinded)" report link is "Not Visible"


@MockStudyBuilder
#VCR.06 RR.02 RR.03 RR.01
Scenario: Verify grid columns visiblity 
     Given I am logged in as "PortalE2EUser"
     And I am on "At a Glance" page
     And "Visit Compliance (Unblinded)" report visibility status is "Enabled"
     And I click on "Analytics & Reports" link on top navigation bar
     And I click on "Reports" link
     And I am on "Reports" page
     And I click on "Visit Compliance (Unblinded)" from the Report List
	 When I click on "Hamburger Menu" button
     Then following columns are displayed for "Visit Compliance" Grid Menu
        | Label                               |
        | Site                                |
        | Subject                             |
        | Visit                               |
        | Visit Date                          |
        | Visit Compliance                    |
        | Average Compliance for Visits       |
        | Date of Deactivation                |
        | Date of Reactivation                |
     And I click on "Site" link
     And I click on the background screen
     And "Site" column is not visible in "Visit Compliance" grid