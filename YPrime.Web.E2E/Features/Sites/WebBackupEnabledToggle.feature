Feature: WebBackupEnabledToggle
NLA: 157
Description: Web Backup Tablet Site Activation

Background:
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And User "PortalE2EUser" with role "YP" has access to site "10000"

#1
#SITEAC001.20 - ‘Web Backup Enabled’ is available on the Site Details page when,
#CAN ACTIVATE WEB-BACKUP (TABLET) is enabled and WebBackupTabletEnabled key value is greater than 0
#SITEAC001.30 - Web Backup Enabled Toggle should not be present when,
#CAN ACTIVATE WEB-BACKUP (TABLET) is disabled and WebBackupTabletEnabled key value is greater than or less than 0
#CAN ACTIVATE WEB-BACKUP (TABLET) is enabled and WebBackupTabletEnabled key value is 0
@MockStudyBuilder
Scenario Outline: 01_Verify Web Backup Enabled Toggle Visibility
	Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "<CanActivateWebBackUp>"
	And I update "WebBackupTabletEnabled" key with value "<Days>" days
	And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I click on "Sites" link on top navigation bar
	And I am on "Site Management" page
	And I click on "10000" link
	When I am on "Site Details" page
	Then "Web Backup Enabled" Toggle is "<Visibility>"

Examples:
	| CanActivateWebBackUp | Days | Visibility  |
	| Disabled             | 5    | Not Visible |
	| Disabled             | 0    | Not Visible |
	| Enabled              | 0    | Not Visible |
	| Enabled              | 5    | Visible     |

#2
#SITEAC001.50 - When toggle is enabled, Enabled until date will appear based on the WebBackupTabletEnabled key value
#SITEAC001.40 - When toggle is disabled, Enabled until date should disappear
@MockStudyBuilder
Scenario: 02_Verify Enabled Until Date when Toggle is enabled and disabled
Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
And I update "WebBackupTabletEnabled" key with value "5" days
And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
And I am logged in as "PortalE2EUser"
And I am on "At a Glance" page
And I click on "Sites" link on top navigation bar
And I am on "Site Management" page
And I click on "10000" link
And I am on "Site Details" page
And "Web Backup Enabled" Toggle is displayed
And "WebBackupEnabled" togglebutton is disabled
And "Enabled until" text is "Not Visible"
And I click on "WebBackupEnabled" togglebutton
And "WebBackupEnabled" togglebutton is enabled
And I click on "Next Button To Site Language" button
And I am on "Site Details" page
And I click on "en-US" togglebutton
And I click on "Next" button
And I click on "Save" button
And "The site details have been updated successfully." text is displayed in the page
And I click on "Sites" link on top navigation bar
And I am on "Site Management" page
And I click on "10000" link
When I am on "Site Details" page
Then I verify Enabled until date is "currentday+5"
And I click on "WebBackupEnabled" togglebutton
And "WebBackupEnabled" togglebutton is disabled
And I click on "Next Button To Site Language" button
And I am on "Site Details" page
And I click on "Next" button
And I click on "Save" button
And "The site details have been updated successfully." text is displayed in the page
And I click on "Sites" link on top navigation bar
And I am on "Site Management" page
And I click on "10000" link
When I am on "Site Details" page
Then "Enabled until" text is "Not Visible"
And "Web Backup Enabled" Toggle is displayed

#3
#SITEAC001.70 - When web backup is enabled and there no devices associated with the site,
#then Web Backup Enabled toggle will be greyed out with ‘There are no devices at the site’ message
@MockStudyBuilder
Scenario: 03_Verify Web Backup Enabled toggle is greyed out with ‘There are no devices at the site’ message
	Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
	And I update "WebBackupTabletEnabled" key with value "5" days
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I click on "Sites" link on top navigation bar
	And I am on "Site Management" page
	And I click on "10000" link
	When I am on "Site Details" page
	Then "WebBackupEnabled" Toggle is greyed out
	And the following text is displayed on the page
		| Value                            |
		| There are no devices at the site |
