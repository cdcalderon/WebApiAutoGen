Feature: Version Mgmt: Version Display tied to Questonnaire
    PBI 87864


    Background:
        Given I have logged in with user "PortalE2EUser"
        And "Tablet" Device "Yp-MG000207" is assigned to Site "Initial Site"
        And Subject "S-10001-004" is assigned to "Yp-MG000207" Device
        And Subject "S-10001-004" has completed "Questionnaire Forms" questionnaire for question "How would you rate your health in general?" and choice "Fair"

    #1
    Scenario: Upon completion and successful sync of a questionnaire Software And Config Version should be displayed in questionnaire management page
        Given I am on "At a Glance" page
        And I click on the Subject menu item
        And I select Subject "S-10001-004"
        And I am on Subject "S-10001-004" page
        And I click on subject "Questionnaires" tab
        When I click on "Questionnaire Forms" diary entry
        Then I am on "Diary Entry Details" page
        And "0.0.0.1-20.0-01.15" is displayed in "Version" details field
        And following data is displayed in the data field table
            | Label                                      | Value       | Fieldtype |
            | How would you rate your health in general? | Fair        | text      |
