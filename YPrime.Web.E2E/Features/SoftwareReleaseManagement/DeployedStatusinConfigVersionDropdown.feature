@ignore @manual
Feature: Configuration version with "Deployed" status is displayed
    PBI 102607

    Background:
        Given I have logged in with user "PortalE2EUser"
        And I am on Software Release Management page
        And Configuration Version "7.0-7.0" is in "Deployed" status
        And Configuration Version "6.0-6.0" is in "Processing" status
        And Configuration Version "5.0-5.0" is in "Failed" status


    #1
    Scenario Outline: Verify that only configuration version with "Deployed" status is displayed in Configuration Version dropdown
        When I click on "Configuration Version" dropdown
        Then "<Configuration Version>" is "<Displayed State>" in the "Configuration Version" dropdown

        Examples:
            | Configuration Version | Displayed State |
            | 7.0-7.0               | Displayed       |
            | 6.0-6.0               | Not Displayed   |
            | 5.0-5.0               | Not Displayed   |

