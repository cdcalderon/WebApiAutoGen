Feature: WebBackupMultipleSiteSelectionPage
NLA: 341
Description: Web Backup Multiple Site Selection Page	

#1
#MSSP001.20, MSSP001.30, MSSP001.40, #MSSP001.50
@MockStudyBuilder
Scenario: 01_Validations of Web Backup Multiple Site Selection Page
	Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
	And Site "Site 2" is assigned to Country "United States" and has site number "20000"
	And Site "Site 3" is assigned to Country "United States" and has site number "30000"
	And User "PortalE2EUser" with role "YP" has access to site "10000"
	And User "PortalE2EUser" with role "YP" has access to site "20000"
	And User "PortalE2EUser" with role "YP" has access to site "30000"
	And "Tablet" Device "YP-E2E-Device-Tablet1" is assigned to Site "Site 1"
	And "Tablet" Device "YP-E2E-Device-Tablet2" is assigned to Site "Site 2"
	And "Tablet" Device "YP-E2E-Device-Tablet3" is assigned to Site "Site 3"
	And "CAN ACTIVATE WEB-BACKUP (TABLET)" permission is "Enabled"
	And "CAN ACCESS WEB-BACKUP BUTTON (TABLET)" permission is "Enabled"
	And I update "WebBackupTabletEnabled" key with value "5" days
	And I enable Web Backup until "currentday+5" for the site "10000"
	And I enable Web Backup until "currentday+5" for the site "20000"
	And I am logged in as "PortalE2EUser"
	And I am on "At a Glance" page
	And I click on "Site Tablet Web Backup" button
	And I click on "Ok" button in the popup
	When I am on "Web Backup" page
	Then the following text is displayed on the page
		| Value           |
		| Home Web Backup |
	And I click on "Home" link
	And I am on "At a Glance" page
	And I click on "Site Tablet Web Backup" button
	And I click on "Ok" button in the popup
	When I am on "Web Backup" page
	Then the following text is displayed on the page
		| Value                                                                            |
		| If you do not see your site, please ensure that Web Backup is currently enabled. |
	And following columns are displayed for "Web Backup Details" Grid Menu
		| Label             |
		| Site Number       |
		| Name              |
		| Active Until      |
		| Launch Web Backup |
	And following data is displayed in "Web Backup Details" Grid
		| Site Number | Name   | Active Until |
		| 10000       | Site 1 | currentday+5 |
		| 20000       | Site 2 | currentday+5 |
		| 30000       | Site 3 | Invalid date |
	And I verify Launch Web Backup button is "enabled" for site "10000"
	And I verify Launch Web Backup button is "enabled" for site "20000"
	And I verify Launch Web Backup button is "disabled" for site "30000"
	When I click Launch Web Backup button for site "10000"
	Then I switch to the tab or window number "2"
	And "Web backup for Site 1" text is displayed in the page