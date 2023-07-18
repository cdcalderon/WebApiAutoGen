@ignore @manual
Feature: Activate Visit Button
    PBI 86411

    #Config1
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit

    #Config2
    #Handheld Visit Activation is disabled.
    #Can Activate Visits in Portal is disabled

    #Config3
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is disabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit

    #Config4
    #Handheld Visit Activation is disabled
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit

    #Config5
    #Handheld Visit Activation is enabled
    #Can Activate Visits in Portal is enabled

    #config6
    #Handheld Visit Activation is enabled
    #Can Activate Visits in Portal is enabled
    #Treatment Visit 6 is created
    #Clinician Questionnaire is assigned to Treatment Visit 6

    #1
    Scenario Outline: Verify that Activate Visit Button is displayed based on permissions and study configuration
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        When I click on "Visit Tab"
        Then "Activate Visit Button" is "<Visible>"
            | Configuration | Visible |
            | Config1       | Yes     |
            | Config2       | No      |
            | Config3       | No      |
            | COnfig4       | No      |
            | Config5       | No      |
            | Config6       | No      |

    #Config7
    #Handheld Visit Activation is enabled
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #BusinessRule "Information Comparison - Visit Visibility" is assigned to Screening Visit
    #BusinessRule "Information Comparison - Visit Visibility" is set to True

    #Config8
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #BusinessRule "Information Comparison - Visit Visibility" is assigned to Screening Visit
    #BusinessRule "Information Comparison - Visit Visibility" is set to False

    #2
    Scenario Outline: Verify that Activate Visit button is displayed when visit visibility business rule is true and not displayed when vist visiblity business rule is false
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And "S-10001-001" is in "<SubjectStatus>" status
        And "BusinessRule" is "<BusinessRule>"
        When I click on "Visit Tab"
        Then "Activate Visit Button" is "<Visible>"
            | Configuration | SubjectStatus | BusinessRule | Visible |
            | Config7       | Enrolled      | True         | Yes     |
            | Config7       | Screened      | True         | No      |
            | Config8       | Enrolled      | False        | No      |
            | Config8       | Screened      | False        | Yes     |

    #Config9
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Enrollment Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #Subject Questionnaire is assigned to Enrollment Visit
    #BusinessRule "Information Comparison - Visit enabled" is assigned to Enrollment Visit
    #BusinessRule "Information Comparison - Visit enabled" is True


    #Config10
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Enrollment Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #Subject Questionnaire is assigned to Enrollment Visit
    #BusinessRule "Information Comparison - Visit enabled" is assigned to Enrollment Visit
    #BusinessRule "Information Comparison - Visit enabled" is False

    #3
    Scenario Outline: Verify that Activate Visit button is displayed when visit enabled business rule is true and not displayed when visit visible business rule is false.
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And subject "S-10001-001" is "<Gender>"
        And "BusinessRule" is "<BusinessRule>"
        When I click on "Visit Tab"
        Then "Activate Visit Button" is "<Enabled>"
            | Configuration | Gender | BusinessRule | Enabled |
            | Config9       | Male   | True         | No      |
            | Config9       | Female | True         | Yes     |
            | Config10      | Male   | False        | Yes     |
            | Config10      | Female | False        | No      |

    #Config11
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Treatment visit 1 is created
    #Subject Questionnaire is assigned to Treatment visit 1

    #Treatment visit 2 is created
    #Subject Questionnaire is assigned to Treatment vist 2

    #Treatment visit 3 is created
    #Subject Questionnaire is assigned to Treatment vist 3

    #Treatment visit 4 is created
    #Subject Questionnaireis assigned to Treatment vist 4

    #Treatment visit 5 is created
    #Subject Questionnaireis assigned to Treatment vist 5

    #4
    Scenario Outline: Verify that Activate Visit button is displayed based on Visit statuses.
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        When I click on "Visit Tab"
        And "Visit" is in "<VisitStatus>"
        Then "Activate Visit Button" is "<Visible>"
            | Configuration | Visit             | VisitStatus | Visible |
            | Config11      | Treatment Visit 1 | Not Started | Yes     |
            | Config11      | Treatment Visit 2 | In Progress | Yes     |
            | Config11      | Treatment Visit 3 | Partial     | No      |
            | Config11      | Treatment Visit 4 | Completed   | No      |
            | Config11      | Treatment Visit 5 | Missed      | No      |


    #Config12
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #Value 0 in Open Days After field in Screening Visit
    #Soft Stop in Visit Order Validation Field

    #Enrollment Visit is created
    #Value 1 in Open Days After field in Enrollment Visit
    #Hard Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Screening Visit

    #Treatment Visit 1 is created
    #Value 1 in Open Days After field in Treatment Visit 1
    #Soft Stop in Visit Order Validation field
    #Enrollment Visit in Please select field
    #Value 2 in Close Days After field in Treatment Visit 1
    #Subject Questionnaire is assigned to Treatment visit 1

    #Treatment Visit 2 is created
    #Value 0 in Open Days After field in Treatment Visit 2
    #No Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Treatment vist 2

    #5
    Scenario Outline: Verify that warning messages are displayed If visit selected is out of window and prior visits are not fully completed.
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "<Visit>" is displayed
        And "<Visits>" is in "<Visit Status>"
        When I click on "Activate Visit Button" near "<Visit>"
        Then "<popuptype>" is displayed
        And "<message>" is displayed
        And "<actionbuttons>" is displayed
            | Configuration | Visit             | Visit Status | popuptype | message                                                                                                                                                                                                                                   | actionbuttons |
            | Config12      | Screening Visit   | Not Started  |           |                                                                                                                                                                                                                                           |               |
            | Config12      | Enrollment Visit  | Not Started  | Warning   | Visit is out of window.                                                                                                                                                                                                                   | Ok            |
            | Config12      | Treatment Visit 1 | Not started  | Warning   | The visit is out of window and there are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
            | Config12      | Treatment Visit 2 | Not started  | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed?                                | Yes/No        |

    #Config13
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled

    #Treatment Visit 3 is created
    #Value 1 in Open Days After field in Treatment Visit 3
    #Hard Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Treatment vist 3

    #Treatment Visit 4 is created
    #Value 1 in Open Days After field in Treatment Visit 4
    #Treatment Visit 2 in Please select field
    #Value 2 in Close Days After field in Treatment Visit 4
    #Soft Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Treatment vist 4

    #Treatment Visit 5 is created
    #Value 1 in Open Days After field in Treatment Visit 5
    #No Stop in Visit Order Validation field

    #6
    Scenario: Verify that Hard stop warning message is displayed If visit selected is out of window and previous visit is completed.
        Given I have configured a study using "Config13"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Treatment Visit 3" is displayed
        And "Treatment Visit 2" is in "Completed" visit status
        And "Treatment Visit 3" is in "Not Started" visit status
        When I click on "Activate Visit Button" near "Treatmeent Visit 3"
        Then a pop up is displayed
            | popupType | message                 | actionbuttons |
            | Warning   | Visit is out of window. | Ok            |


    #7
    Scenario: Verify that Soft stop warning message is displayed If visit selected is out of window and previous visit is completed.
        Given I have configured a study using "Config13"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Treatment Visit 4" is displayed
        And "Treatment Visit 2" is in "Completed" visit status
        And "Treatment Visit 3" is in "Completed" visit status
        And "Treatment Visit 4" is in "Not Started" visit status
        When I click on "Activate Visit Button" near "Treatment Visit 4"
        Then a pop up is displayed
            | popuptype | message                                                 | actionbuttons |
            | Warning   | This visit is out of window. Would you like to proceed? | Yes/No        |


    #8
    Scenario: Verify that no stop warning message is displayed If visit selected is out of window and previous visit is completed.
        Given I have configured a study using "Config13"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "<Visit>" is displayed
        And "Treatment Visit 5" is displayed
        And "Treatment Visit 2" is in "Completed" visit status
        And "Treatment Visit 3" is in "Completed" visit status
        And "Treatment Visit 4" is in "Completed" visit status
        And "Treatment Visit 5" is in "Not Started" visit status
        When I click on "Activate Visit Button" near "Treatment Visit 5"
        Then a pop is displayed
            | popuptype | message | actionbuttons |
            |           |         |               |


    #Config14
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled

    #Treatment Visit 6 is created
    #Subject Questionnaire is assigned to Treatment Visit 6
    #Value 0 in Open Days After field in Treatment Visit 6
    #Hard Stop in Visit Order Validation field

    #Treatment Visit 7 is created
    #Subject Questionnaire is assigned to Treatment Visit 7
    #Value 0 in Open days After field in Treatment Visit 7
    #Soft Stop in Visit Order Validation field

    #Treatment Visit 8 is created
    #Subject Questionnaire is assigned to Treatment Visit 8
    #Value 0 in Open days After field in Treatment Visit 8
    #No Stop in Visit Order Validation field

    #9
    Scenario Outline: Verify that warning messages are displayed If the visit selected is in window and previous visits not being completed.
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "<Visit>" is displayed
        And "Treatment Visit 5" is in "Not Started" visit status
        And "<Visit>" is in "<VisitStatus>"
        When I click on "Activate Visit Button" near "<Visit>"
        Then "<popuptype>" is displayed
        And "<message>" is displayed
        And "<actionbuttons>" is displayed
            | Configuration | Visit             | VisitStatus | popuptype | message                                                                                                                                                                                                    | actionbuttons |
            | Config14      | Treatment Visit 6 | Not Started | Warning   | There are previous visits which have not been fully completed.                                                                                                                                             | OK            |
            | Config14      | Treatment Visit 7 | Not Started | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/ No       |
            | Config14      | Treatment Visit 8 | Not Started | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |

    #Config15
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled

    #Treatment Visit 8 is created
    #Subject Questionnaire is assigned to Treatment Visit 8
    #Value 1 in Open Days After field in Treatment Visit 8
    #Treatment Visit 8 visit date is 1 day ago
    #Hard Stop in Visit Order Validation field

    #Treatment Visit 9 is created
    #Subject Questionnaire is assigned to Treatment Visit 9
    #Value 1 in Open days After field in Treatment Visit 9
    #Treatment Visit 9 visit date is 1 day ago
    #Soft Stop in Visit Order Validation field

    #Treatment Visit 10 is created
    #Subject Questionnaire is assigned to Treatment Visit 10
    #Value 1 in Open days After field in Treatment Visit 10
    #Treatment Visit 10 visit date is 1 day ago
    #No Stop in Visit Order Validation field

    #10
    Scenario Outline: Verfiy that no warning messages are displayed If visit selected is in window and visit is available.
        Given I have configured a study using "<Configuration>"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "<Visit>" is displayed
        And "<Visit>" is in "<VisitStatus>"
        When I click on "Activate Visit Button" near "<Visit>"
        Then "<popuptype>" is displayed
        And "<message>" is displayed
        And "<actionbuttons>" is displayed
            | Configuration | Visit              | VisitStatus | popuptype | message | actionbuttons |
            | Config15      | Treatment Visit 8  | Not Started |           |         |               |
            | Config15      | Treatment Visit 9  | Not Started |           |         |               |
            | Config15      | Treatment Visit 10 | Not Started |           |         |               |

    #Config16
    #Handheld Visit Activation is enabled.
    #Can Activate Visits in Portal is enabled
    #Screening Visit is created
    #Subject Questionnaire is assigned to Screening Visit
    #Subject Questionnaire 2 is assigned to Screening Visit
    #Value 0 in Open Days After field in Screening Visit
    #Soft Stop in Visit Order Validation Field

    #Enrollment Visit is created
    #Value 1 in Open Days After field in Enrollment Visit
    #Hard Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Screening Visit

    #Treatment Visit 1 is created
    #Value 1 in Open Days After field in Treatment Visit 1
    #Soft Stop in Visit Order Validation field
    #Enrollment Visit in Please select field
    #Value 2 in Close Days After field in Treatment Visit 1
    #Subject Questionnaire is assigned to Treatment visit 1

    #Treatment Visit 2 is created
    #Value 0 in Open Days After field in Treatment Visit 2
    #No Stop in Visit Order Validation field
    #Subject Questionnaire is assigned to Treatment vist 2

    #11
    Scenario: Verify that warning messages are displayed if the user selects Yes on Soft Stop warning message.
        Given I have configured a study using "Config14"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Screening Visit" is in "Not Started" visit status
        And "Enrollment Visit" is displayed
        And "Enrollment Visit" is in "Not Started" visit status
        And I click on "Activate Visit Button" near "Enrollment Visit"
        And a pop up is displayed
            | popupType | Message                                                                                                                                                                                                                                   | ActionsButton |
            | Warning   | The visit is out of window and there are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And I click on "Yes button"
        And pop up is displayed
            | popupType | Message                                                                                                                                          | ActionsButton |
            | Warning   | Visit {Enrollment Visit} is now active, please contact your subject and request that they sync data in order to view their Visit Questionnaires. | Ok            |
        When I click on "Ok button"
        Then a pop up is not displayed
        And I am on "Visit tab" page
        And "Activate Visit button" is active
        And "Screening Visit" is in "Missed" visit status

    #12
    Scenario: Verify that warning message is displayed if the user selects Yes on No stop warning message.
        Given I have configured a study using "Config14"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Enrollment Visit" is in "In Progress" visit status
        And "Treatment Visit 1" is displayed
        And "Treatment Visit 1" is in "Not Started" visit status
        And I click on "Activate Visit Button" near "Treatment Visit 1"
        And a pop up is displayed
            | popupType | Message                                                                                                                                                                                                    | ActionsButton |
            | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And I click on "Yes button"
        And pop up is displayed
            | popupType | Message                                                                                                                                           | ActionsButton |
            | Warning   | Visit {Treatment Visit 1} is now active, please contact your subject and request that they sync data in order to view their Visit Questionnaires. | Ok            |
        When I click on "Ok button"
        Then a pop up is not displayed
        And I am on "Visit tab" page
        And "Activate Visit button" is active
        And "Enrollment Visit" is in "Partial" visit status

    #13
    Scenario: Verify that warning message is not displayed if the user selects No on Soft stop warning message.
        Given I have configured a study using "Config14"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Screening Visit" is in "In Progress" visit status
        And "Enrollment Visit" is displayed
        And "Enrollment Visit" is in "Not Started" visit status
        And I click on "Activate Visit Button" near "Enrollment Visit"
        And a pop up is displayed
            | popupType | Message                                                                                                                                                                                                                                   | ActionsButton |
            | Warning   | The visit is out of window and there are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And I click on "No button"
        And pop up is not displayed
            | popupType | Message                                                                                                                                           | ActionsButton |
            | Warning   | Visit {Treatment Visit 1} is now active, please contact your subject and request that they sync data in order to view their Visit Questionnaires. | Ok            |
        And I am on "Visit tab" page
        And "Activate Visit button" is active
        When I click on "Activate Visit Button" near "Enrollment Visit"
        Then a pop up is displayed
            | popupType | Message                                                                                                                                                                                                                                   | ActionsButton |
            | Warning   | The visit is out of window and there are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And "Screening Visit" is in "In Progress" visit status

    #14
    Scenario: Verify that warning message is not displayed if the user selects No on No stop warning message.
        Given I have configured a study using "Config14"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Screening Visit" is in "In Progress" visit status
        And "Treatment Visit 1" is displayed
        And "Treatment Visit 1" is in "Not Started" visit status
        And I click on "Activate Visit Button" near "Treatment Visit 1"
        And a pop up is displayed
            | popupType | Message                                                                                                                                                                                                    | ActionsButton |
            | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And I click on "No button"
        And pop up is not displayed
        And I am on "Visit tab" page
        And "Activate Visit button" is active
        When I click on "Activate Visit Button" near "Treatment Visit 1"
        Then a pop up is displayed
            | popupType | Message                                                                                                                                                                                                    | ActionsButton |
            | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And "Screening Visit" is in "In Progress" visit status

    #15
    Scenario: Verify that Pop up is displayed if user clicks Activate Visit button after it has already been activated
        Given I have configured a study using "Config14"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        And I click on "Visit Tab"
        And "Enrollment Visit" is in "Not Started" visit status
        And "Treatment Visit 1" is displayed
        And "Treatment Visit 1" is in "Not Started" visit status
        And I click on "Activate Visit Button" near "Treatment Visit 1"
        And a pop up is displayed
            | popupType | Message                                                                                                                                                                                                    | ActionsButton |
            | Warning   | There are previous visits which have not been fully completed. If you continue with completing this visit, all previously unsaved forms will become unavailable for completion. Would you like to proceed? | Yes/No        |
        And I click on "Yes button"
        And pop up is displayed
            | popupType | Message                                                                                                                                           | ActionsButton |
            | Warning   | Visit {Treatment Visit 1} is now active, please contact your subject and request that they sync data in order to view their Visit Questionnaires. | Ok            |
        And I click on "Ok button"
        And I am on "Visit tab" page
        When I click on "Activate Visit Button" near "Treatment Visit 1"
        Then a pop up is displayed
            | popupType | Message                                    | ActionsButton |
            |           | Subject forms have already been activated. | Ok            |

    #config17
    #Handheld Visit Activation is enabled
    #Can Activate Visits in Portal is enabled
    #Treatment Visit 9 is created
    #Caregiver Questionnaire is assigned to Treatment Visit 9

    #16
    Scenario: Verify that Activate Visit Button is not displayed when caregiver questionnaire is assigned to a visit
        Given I have configured a study using "Config17"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        When I click on "Visit Tab"
        Then "Activate Visit Button" is not "Visible"


    #config18
    #Handheld Visit Activation is enabled
    #Can Activate Visits in Portal is enabled
    #Treatment Visit 10 is created
    #Caregiver Questionnaire is assigned to Treatment Visit 10

    #17 @Provisioned @Handheld
    Scenario: Verify that Activate Visit Button is displayed when caregiver questionnaire is assigned to a visit
        Given I have configured a study using "Config18"
        And I am logged in as "PortalE2EUser"
        And I select subject "S-10001-001"
        When I click on "Visit Tab"
        Then "Activate Visit Button" is "Visible"
