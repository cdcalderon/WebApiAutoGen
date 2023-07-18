
Feature: Software Release Management
NLA-333

#SRMAN001.05 SRMAN001.40 SRMAN001.50 SRMAN001.60 SRMAN001.70 SRMAN001.80 SRMAN001.100 SRMAN001.200
# Configuration versions assigned- 3.0-0.6 , 2.0-0.5 ,1.0-0.4,0.0-0.1
@MockStudyBuilder
Scenario: Verify dropdown Software Version,Configuration Version and only "Software Release Management" page is displayed when no configuration version is assigned
   Given No configuration is assigned to the study
    And I am logged in as "PortalE2EUser"
    And "Ability to View Software Release Page" permission is "Enabled" 
    And I navigate to "Software Release Management" page
    And I refresh page
    And Page title is "Software Release Management"
    And Top navigation bar is "Not Visible"
    And The following Software Versions are assigned to study
            | Version |
            | 7.0.0.0 |
            | 0.0.0.1 |   
    And I refresh page
    And "Release Name inputtextbox" has value ""
    And "Software Version" dropdown has placeholder "Please Select"
    And "Configuration Version" dropdown has placeholder "Please Select"
    And I click on "Software Version" dropdown
    And "Software Version" is sorted from the "latest" to the "oldest" 
    And I click on the background screen
    And I click on "Configuration Version" dropdown
    And "Configuration Version" is sorted from the "latest" to the "oldest" 
    When I click on "Create Release" button
    Then "Please enter a release name." validation message is displayed for "Release Name inputtextbox"
    And "Please select a software version." validation message is displayed for "Software Version" dropdown
    And "Please select a configuration version." validation message is displayed for "Configuration Version" dropdown
    And I enter "Test Release" for "Release Name inputtextbox"
    And I click on "Software Version" dropdown
    And I select "0.0.0.1" from "Software Version" dropdown
    And I click on "Configuration Version" dropdown
    And I select "2.0-0.5" from "Configuration Version" dropdown
    When I click on "Create Release" button
    And "Software release must be assigned to at least one device." message is displayed in "Review" PopUp
    And I click on "Cancel" button
    And "Required" togglebutton is disabled
    And I click on "Study Wide" togglebutton
    And "Country(s)" multi-select dropdown is disabled
    And "Site(s)" multi-select dropdown is disabled
    And "Device Type" dropdown is disabled
    And I click on "Create Release" button
    And a popup is displayed
    | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
    | Review Software and Configuration Release | 0 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.0 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
    When I click on "Confirm" button
    Then Top navigation bar is "Visible"
    And I am on "Software Release Management" page
    And "Test Release" is set as "Not Required" in db

#SRMAN001.05
@MockStudyBuilder
Scenario: Verify "403 Forbidden" is displayed when No configuration version is assigned and "Ability to View Software Release Page" permission is "Disabled"
    Given No configuration is assigned to the study
    And I am logged in as "PortalE2EUser"
    And "Ability to View Software Release Page" permission is "Disabled" 
    When I navigate to "Software Release Management" page
    And I refresh page
    Then Page title is "403 - Forbidden: Access is denied."

#SRMAN001.30 SRMAN001.110 SRMAN001.120 SRMAN001.130 SRMAN001.150
@MockStudyBuilder
Scenario: Verify "Ability to View Software Release Page" permission and Multi select filters(Configuration,Device and Site)
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Site "Site 2" is assigned to Country "India" and has site number "20000"
    And "Phone" Device "YP-E2E-Device-Phone" is assigned to Site "Site 1"
    And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 2"
    And User "PortalE2EUser" with role "YP" has access to site "10000"
    And I am logged in as "PortalE2EUser"
    And "Ability to View Software Release Page" permission is "Disabled"
    And I am on "At a Glance" page
    When I click on "Manage Study" link on top navigation bar
    Then "Software Release Management" link is "NOT VISIBLE"
    And I click on the user icon
    And I click on " Logout" button
    And I am logged in as "PortalE2EUser"
    And "Ability to View Software Release Page" permission is "ENABLED"
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And "Software Release Management" link is "VISIBLE"
    And I click on "Software Release Management" link
    And I am on "Software Release Management" page
    And I click on "Device Type" multi-select dropdown
    And I select "Phone" from "Device Type" multi-select dropdown
    And I click on "Device Type" multi-select dropdown
    And I select "Tablet" from "Device Type" multi-select dropdown
    And I click on "Country(s)" multi-select dropdown
    And I select "United States" from "Country(s)" multi-select dropdown
    And I click on "Country(s)" multi-select dropdown
    And I select "India" from "Country(s)" multi-select dropdown
    When I click on cross Icon for "Phone"
    Then "Phone" value is "not displayed" in "Device Type:" textBox
    And "United States" value is "not displayed" in "Country(s):" textBox
    And I click on "Device Type" multi-select dropdown
    And I select "Phone" from "Device Type" multi-select dropdown
    And I click on "Country(s)" multi-select dropdown
    And I select "United States" from "Country(s)" multi-select dropdown
    And I click on "Site(s)" multi-select dropdown
    And I select "Site 2" from "Site(s)" multi-select dropdown
    And I click on "Site(s)" multi-select dropdown
    And I select "Site 1" from "Site(s)" multi-select dropdown
    When I click on cross Icon for "United States" 
    Then "Site 1" value is "not displayed" in "Site(s):" textBox
    When I click on cross Icon for "Site 2"
    Then "Site 2" value is "not displayed" in "Site(s):" textBox
    
@MockStudyBuilder
#SRMAN001.80 SRMAN001.100 SRMAN001.130 SRMAN001.160 SRMAN001.170 SRMAN001.180 SRMAN001.200
Scenario: Verify Release Grid is displayed when release is created and release is deactivated when toggle button is disabled for release
    Given Version number "7.0.0.0" is added with Package path "https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_7.0.0.0.zip"
    And Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Site "Site 2" is assigned to Country "India" and has site number "20000"
    And Software Release "Test Release 01" has been created with Software Version "0.0.0.1" and Configuration Version "1.0-0.4"
    And "Phone" Device "YP-E2E-Device-Phone" is assigned to Site "Site 1"
    And "Phone" Device "YP-E2E-Device-Phone" is assigned to Software Release "Test Release 01"
    And I am logged in as "PortalE2EUser" 
    And I am on "At a Glance" page
    And I click on "Manage Study" link on top navigation bar
    And I click on "Software Release Management" link
    And Page title is "Software Release Management"
    And I enter "Test Release" for "Release Name inputtextbox"
    And I click on "Software Version" dropdown
    And I select "7.0.0.0" from "Software Version" dropdown
    And I click on "Configuration Version" dropdown
    And I select "2.0-0.5" from "Configuration Version" dropdown
    And I click on "Required" togglebutton
    And "Phone" value is "VISIBLE"
    And I click on "Country(s)" multi-select dropdown
    And I select "United States" from "Country(s)" multi-select dropdown
    And I click on "Site(s)" multi-select dropdown
    And I select "Site 1" from "Site(s)" multi-select dropdown
    And I click on "Create Release" button
    And a popup is displayed
    | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
    | Review Software and Configuration Release | 1 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.1 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
    When I click on "Cancel" button
    Then Page title is "Software Release Management"
    And "Test Release" is displayed for "Release Name inputtextbox"
    And "Software Version" value is "7.0.0.0"
    And "Required" togglebutton is enabled
    And "Configuration Version" value is "2.0-0.5"
    And I click on "Create Release" button
    And Page title is "Software Release Management"
    And I click on "Confirm" button
    And Page title is "Software Release Management"
    And following data is displayed in "Release" Grid
    | Release Date            | Release Name | Software Version | Configuration Version | Required | Study Wide | Device Type(s) | Country(s)    | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
    | (dd-MMM-yy)Current Date | Test Release | 7.0.0.0          | 2.0-0.5               | true     | false      | Phone          | United States | Site 1  | 1/0                      | 1/0                        |
    And Active toggle for "Test Release" is "Enabled"
    And "Test Release" is set as "Required" in db
    And I click on Active toggle button in Grid for "Test Release"
    And "Are you sure you want to deactivate Test Release?" message is displayed in "Deactivate Release" PopUp
    And I click on "No" button
    And Page title is "Software Release Management"
    And I click on Active toggle button in Grid for "Test Release" 
    And "Are you sure you want to deactivate Test Release?" message is displayed in "Deactivate Release" PopUp 
    When I click on "Yes" button
    Then Page title is "Software Release Management"
    And Active toggle for "Test Release" is "Disabled"
   

@MockStudyBuilder
#SRMAN001.190
Scenario: Verify Assigned/Reported Config is updated When device(s) with a deactivated version have been assigned to an Active higher software version
Given Version number "7.0.0.0" is added with Package path "https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_7.0.0.0.zip"
And Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And Software Release "Test Release 01" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
And "Phone" Device "YP-E2E-Device-Phone" is assigned to Site "Site 1"
And "Phone" Device "YP-E2E-Device-Phone" is assigned to Software Release "Test Release 01"
And I am logged in as "PortalE2EUser"
And I have all the correct permissons to view Device Management 
And I am on "At a Glance" page
And I click on "Manage Study" link on top navigation bar
And I click on "Software Release Management" link
And I am on "Software Release Management" page
And following data is displayed in "Release" Grid
| Release Date            | Release Name    | Software Version | Configuration Version | Required | Study Wide | Device Type(s) | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
| (dd-MMM-yy)Current Date | Test Release 01 | 1.0.0.0          | 1.0-0.4               | false    | true       |                |            |         | 1/1                      | 1/1                       |
And Active toggle for "Test Release 01" is "Enabled"         
And I click on Active toggle button in Grid for "Test Release 01" 
And I click on "Yes" button
And Active toggle for "Test Release 01" is "Disabled"
And I enter "Test Release" for "Release Name inputtextbox"
And I click on "Software Version" dropdown
And I select "7.0.0.0" from "Software Version" dropdown
And I click on "Configuration Version" dropdown
And I select "2.0-0.5" from "Configuration Version" dropdown
And I click on "Study Wide" togglebutton
And I click on "Create Release" button
And a popup is displayed
| popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
| Review Software and Configuration Release | 1 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.1 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
When I click on "Confirm" button
Then following data is displayed in "Release" Grid
| Release Date            | Release Name    | Software Version | Configuration Version | Required | Study Wide | Device Type(s) | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
| (dd-MMM-yy)Current Date | Test Release    | 7.0.0.0          | 2.0-0.5               | false    | true       |                |            |         | 1/0                      | 1/0                        |
| (dd-MMM-yy)Current Date | Test Release 01 | 1.0.0.0          | 1.0-0.4               | false    | true       |                |            |         | 0/0                      | 0/0                        |
And Active toggle for "Test Release" is "Enabled"
And Active toggle for "Test Release 01" is "Disabled"
When I click View Devices link
Then I am on "Device Management" page
And Device "YP-E2E-Device-Phone" is assigned to Software Version "7.0.0.0" and Configuration Version "2.0-0.5"

