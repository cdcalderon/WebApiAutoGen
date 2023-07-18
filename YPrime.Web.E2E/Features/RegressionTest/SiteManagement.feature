Feature: Site Management Regression
	Features for testing site management functionality

@Regression
@SiteTest
@MockStudyBuilder
#NLA299
#SMR.04
Scenario: New site is successfully created in the portal
        Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And I click on "Sites" link on top navigation bar
        And I am on "Site Management" page
        And "Add New Site" button is enabled
        And I click on "Add New Site" button
        And I am on "Site Details" page
        And following values are associated as
            | Label           | Value |
            | Name            | blank |
            | Site Number     | blank |
            | Primary Contact | blank |
            | Investigator    | blank |
            | Address1        | blank |
            | City            | blank |
            | State           | blank |
            | Zip             | blank |
            | Country         | Select a Country |
            | Phone Number    | blank |
            | TimeZone        | Select a Time Zone |
        And I enter the following data
            | Label           | Value                                  | FieldType    |
            | Name            | Test Site                              | Inputtextbox |
            | Site Number     | 99999                                  | Inputtextbox |
            | Primary Contact | Tester                                 | Inputtextbox |
            | Investigator    | Tester                                 | Inputtextbox |
            | Address1        | (Current Date)                         | Inputtextbox |
            | City            | Malvern                                | Inputtextbox |
            | State           | Pennsylvania                           | Inputtextbox |
            | Zip             | 12345                                  | Inputtextbox |
            | Phone Number    | 9999999                                | Inputtextbox |
        And I click on "Country" dropdown
        And I select "United States" from "Country" dropdown
        And I click on "TimeZone" dropdown
        And I select "(UTC-05:00) Eastern Time (US & Canada)" from "TimeZone" dropdown
        And "Active" togglebutton is disabled
        And I click on "Active" togglebutton
        And I click on "Site Languages" button
        And I am on "Site Details" page
        And "en-US" togglebutton is disabled
        And I click on "en-US" togglebutton
        And "Next" button is enabled
        And I click on "Next" button
        And I am on "Site Details" page
        When I click on "Save" button
        Then I am on "Confirmation of Site Management" page