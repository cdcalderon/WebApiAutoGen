@DataGridSiteManagement
Feature: DataGridSiteManagement

Verify the Data grid functionality for Site Management

Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Language "en-US" is assigned to "Site 1"
    And User "PortalE2EUser" with role "YP" has access to site "10000"

#1    
@MockStudyBuilder
#SMR.02, .05, .06
Scenario: Validate for Data grid values, link text and Site Details page
    Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And "10000" SiteNumber has isActive set to "False"
    And "10001" SiteNumber has isActive set to "False"
	And I click on "Sites" link on the top navigation bar
	And the data grid values given under column name is displayed as
        | Site Number | Name         | Address  | City      | Country       | Primary Contact | Phone Number | Active |
        | 10000       | Site 1       | test Dr. | Malvern   | United States | ypadmin         | 123-456-7890 | False  |
        | 10001       | Initial Site | test Dr. | Malvern   | United States | ypadmin         | 123-456-7890 | False  |
    And I click on "10000" link
    When I am now on "Site Details" page
    Then the following fields are enabled
        | Label           | FieldType    |
        | Name            | Inputtextbox |
        | Site Number     | Inputtextbox |
        | Primary Contact | Inputtextbox |
        | Investigator    | Inputtextbox |
        | Address1        | Inputtextbox |
        | Address2        | Inputtextbox |
        | Address3        | Inputtextbox |
        | Fax Number      | Inputtextbox |
        | City            | Inputtextbox |
        | State           | Inputtextbox |
        | Zip             | Inputtextbox |
        | Country         | Dropdown     |
        | Phone Number    | Inputtextbox |
        | TimeZone        | Dropdown     |
        | Active          | Toggle       |
    And "WebBackupEnabled" togglebutton is disabled
    And following values are associated as
        | Label           | Value                                  |
        | Name            | Site 1                                 |
        | Site Number     | 10000                                  |
        | Primary Contact | ypadmin                                |
        | Investigator    | Investigator Site 1                    |  
        | Address1        | test Dr.                               |
        | City            | Malvern                                |
        | State           | PA                                     |
        | Zip             | 19355                                  |
        | Country         | United States                          |
        | Phone Number    | 123-456-7890                           |
        | TimeZone        | (UTC-05:00) Eastern Time (US & Canada) |
    And I clear text field "Phone Number"
    And I enter "20002" in "Site Number" inputtextbox field
    And I click on "Next Language" button
    And I click on "Next" button
    And I click on "Save" button
    And "The Phone Number field is required." text is displayed in the page
    And I enter "123-456-7890" in "Phone Number" inputtextbox field
    And I click on "Next Language" button
    And I click on "en-UK" togglebutton
    And I click on "Next" button
    And I click on "Save" button
    And "The site details have been updated successfully." text is displayed in the page
    And I click on "Sites" link on the top navigation bar
    And "20002" text is displayed in the page
    And Site Number "20002" is updated in Sites table

#2
@MockStudyBuilder
#SMR.03
Scenario: User validates for Bulk Edit Sites enable
    Given "CAN SITE BULK UPDATE" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page 
    And "10000" SiteNumber has isActive set to "False"
    And I click on "Sites" link on the top navigation bar
    And "Bulk Edit Sites" button is displayed
    And "Bulk Edit Sites" button is enabled
    And I click on "Bulk Edit Sites" button
    And the following columns is displayed within Bulk Edit Site grid
        | ColumnName  |
        | Site Number |
        | Name        |
        | Active      |
    And the following columns have the field type within Bulk Edit Site grid as 
        | Label               | FieldType    |
        | Site Number input   | Inputtextbox |
        | Name input          | Inputtextbox |
        | Master Active toggle| Toggle       |
    When I click on "Master Active toggle" togglebutton
    And "InitalSite toggle" togglebutton is enabled
    And "Site1 toggle" togglebutton is enabled
    And I click on "Master Active toggle" togglebutton
    And "InitalSite toggle" togglebutton is disabled
    And "Site1 toggle" togglebutton is disabled
    And I click on "Save" button
    And "False" is updated for isActive in "Site 1" in Site table
    Then "The Site Management changes have been updated successfully. " is displayed in the page

#3
@MockStudyBuilder
#SMR.03, .04, .10
Scenario: User validates for Bulk Edit Disable and Add New site validations
    Given "CAN SITE BULK UPDATE" permission is "Disabled"
	And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	And I click on "Sites" link on the top navigation bar
    And "Bulk Edit Sites" button is "Not Visible"
    And "Add New Site" button is enabled
    And I click on "Add New Site" button
    And I am on "Site Details" page
    And I click on "Next Language" button
    And I am on "Site Details" page
    And "Next" button is enabled
    And I click on "Next" button
    And I am on "Site Details" page
    When I click on "Save" button
    Then "The Name field is required." is displayed in the page
    And "The Site Number field is required." is displayed in the page
    And "The Primary Contact field is required." is displayed in the page
    And "The Investigator field is required." is displayed in the page
    And "The Address1 field is required." is displayed in the page
    And "The City field is required." is displayed in the page
    And "The Zip field is required." is displayed in the page
    And "The Country field is required." is displayed in the page
    And "The TimeZone field is required." is displayed in the page
    And "The Phone Number field is required." is displayed in the page

#4
@MockStudyBuilder
#SMR.07
Scenario: User validates for all languages selection
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	And I click on "Sites" link on the top navigation bar
    And I click on "10000" link
    And I am now on "Site Details" page
    And I click on "Site Languages" tab
    And I click on "en-US" togglebutton
    And I click on "en-UK" togglebutton
    And I click on "Next" button
    When I click on "Save" button
    And "The site details have been updated successfully." text is displayed in the page
    And I click on "Subject" link on the top navigation bar
    And I click on "All Sites" dropdown
    And I select "Site 1" from "All Sites" dropdown
    And I click on "Add New Subject" button
    Then the following choices are displayed in "Language" dropdown
		| Value                   |
		| English (United States) |
        | English (United Kingdom)|

#5
@MockStudyBuilder
#SMR.08
Scenario: User validates one language selection
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	And I click on "Sites" link on the top navigation bar
    And I click on "10000" link
    And I am on "Site Details" page
    And I click on "Site Languages" tab
    And I click on "en-US" togglebutton
    And I click on "Next" button
    When I click on "Save" button
    And "The site details have been updated successfully." text is displayed in the page
    And I click on "Subject" link on the top navigation bar
    And I click on "All Sites" dropdown
    And I select "Site 1" from "All Sites" dropdown
    And I click on "Add New Subject" button
    Then the following choices are displayed in "Language" dropdown
		| Value                    |
		| English (United States)  |

#6
@MockStudyBuilder
#SMR.09
Scenario: User validates no language selection
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
	And I click on "Sites" link on the top navigation bar
    And I click on "10000" link
    And I am on "Site Details" page
    And I click on "Site Languages" tab
    And I click on "Next" button
    When I click on "Save" button
    Then "At least one language must be selected." is displayed in the page

