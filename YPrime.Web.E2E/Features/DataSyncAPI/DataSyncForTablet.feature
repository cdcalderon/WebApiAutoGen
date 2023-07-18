@ApiTest
Feature: Data Sync For Tablet
 ECOA 2597 - API E2E Setup
	
Background: 
        Given Site "Site 10" is assigned to Country "United States" and has site number "100010"
        And Site "Site 11" is assigned to Country "United States" and has site number "100011"
        And Patient "patient 1" with patient number "S-10001" is associated with "Site 10"
        And Patient "patient 2" with patient number "S-10002" is associated with "Site 11"
        And Diary Entry "diary 1" is associated with "patient 1" 
        And Diary Entry "diary 2" is associated with "patient 2" 
        And Answer "answer 1" is associated with "diary 1" 
        And Answer "answer 2" is associated with "diary 2"  
        And The following Software Releases are assigned to the study
			| Version  | PackagePath   | Configuration Version |
			| 5.1.0.13 | software2.zip | 13.0-0.1              |
        And "Tablet" Device "YP-E2ETABLET" is assigned to Site "Site 10"    
		And "Tablet" Device "YP-E2ETABLET" is assigned to Software Release "Release 5.1.0.13"

@ID:2597
    #1
    Scenario: Sync initial data endpoint response includes all transactional data for study
        Given API request contains 
            | Device Type | Patient | AssetTag     |
            | Tablet      | null    | YP-E2ETABLET |
        When the request is made to "sync initial data" endpoint
        Then API response contains 
            | Tables                       |
            | Patient                      |
            | PatientVisit                 |
            | DiaryEntry                   |
            | Answer                       |
            | PatientAttribute             |
            | SecurityQuestion             |
            | CareGiver                    |
            | Device                       |
            | InputFieldTypeResult         |
            | QuestionInputFieldTypeResult |
            | Site                         |
            | SiteLanguage                 |
            | StudyUser                    |
            | StudyUserRole                |
            | SystemAction                 |
            | SystemActionStudyRole        |
            | AnswerScore                  |
            | MissedVisitReason            |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"
        And "DiaryEntry" includes "diary 1"
        But "DiaryEntry" does not include "diary 2"
        And "Answer" includes "answer 1"
        But "Answer" does not include "answer 2"

@ID:2597     
    #2
    Scenario: Sync client data endpoint response includes data for site in api request.
        Given API request contains 
                | Device Type | Patient | AssetTag     |
                | Tablet      | null    | YP-E2ETABLET |
        When the request is made to "sync client data" endpoint
        Then API response contains 
            | Tables                       |
            | Patient                      |
            | PatientVisit                 |
            | DiaryEntry                   |
            | Answer                       |
            | PatientAttribute             |
            | SecurityQuestion             |
            | CareGiver                    |
            | Device                       |
            | InputFieldTypeResult         |
            | QuestionInputFieldTypeResult |
            | Site                         |
            | SiteLanguage                 |
            | StudyUser                    |
            | StudyUserRole                |
            | SystemAction                 |
            | SystemActionStudyRole        |
            | AnswerScore                  |
            | MissedVisitReason            |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"
        And "DiaryEntry" includes "diary 1"
        But "DiaryEntry" does not include "diary 2"
        And "Answer" includes "answer 1"
        But "Answer" does not include "answer 2"



@ignore
@ID:5209
#1
Scenario: Verify that 200 is returned when Authentication is included  
    Given API request contains authentication header and the following data 
                | Device Type | Patient | AssetTag     |
                | Tablet      | S-10001 | YP-E2ETablet |
    When the request is made to "sync client data" endpoint
    Then API successfully response contains 200 

       

@ignore
@ID:5209
#1
Scenario: Verify that 401 is returned when Authentication is not included 
       Given API request contains no authentication header and the following data 
                | Device Type | Patient | AssetTag     |
                | Tablet      | S-10001 | YP-E2ETablet | 
       When the request is made to "sync client data" endpoint
       Then API unsuccessfully response contains 401