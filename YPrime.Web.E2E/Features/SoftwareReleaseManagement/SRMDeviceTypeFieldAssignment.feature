@ignore


Feature: SRM Device Type Field Assignment

Background: 
    Given I have logged in with user "PortalE2EUser"                
    And Site "Site 3" is assigned to Country "United States" and has site number "300000"
    And The following Software Versions are assigned to study Yprime-Sandbox-e2e
         | Version   |
         | 5.4.0.161 |       
    And I am on Software Release Management page
    And Initial Release is displayed in grid
        | Release Date | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
        | 01-Jan-01    | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 5/5                      | 5/5                        |

@ID:3245
#1
Scenario: Verify that the device field is populated when no devices are assigned to a site. 
    When I click on "Device Type" multi-select dropdown 
    Then "Phone" value is displayed in "Device Type" multi-select dropdown
    And "Tablet" value is displayed in "Device Type" multi-select dropdown 

