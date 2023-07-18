Feature: Subject Questionnaire Details

#NLA-118
Background:
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
	And User "PortalE2EUser" with role "YP" has access to site "10001"
	And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"
	And Diary Entry "Questionnaire Forms" is associated with "patient 1"
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
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "In general, How would you say your health is:" and choice "Not better"
    And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Good"
   
#1
#SubQ.01,SubQ.02
@MockStudyBuilder
Scenario: Verify Questionnaire detail if Diary source is eCOA App    
	Given I am logged in as "PortalE2EUser"
    And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label                                                                     | Value                      |       
        | Data Source Name                                                          |   eCOA App                 |
        | Started Time                                                              |   Current Date             |
        | Completed Time                                                            |   Current Date             |
        | Transmitted Time                                                          |   Current Date             |
    And  I am on "Subject Management" page    
    And I select Subject "S-10001-004"
    And I am on Subject "S-10001-004" page
    When I click on "Questionnaires" tab
    Then following data is displayed in "Subject Questionnaire" Grid
    | Questionnaire        | Diary Date   | Diary Status | Diary Source | Visit | Start Time      | Time Completed      | Time Transmitted |
    | Questionnaire Forms  | Current Date | Source       | eCOA App     |       |Current Date     | Current Date        | Current Date     |
    And I click on "Questionnaire Forms" link
    And I am on "Diary Entry Details" page
    When "S-10001-004" text is displayed in the page
    Then the following text is displayed on the "Dairy Entry Details" page
        | Label                                                                     | Value                      |
        | Diary Status                                                              |   Source                   |
        | Data Source Name                                                          |   eCOA App                 |
        | Diary Date                                                                |   Current Date             |
        | Questionnaire Display Name                                                |   Questionnaire Forms      |
        | Visit Name                                                                |                            |
        | Asset Tag                                                                 |   YP-E2E-Device            |
        | Version                                                                   |   1.0.0.0-1.0-0.4          |
        | Started Time                                                              |    Current Date            |
        | Completed Time                                                            |    Current Date            |
        | Transmitted Time                                                          |    Current Date            |    
    Then following data is displayed in "Response" Grid
        | Question                                                                  | Response            |
        | In general, How would you say your health is:                             | Not better          |
    Then following data is displayed in "Response" Grid
        | Question                                                                  | Response            |
        | How would you rate your health in general?                                | Good                |
    And I click on "Back to Subject" link
    And I am on Subject "S-10001-004" page
   

#2
#SubQ.01,SubQ.02
@MockStudyBuilder
Scenario: Verify Questionnaire and visit  detail if Diary source is Web Diary
	Given I am logged in as "PortalE2EUser"
    And Patient Visit "Screening Visit" is associated with "patient 1"
    And I update Diary Entry "Questionnaire Forms" for "patient 1" with following details
        | Label                                                                     | Value                |       
        | Data Source Name                                                          |   Web Diary          |
        | Started Time                                                              |   Current Date       |
        | Completed Time                                                            |   Current Date       |
        | Transmitted Time                                                          |   Current Date       |
        |  Visit Name                                                               |   Screening Visit    |
    And I am on "Subject Management" page    
    And I select Subject "S-10001-004"
    And I am on Subject "S-10001-004" page
    When I click on "Questionnaires" tab
    Then following data is displayed in "Subject Questionnaire" Grid
    | Questionnaire       | Diary Date   | Diary Status | Diary Source | Visit             | Start Time  | Time Completed   | Time Transmitted |
    | Questionnaire Forms | Current Date | Source       | Web Diary    | Screening Visit   | Current Date   | Current Date  | Current Date     |
    And I click on "Questionnaire Forms" link
    And I am on "Diary Entry Details" page
    And "S-10001-004" text is displayed in the page
    When "Screening Visit" text is displayed in the page
    Then the following text is displayed on the "Dairy Entry Details" page
        | Label                                                                     | Value                  |
        | Diary Status                                                              |   Source               |
        | Data Source Name                                                          |   Web Diary            |
        | Diary Date                                                                |   Current Date         |
        | Questionnaire Display Name                                                |   Questionnaire Forms  |
        | Visit Name                                                                |   Screening Visit      |
        | Asset Tag                                                                 |   YP-E2E-Device        |
        | Version                                                                   |   1.0.0.0-1.0-0.4      |
        | Started Time                                                              |   Current Date         |
        | Completed Time                                                            |   Current Date         |
        | Transmitted Time                                                          |   Current Date         |    
    Then following data is displayed in "Response" Grid
        | Question                                                                  | Response            |
        | In general, How would you say your health is:                             | Not better          |
    Then following data is displayed in "Response" Grid
        | Question                                                                  | Response            |
        | How would you rate your health in general?                                | Good                |
    And I click on "Back to Subject" link
    And I am on Subject "S-10001-004" page