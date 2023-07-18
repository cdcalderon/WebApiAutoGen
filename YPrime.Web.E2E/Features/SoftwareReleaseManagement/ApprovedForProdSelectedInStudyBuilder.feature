@ignore @manual
Feature: Approved For Prod Selected In StudyBuilder
    PBI 87838 Software Release Management Page When Approved for prod is selected

    Background:
        Given I have a study configured using "yprime_e2e_Prod"
        And "Approved for Prod togglebutton" for "1.2-3.1" is enabled
        And "Approved for Prod togglebutton" for "1.0-3.0" is disabled
        And I am on Software Release Management page

    #1
    Scenario: When Approved for Prod toggle is enabled then the configuration versions is displayed
        When I click on "Configuration Version dropdown"
        And I click on "Configuration Version dropdown"
        Then "1.2-3.1" is displayed
        And "1.0-3.0" is not displayed