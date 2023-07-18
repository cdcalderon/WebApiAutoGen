Feature: WebBackupNavigationAccess
NLA: 161
Description: Tablet Site Web Backup Access

Background:
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And  Site "Site 2" is assigned to Country "United States" and has site number "10002"
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And User "PortalE2EUser" with role "YP" has access to site "10002"

#1
#TSSWB001.20 ‘Site Tablet Web Backup’ button is available within the Portal’s Navigation bar when the site web backup is enabled.
#TSSWB001.30  When one site is activated for Web Backup for the study, the following will be shown under the ‘Site & Subject Web Backup” button:‘Expires {DD-MON}’
#TSSWB001.40 If the conditions in SITEAC001.30 are met and the Study has no site activations set, the Web Backup button will not be displayed on the menu bar.
@MockStudyBuilder
Scenario: Enable Site Tablet Web Backup permission and check visibility of "Site Tablet Web Backup"
Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
And I update "WebBackupTabletEnabled" key with value "5" days
And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
And I am logged in as "PortalE2EUser"
And I am on "At a Glance" page
And I click on "Sites" link on top navigation bar
And I am on "Site Management" page
And I click on "10000" link
And I am on "Site Details" page
And I click on "WebBackupEnabled" togglebutton
And "WebBackupEnabled" togglebutton is enabled
And I click on "Next Button To Site Language" button
And I am on "Site Details" page
And I click on "en-US" togglebutton
And I click on "Next" button
And I click on "Save" button
When "The site details have been updated successfully." text is displayed in the page
Then "Site Tablet Web Backup" link is "visible" on top navigation bar
And "Expires currentday+5" date is "visible" on top navigation bar
And I click on "Sites" link on top navigation bar
And I am on "Site Management" page
And I click on "10000" link
And I am on "Site Details" page
And I click on "WebBackupEnabled" togglebutton
And "WebBackupEnabled" togglebutton is disabled
And I click on "Next Button To Site Language" button
And I am on "Site Details" page
And I click on "Next" button
And I click on "Save" button
And "The site details have been updated successfully." text is displayed in the page
And "Site Tablet Web Backup" link is "not visible" on top navigation bar
And "Expires currentday+5" date is "not visible" on top navigation bar


#2
#TSSWB001.50 When the "Site Tablet Backup" button is selected, a pop-up will be displayed which states, “To send a Web Backup link to a subject for site based questionnaire completion on their personal device, please use the web backup button available within the subject visit management"
#OK--> A new tab opens in the user’s current browser and DEEM001.01 is displayed.
@MockStudyBuilder
Scenario: Click on "Site Tablet Web Backup" button
	Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
	And I update "WebBackupTabletEnabled" key with value "5" days
	And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I click on "Sites" link on top navigation bar
	And I am on "Site Management" page
	And I click on "10000" link
	And I am on "Site Details" page
	And "Web Backup Enabled" Toggle is "Enabled"
	When I click on "Site Tablet Web Backup" link on top navigation bar
	Then popup is displayed with message "“To send a Web Backup link to a subject for site based questionnaire completion on their personal device, please use the web backup button available within the subject visit management"
	And I click on "Ok" button in the popup
	And I switch to the tab or window number "2"
	And "Web backup for test" text is "Visible"

#3
#TSSWB001.30 *If multiple sites have been activated or no sites have been activated, the expiration date will not be displayed.
@MockStudyBuilder
Scenario: Verify that the Expiry Date is not visible when mulitiple sites have been activated
	Given "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
	And I update "WebBackupTabletEnabled" key with value "5" days
	And "Tablet" Device "YP-E2E-Device-Tablet" is assigned to Site "Site 1"
	And "Tablet" Device "YP-E2E-Device-Tablet2" is assigned to Site "Site 2"
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
    And I click on "Sites" link on top navigation bar
    And I am on "Site Management" page
    And I click on "10000" link
    And I am on "Site Details" page
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
    And I click on "10002" link
    And I am on "Site Details" page
    And I click on "WebBackupEnabled" togglebutton
	And "WebBackupEnabled" togglebutton is enabled
	And I click on "Next Button To Site Language" button
	And I am on "Site Details" page
	And I click on "en-US" togglebutton
	And I click on "Next" button
	And I click on "Save" button
	When "The site details have been updated successfully." text is displayed in the page
	Then "Expires currentday+5" date is "not visible" on top navigation bar
	And "Site Tablet Web Backup" link is "visible" on top navigation bar

