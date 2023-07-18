Feature: Remove Add New Role Buttton
    PBI 93483

    Background:
        Given I have logged in with user "PortalE2EUser"
        And I am on Role Management page

    #1
    Scenario: To verify that appropriate field and name is displayed.        
        Then the following data is displayed in the grid
            | Name         | Last Update | Set Permissions        | Set Subscriptions        | Set Reports        |
            | YPrime       | Not Null    | Set Permissions button | Set Subscriptions button | Set Reports button |

    #2
    Scenario: To verify the role title in the permission, subscriptions, and report page is displayed correctly        
        When I click "Set Permissions" button for row "YPrime"
        And I am on "Manage Role Permissions" page
        Then "YPrime" text is displayed on manage role page

    #3
    Scenario: To verify the role shows up in the portal (user icon)        
        Given I am on At a Glance page
        When I click on the user icon
        Then "YPrime" text is displayed in user info menu

