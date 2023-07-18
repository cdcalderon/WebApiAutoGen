@ApiTest
Feature: Data Sync For BYOD
 ECOA 2531 - Initial/ Incremental sync update 
	
Background: 
        Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
        And Patient "patient 1" with patient number "S-10001" is associated with "Site 1"
        And Patient "patient 2" with patient number "S-10002" is associated with "Site 1"
        And Diary Entry "diary 1" is associated with "patient 1" 
        And Diary Entry "diary 2" is associated with "patient 2" 
        And Patient Visit "visit 1" is associated with "patient 1" 
        And Patient Visit "visit 2" is associated with "patient 2"
        And Answer "answer 1" is associated with "diary 1" 
        And Answer "answer 2" is associated with "diary 2"         
        And "BYOD" Device "YP-E2EBYOD" is assigned to Site "Site 1" and assigned to latest software release
        And Patient "patient 1" with patient number "S-10001" is assigned to device "YP-E2EBYOD"
        

@ID:2531
    #1
    Scenario: Sync initial data endpoint response includes data for BYOD enrollment ID in api request. 
        Given API request contains 
            | Device Type | Patient | AssetTag   |
            | BYOD        | null    | YP-E2EBYOD |
        When the request is made to "sync initial data" endpoint
        Then API response contains 
            | Tables        |
            | DiaryEntry    |
            | Answer        |
            | Patient       |
            | PatientVisit  |
        And API response does not contain
            | Tables           |
            | StudyUser        |
            | StudyUserRole    |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"
        And "DiaryEntry" includes "diary 1"
        But "DiaryEntry" does not include "diary 2"
        And "Answer" includes "answer 1"
        But "Answer" does not include "answer 2"
        And "PatientVisit" includes "visit 1"
        But "PatientVisit" does not include "visit 2"
    
@ID:2531
    #2
    Scenario: Sync client data endpoint response includes data for BYOD enrollment ID in api request.
        Given API request contains 
                | Device Type | Patient | AssetTag   |
                | BYOD        | S-10001 | YP-E2EBYOD |
        When the request is made to "sync client data" endpoint
        Then API response contains 
            | Tables        |
            | DiaryEntry    |
            | Answer        |
            | Patient       |
            | PatientVisit  |
        And API response does not contain
            | Tables           |
            | StudyUser        |
            | StudyUserRole    |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"
        And "DiaryEntry" includes "diary 1"
        But "DiaryEntry" does not include "diary 2"
        And "Answer" includes "answer 1" 
        But "Answer" does not include "answer 2" 
        And "PatientVisit" includes "visit 1"
        But "PatientVisit" does not include "visit 2"



@ignore
@ID:5209
#1
Scenario: Verify that 200 is returned when Authentication is included  
    Given API request contains authentication header and the following data 
                | Device Type | Patient | AssetTag   |
                | BYOD        | S-10001 | YP-E2EBYOD |
    When the request is made to "sync client data" endpoint
    Then API successfully response contains 200 

       

@ignore
@ID:5209
#1
Scenario: Verify that 401 is returned when Authentication is not included 
       Given API request contains no authentication header and the following data 
                | Device Type | Patient | AssetTag   |
                | BYOD        | S-10001 | YP-E2EBYOD |
        When the request is made to "sync client data" endpoint
        Then API unsuccessfully response contains 401