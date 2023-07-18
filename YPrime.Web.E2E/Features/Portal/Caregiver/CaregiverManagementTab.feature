Feature: Caregiver Management Tab Visibility 
    #ECOA-2029 
    #Caregiver Management Tab is visible in Portal 
        
@ID:2029
 #1
 @MockStudyBuilder
 Scenario Outline: Verify that Caregiver Management Tab is visible based on perimission and study configuration
    Given Site "Site 1" is assigned to Country "United States" and has site number "100000"
    And User "PortalE2EUser" with role "YP" has access to site "100000"
    And Patient "patient 1" with patient number "S-10001-003" is associated with "Site 1"
    And Caregiver study setting "Caregivers Enabled" is "<Enable Caregiver Functions>"
    And "CAN CREATE CAREGIVER IN PORTAL" permission is "<CAN CREATE CAREGIVER IN PORTAL>"
    And "CAN VIEW CAREGIVER DETAILS." permission is "<CAN VIEW CAREGIVER DETAILS>"
    And I have logged in with user "PortalE2EUser"
    And I am on Dashboard page
    And I click on "Subject" link on the top navigation bar
    When I select Subject "S-10001-003"
    Then I am on Subject "S-10001-003" page
    And "Caregiver Management" tab is "<Visible>" 
    
    Examples: 
    | Enable Caregiver Functions | CAN CREATE CAREGIVER IN PORTAL | CAN VIEW CAREGIVER DETAILS | Visible     |
    | Enabled                    | Enabled                        | Enabled                    | Visible     |
    | Enabled                    | Enabled                        | Disabled                   | Visible     |
    | Enabled                    | Enabled                        | Disabled                   | Visible     |
    | Enabled                    | Disabled                       | Disabled                   | Not Visible |
    | Disabled                   | Enabled                        | Enabled                    | Not Visible |
    | Disabled                   | Disabled                       | Enabled                    | Not Visible |
    | Disabled                   | Disabled                       | Enabled                    | Not Visible |
    | Disabled                   | Disabled                       | Disabled                   | Not Visible |


    




	
