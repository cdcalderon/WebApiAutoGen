@ignore @manual
Feature: BYOD Enrollment Information Button
    PBI 90411

    Background:
        Given I have study configured using "yprime_e2e_subject_6digit_pin_byod"
        And "S-10002-005" is assigned to "Site 4"
        And I have logged in with user "PortalE2EUser"
        And I am on "At Glance" Page
        And I click on "Subject link" on the top navigation bar
        And I click on "All Sites dropdown"
        And I select "Site 4" from "All Sites dropdown"
        And I select "Subject S-10002-005"
        And I am on "Subject Management" screen

    #Config 1
    #BYOD Enabled is enabled
    #Can create a patient bring your own device code is disabled

    #Config 2
    #BYOD Enabled is disabled
    #Can create a patient bring your own device code is enabled

    #Config 3
    #BYOD Enabled is enabled
    #Can create a patient bring your own device code is enabled

    #Config 4
    #BYOD Enabled is disabled
    #Can create a patient bring your own device code is disabled

    #1
    Scenario Outline: Verify that BYOD Enrollment Information button is displayed when configuration and persmission is set to true.
        Given I have configured a study using "<Configuration>"
        When  I am on "Subject Management" screen
        Then "BYOD Enrollment Information" button is "<Visible>"

        Examples:
            | Configuration | Visible |
            | Config #1     | No      |
            | Config #2     | No      |
            | Config #3     | Yes     |
            | Config #4     | No      |

    #2
    Scenario: Verify that Confirmation popup is displayed when user clicks on BYOD Enrollment Information button and the same Enrollment ID is displayed each time the user selects this button.
        Given I have configured a study using "Config 3"
        And I am on "Subject Management" screen
        And I click on "BYOD Enrollment Information" button
        And "Confirmation" popup is displayed
        And capture the "<EnrollmentID>"
        #Query the DB for a unused ID. Store that ID in a variable for use in the next step
        And I click on "X" icon on the "Confirmation" popup
        And "Confirmation" popup is dismissed
        When I click on "BYOD Enrollmemt Information" button
        Then "Confirmation" popup is displayed
        And "EnrollmentID" displayed in "Confirmation" popup is "<EnrollmentID>"


















