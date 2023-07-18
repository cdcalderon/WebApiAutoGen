Feature: Device Management

@MockStudyBuilder
#DMGT.02 DMGT.04
Scenario: Verify Device count is displayed for assigned devices in "Device Inventory" widget
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
And "BYOD" Device "YP-E2EBYOD" is assigned to Site "Site 1"
And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 1"
And I am logged in as "PortalE2EUser"
When I am on "At a Glance" page
Then "1" Devices is displayed for "Phone"
And "1" Devices is displayed for "Tablet"
And "3" Devices is displayed for "Total Devices"


@MockStudyBuilder
#DMGT.03-1
Scenario: Verify Unused device count is "0" for Single device study
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
And I am logged in as "PortalE2EUser"
When I am on "At a Glance" page
Then "0" Devices is displayed for "Phone"
And "1" Devices is displayed for "Tablet"

@MockStudyBuilder
#DMGT.03-1
Scenario: Verify device count is "0" for No device study
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And I am logged in as "PortalE2EUser"
When I am on "At a Glance" page
And "0" Devices is displayed for "Total Devices"

@MockStudyBuilder
#DMGT.07
Scenario: Verify "Hamburger icon" in "Device Inventory" widget is not displayed when "CAN VIEW THE LIST OF DEVICES IN THE SYSTEM" permission is "Disabled"
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 1"
And "CAN VIEW THE LIST OF DEVICES IN THE SYSTEM." permission is "Disabled"
And I am logged in as "PortalE2EUser"
When I am on "At a Glance" page
Then "Hamburger" button is not "Visible" 


@MockStudyBuilder
#DMGT.11 DMGT.07 DMGT.10 DMGT.09
Scenario: Verify "Device Management" Page for devices assigned and "Device Details" page for device details
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And "CAN VIEW THE LIST OF DEVICES IN THE SYSTEM." permission is "Enabled"
And "CAN VIEW THE DETAILS PAGE FOR A DEVICE ." permission is "Enabled"
And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
And "BYOD" Device "YP-E2EBYOD" is assigned to Site "Site 1"
And "BYOD" Device "YP-E2EBYOD" is assigned to Software Release "Initial Release"
And "BYOD" Device "YP-E2EBYOD" last sync date is (Current Date) with sync Action "SyncInitialClientData"
And I am logged in as "PortalE2EUser"
And I am on "At a Glance" page
And I click on "Hamburger icon" in "Device Inventory" widget
And I click on View Devices button under Hamburger icon
And I am on "Device Management" page
Then following data is displayed in the grid
| Site Name | Device Name   | Device Type | Release Name    | Software Version | Configuration Version | Last Reported Software Version | Last Reported configuration Version | Last Data Sync |
| Site 1    | YP-E2EBYOD | BYOD       | Initial Release | 1.0.0.0            | 1.0-0.4              | 1.0.0.0                         | 1.0-0.4                             | (Current Date) |
And I click on "YP-E2EBYOD" link
And I am on "Device Details" page
Then following data is displayed on Device Details page
| Field                                 | Value             |
| Site Id                               | 10000             |
| Device Type Name                      | BYOD             |
| Assigned Software Version             | 1.0.0.0            |
| Last Reported Software Version        | 1.0.0.0             |
| Assigned Configuration Version        | 1.0-0.4           |
| Last Reported Configuration Version   | 1.0-0.4           |
| Last Data Sync Date                   | (Current Date)    |
And "Sync Logs" is displayed on device details
And following data is displayed in device details grid
| Sync Date         | Last Reported Software Version | Last Reported Configuration Version | Sync Action    |
| (Current Date)    | 1.0.0.0                         | 1.0-0.4                          | SyncInitialClientData |

@MockStudyBuilder
#DMGT.03-2
Scenario: Verify Users will only have access to the devices of sites that they have access to.
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And Site "Site 2" is assigned to Country "United States" and has site number "20000"
And User "ypadmin1@yprime.com" with role "YP" has access to site "20000"
And User "ypadmin1@yprime.com" with role "YP" has access to site "10000"
And "Tablet" Device "YP-E2E-Device-Tablet_Site1" is assigned to Site "Site 1"
And "Phone" Device "YP-E2E-Device_Site1" is assigned to Site "Site 1"
And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 2"
And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 2"
And "CAN VIEW THE LIST OF DEVICES IN THE SYSTEM." permission is "Enabled"
And I have logged in with user "ypadmin1@yprime.com", password "Welcome012!"
And I am on "At a Glance" page
Then "2" Devices is displayed for "Phone"
And "2" Devices is displayed for "Tablet"
And I click on the user icon
And I click on " Logout" button
And I am logged in as "PortalE2EUser"
And I am on "At a Glance" page
Then "1" Devices is displayed for "Phone"
And "1" Devices is displayed for "Tablet"



@MockStudyBuilder
#DMGT.10
Scenario Outline: Verify Three items will be displayed, “SyncInitialClientData”, “SyncClientData” or “CheckForUpdates” for "Sync Action"
Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
And User "PortalE2EUser" with role "YP" has access to site "10000"
And Software Release "Initial Release" has been created with Software Version "1.0.0.0" and Configuration Version "1.0-0.4"
And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 1"
And "Phone" Device "YP-E2E-Device" is assigned to Software Release "Initial Release"
And "Phone" Device "YP-E2E-Device" last sync date is (Current Date) with sync Action "<SyncAction>"
And "CAN VIEW THE LIST OF DEVICES IN THE SYSTEM." permission is "Enabled"
And "CAN VIEW THE DETAILS PAGE FOR A DEVICE ." permission is "Enabled"
And I am logged in as "PortalE2EUser"
And I am on "At a Glance" page
And I click on "Hamburger icon" in "Device Inventory" widget
And I click on View Devices button under Hamburger icon
And I am on "Device Management" page
And I click on "YP-E2E-Device" link
And I am on "Device Details" page
Then following data is displayed in device details grid
| Sync Date         | Last Reported Software Version | Last Reported Configuration Version | Sync Action    |
| (Current Date)   | 1.0.0.0                      | 1.0-0.4                            | <SyncAction> |

Examples: 
			| SyncAction            |
			| SyncInitialClientData |
			| CheckForUpdates       |
			| SyncClientData        |