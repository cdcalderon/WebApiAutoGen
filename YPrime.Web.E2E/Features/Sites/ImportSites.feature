@Sites
Feature: ImportSites
#NLA-139 Import Sites feature  


Background: 
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And User "PortalE2EUser" with role "YP" has access to site "10000"
    And "CAN IMPORT SITES." permission is "Enabled"

@MockStudyBuilder
#1
Scenario: Verify Import Site Link is not visible when permission is disabled
Given "CAN IMPORT SITES." permission is "Disabled"
    And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    When I click on "Manage Study" link on top navigation bar
    Then "Import Sites" link is "NOT VISIBLE"

@MockStudyBuilder
#2
#ISI.01,ISI.02,ISI.03,ISI.04
Scenario: Verify layout of Import page when accessed and Site import file download functionality
Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And "Import Sites" link is "VISIBLE"
    When I click on "Import Sites" link
    Then I am on "Site Import" page
    And following labels are displayed
    | Label       |
    | Delimiter   |
    | Extension   |
    | Template    |
    | Import File |
    And I click on "Delimiter" dropdown
    #In below step i have to validate | character which is not possible so i am using \| to escape | with \ 
    And the following choices are displayed in "Delimiter" dropdown
		| Value |
		| ,     |
		|  \|   |
    And I select "," from "Delimiter" dropdown on site import page
    And I click on "Extension" dropdown
    And the following choices are displayed in "Extension" dropdown
		| Value  |
		|  .csv  |
		|  .txt  |
    And I select ".csv" from "Extension" dropdown on site import page
    And I click on "download" button to generate "SiteImport" in ".csv" format file to save in Export Evidence folder
    And I click on "Delimiter" dropdown 
    And I select "|" from "Delimiter" dropdown on site import page
    And I click on "Extension" dropdown
    And I select ".txt" from "Extension" dropdown on site import page
    And I click on "download" button to generate "SiteImport" in ".txt" format file to save in Export Evidence folder 


@MockStudyBuilder
#3
#ISI.05,ISI.06,ISI.08
#Select File’ à Will open File Explorer and allows user to specify path of the file that’s being imported.
#Import success will add sites in system for user to proceed with. Dbo.Sites
Scenario Outline: Verify import functionality with correct data set
Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Import Sites" link
    And I am on "Site Import" page
    And I click on "Delimiter" dropdown 
    And I select "<Delimiter>" from "Delimiter" dropdown on site import page
    And I click on "Extension" dropdown
    And I select "<FileFormat>" from "Extension" dropdown on site import page
    When I upload a file with file name "<FileName>"
    Then "Validate Import" button is enabled
    And I click on "Validate Import" button
    And "<Message>" text is displayed
    And "Import" button is enabled
    And I click on "Import" button
    And "Info" popup is displayed with message "<PopUp Message>"
    And I click on "Ok" button in the popup
    And "<SiteNumber>" text is displayed
    And "<SiteName>" text is displayed
    And the following text is displayed on the "Site Import" page
        | Label            | Value               |
        | Address1         | ABC                 |
        | Address2         | EFG                 |
        | Address3         | IJK                 |
        | CountryName      | India               |
        | State            | XY                  |
        | City             | BN                  |
        | Zip              | 12                  |
        | TimeZone         | UTC                 |
        | PhoneNumber      | 1234567890          |
        | FaxNumber        | 5423787             |
        | PrimaryContact   | ypadmin             |
        | IsActive         | True                |
        | Notes            | abcv                |
        | Investigator     | Investigator        |
        | AllowedLanguages | en-US               |
    And I click on "Sites" link on top navigation bar
    And I am on "Site Management" page
    And "<SiteNumber>" text is displayed
          
	Examples: 
     | Delimiter | FileFormat | FileName               | Message                                                                                            | PopUp Message                   | SiteNumber | SiteName |
     | ,         | .csv       | SiteImport_CSVFile.csv | Succesfully validated import file with 0 Import Validation Errors & 0 Import Validation Warning(s) | Sucessfully imported 1 site(s). | 10006      |  Site6   |
     | \|        | .txt       | SiteImport_TxtFile.txt | Succesfully validated import file with 0 Import Validation Errors & 1 Import Validation Warning(s) | Sucessfully imported 1 site(s). | 10005      |  Site5   |
         

@MockStudyBuilder
#4
#ISI.07
Scenario Outline: Verify file import functionality with incorrect/empty data set 
Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Import Sites" link
    And I am on "Site Import" page
    And I click on "Delimiter" dropdown
    And I select "<Delimiter>" from "Delimiter" dropdown on site import page
    And I click on "Extension" dropdown
    And I select "<FileFormat>" from "Extension" dropdown on site import page
    When I upload a file with file name "<FileName>"
    Then "Validate Import" button is enabled
    And I click on "Validate Import" button
    And "<Message>" text is displayed
    And "Import" button is disabled

	Examples: 
     | Delimiter | FileFormat | FileName                        | Message                                                |
     | ,         | .csv       | SiteImport_CSVBlankFile.csv     | Unable to process empty file.                          |
     | \|        | .txt       | SiteImport_TxtBlankFile.txt     | Unable to process empty file.                          |
     | ,         | .csv       | SiteImport_ExistingDataCSV.csv  | The site number '10000' is already in use              |
     | \|        | .txt       | SiteImport_ExistingDataTxt.txt  | The site number '10000' is already in use              |
     | ,         | .csv       | SiteImport_IncorrectCSVFile.csv | Unable to find a language code with the name 'English' |
     | \|        | .txt       | SiteImport_IncorrectTxtFile.txt | Unable to find a language code with the name 'English' |
     

@MockStudyBuilder
#5
#ISI.07
Scenario Outline: Verify import functionality with some empty columns 
Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Import Sites" link
    And I am on "Site Import" page
    And I click on "Delimiter" dropdown
    And I select "<Delimiter>" from "Delimiter" dropdown on site import page
    And I click on "Extension" dropdown
    And I select "<FileFormat>" from "Extension" dropdown on site import page
    When I upload a file with file name "<FileName>"
    Then "Validate Import" button is enabled
    And I click on "Validate Import" button
    And the following message is displayed on the page
    | Messages                                                                                                                      |
    | 10 Import Validation Error(s)                                                                                                 |
    | Validation Failed for Property: '[SiteNumber, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''     |
    | Validation Failed for Property: '[Name, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''           |
    | Validation Failed for Property: '[Address1, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''       |
    | Validation Failed for Property: '[State, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''          |
    | Validation Failed for Property: '[City, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''           |
    | Validation Failed for Property: '[Zip, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''            |
    | Validation Failed for Property: '[PhoneNumber, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''    |
    | Validation Failed for Property: '[PrimaryContact, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: '' |
    | Validation Failed for Property: '[IsActive, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''       |
    | Validation Failed for Property: '[Investigator, ]' in Row 2, Error: 'The RuntimePropertyInfo field is required.', Value: ''   |
    And "Import" button is disabled

    Examples: 
     | Delimiter | FileFormat | FileName                        |                               
     | ,         | .csv       | SiteImport_PartialDataCSV.csv   |                        
     | \|        | .txt       | SiteImport_PartialDataTxt.txt   |                         
     