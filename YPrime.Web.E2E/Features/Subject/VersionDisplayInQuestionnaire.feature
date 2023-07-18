@ignore @manual
Feature: Version Display In Questionnaire
    PBI # 87864

    #1
    Scenario: Version is displayed on the Questionanire page.
        Given I have a study configured using "yprime_e2e"
        And "Daily Dairy Symptoms" questionnaire is completed with this "4.1.0.95-1.1-3.1"
        And I have logged in with user "PortalE2EUser"
        And I am on "At Glance" Page
        And I click on "Subject Tab" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And I click on "Subject 10001-001"
        And I am on "Subject 10001-001" page
        And I click on "Questionnaire tab"
        And I am on "All Questionnaire" page
        And "Daily Dairy Symptoms" questionnaire is displayed
        When I click "Daily Dairy Symptoms" questionnaire
        Then "4.1.0.95-1.1-3.1" is displayed for "Version" field

    #2
    Scenario: Config Version is displayed on Questionnaire page if user completes the questionnaire through Paper DCF
        Given I have a study configured using "yprime_e2e"
        And "Daily Questionnaire" questionnaire is completed with this "1.1-3.1"
        And I am logged in as "PortalE2EUser"
        And I am on "At Glance" Page
        And I click on "Subject Tab" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Initial Site"
        And I click on "Subject 10001-001"
        And I am on "Subject 10001-001" page
        And I click on "Questionnaire tab"
        And I am on "All Questionnaire" page
        And "Daily Dairy Symptoms" questionnaire is displayed
        When I click "Daily Dairy Symptoms" questionnaire
        Then "1.1-3.1" is displayed for "Version" field




