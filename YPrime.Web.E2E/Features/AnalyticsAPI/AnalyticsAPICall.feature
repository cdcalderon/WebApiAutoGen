@ApiTest

Feature: Analytics API Call

@ID:4668
#1
Scenario: Call endpoint and verify that the data is in the analytics table. 
    Given API request contains analytics
            | InternalName            | DisplayName            |
            | Analytics Internal Name | Analytics Display Name |
    When the request is made to Add Analytics Report endpoint 
    Then the Add Analytics Report API response status code is "200"
    And the data is in the analytics table

@ID:4668
#2
Scenario: Call endpoint with a incomplete data (no DisplayName) verify that error is returned
    Given API request contains analytics 
            | InternalName            | DisplayName            |
            | Analytics Internal Name |                        |
    When the request is made to Add Analytics Report endpoint
    Then the Add Analytics Report API response status code is "400"

@ID:4668
#3
Scenario: Call endpoint with a incomplete data (no InternalName) verify that error is returned 
    Given API request contains analytics 
            | InternalName            | DisplayName            |
            |                         | Analytics Display Name |
    When the request is made to Add Analytics Report endpoint
    Then the Add Analytics Report API response status code is "400"

@ID:4668
#4
Scenario: Call endpoint with duplicate data verify that error is returned. 
    Given API request contains analytics
            | InternalName            | DisplayName            |
            | Analytics Internal Name | Analytics Display Name |  
    And the request is made to Add Analytics Report endpoint
    And the Add Analytics Report API response status code is "200"
    And API request contains analytics
            | InternalName            | DisplayName            |
            | Analytics Internal Name | Analytics Display Name |  
    When the request is made to Add Analytics Report endpoint
    Then the Add Analytics Report API response status code is "500"


@ID:6101
#5
Scenario: Verify that the update API is called and the report name is updated 
    Given Analytics report has the following data
            | InternalName            | DisplayName     |
            | Analytics Internal Name | Customer Report |
    And API request contains analytics
            | InternalName            | DisplayName    |
            | Analytics Internal Name | Sponsor Report |
    When the request is made to "Update" Analytics Report endpoint "Analytics Internal Name Update" "Analytics Internal Name Update"
    Then the Update Analytics Report API reponse status code is "200" 
    And the Updated data is in the analytics table "Analytics Internal Name Update"


@ID:6102
#6
Scenario: Verify that the delete API call removed the report from the list 
    Given Analytics report has the following data
            | InternalName            | DisplayName     |
            | Analytics Internal Name | Customer Report |
    And API request contains analytics
            | InternalName            | DisplayName     |
            | Analytics Internal Name | Customer Report |
    When the request is made to "Delete" Analytics Report endpoint "null" "null"
    Then the Delete Analytics Report API response status code is "200" 
    And the data is not in the analytics table


@ID:5209
#5
Scenario: Verify that 200 is returned with Authentication included
    Given API request contains authentication header and has the following data
             | InternalName            | DisplayName     |
             | Analytics Internal Name | Customer Report |
    When the request is made to Add Analytics Report endpoint with authentication header
    Then the Add Analytics Report API response status code is "200"

@ID:5209
#6
Scenario: Verify that 401 is returned with Authentication not included
    Given API request contains no authentication header and has the following data
             | InternalName            | DisplayName     |
             | Analytics Internal Name | Customer Report |
    When the request is made to Add Analytics Report endpoint
    Then the Add Analytics Report API response status code is "401"