@ApiTest
Feature: Data Sync for Handheld
    ECOA 2530- Initial/Incremental Sync data update
    
    Background: 
        Given Site "site 1" is assigned to Country "United States" and has site number "100000"
        And Patient "patient 1" with patient number "S-10001" is associated with "Site 1"
        And Patient "patient 2" with patient number "S-10002" with status inactive is associated with "Site 1"
        And Patient Visit "visit 1" is associated with "patient 1" 
        And Patient Visit "visit 2" is associated with "patient 2"
        And Diary Entry "diary 1" is associated with "patient 1" 
        And Diary Entry "diary 2" is associated with "patient 2" 
        And Answer "answer 1" is associated with "diary 1" 
        And Answer "answer 2" is associated with "diary 2" 
        And Caregiver "Spouse" is associated with "patient 1"
        And "Phone" Device "YP-E2EPhone" is assigned to Site "Site 1" and assigned to latest software release
     
@ID:2530
    #1
    Scenario: Sync initial data endpoint response includes data for Provisioned device asset tag in api request.
        Given API request contains 
            | Device Type | Patient | AssetTag            |
            | Phone       |         | YP-E2EPhone         |        
        When the request is made to "sync initial data" endpoint 
        Then API response contains
         | Tables                |
         | StudyUser             |
         | Patient               |
         | Device                |
         | Site                  |
         | SiteLanguage          |
         | StudyUserRole         |
         | SystemAction          |
         | SystemActionStudyRole |
        And API response does not contain
         | Tables           |
         | PatientVisit     |
         | PatientAttribute |
         | DiaryEntry       |
         | Answer           |
         | CareGiver        |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"


@ID:2530
    #2
    Scenario: Sync client data endpoint responses include data for patient ID in api request.
        Given Patient "patient 1" with patient number "S-10001" is assigned to device "YP-E2EPhone"
        And API request contains 
              | Device Type | Patient | AssetTag          |
              | Phone       | S-10001 | YP-E2EPhone       |
        When the request is made to "sync client data" endpoint 
        Then API response contains
         | Tables           |
         | PatientVisit     |
         | PatientAttribute |
         | DiaryEntry       |
         | Answer           |
         | StudyUser        |
         | CareGiver        |
        And "PatientVisit" includes "visit 1"
        But "PatientVisit" does not include "visit 2"
 
 @ID:2530
    #3
    Scenario: Sync client data endpoint response includes data for Provisioned device asset tag in api request when subject is not assigned.
        Given API request contains 
              | Device Type | Patient | AssetTag          |
              | Phone       |         | YP-E2EPhone       |
        When the request is made to "sync client data" endpoint 
        Then API response contains
         | Tables                |
         | StudyUser             |
         | Patient               |
         | Device                |
         | Site                  |
         | SiteLanguage          |
         | StudyUserRole         |
         | SystemAction          |
         | SystemActionStudyRole |
        And API response does not contain
         | Tables           |
         | PatientVisit     |
         | PatientAttribute |
         | DiaryEntry       |
         | Answer           |
         | CareGiver        |
        And "Patient" includes "patient 1"
        But "Patient" does not include "patient 2"
        


@ignore
@ID:5209
#1
Scenario: Verify that 200 is returned when Authentication is included  
    Given API request contains authentication header and the following data 
                | Device Type | Patient | AssetTag    |
                | Phone       | S-10001 | YP-E2EPhone |
    When the request is made to "sync client data" endpoint
    Then API successfully response contains 200 

       

@ignore
@ID:5209
#1
Scenario: Verify that 401 is returned when Authentication is not included 
       Given API request contains no authentication header and the following data 
                | Device Type | Patient | AssetTag    |
                | Phone       | S-10001 | YP-E2EPhone |
       When the request is made to "sync client data" endpoint
       Then API unsuccessfully response contains 401