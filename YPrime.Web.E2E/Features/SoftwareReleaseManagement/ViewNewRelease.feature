@ViewNewRelease @VersionManagement
Feature: View New Release(Version management)
	#NLA-334
	#View New Release    
 
Background: 
      Given Version number "0.0.0.7" is added with Package path "https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_0.0.0.7.zip"
        And Version number "0.0.0.8" is added with Package path "https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_0.0.0.8.zip"

@MockStudyBuilder
#1 VEMAN001.20
Scenario: Verify that Version management sub menu item is available within manage study menu
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And I click on "Manage Study" link on the top navigation bar
        And I click on "Version Management" link
       When I am on "Software Version" page
       Then following data is displayed in "Software Version" Grid
            | Version Number | Package Path |
            | 0.0.0.1        |              |
            | 0.0.0.7        | https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_0.0.0.7.zip   |
            | 0.0.0.8        | https:/ypadminMock/01020304-12ef-4764-8f34-b1fd9caa4d3a/apk/yprime.eCOA.Droid_0.0.0.8.zip   |
        And I click on "Hamburger Grid" button
	    And hamburger menu is displayed with the following functionality for export
		    | ButtonName	|
		    | Excel			|
		    | CSV           |
		    | PDF           |
		    | Print         |
        And I click on "Package Path" link
        And I click on the background screen
        And following data is displayed in "Software Version" Grid
            | Version Number |
            | 0.0.0.1        |
            | 0.0.0.7        |
            | 0.0.0.8        |
        And "Package Path" column is not visible in "Software Version" grid

@MockStudyBuilder
#2 VEMAN001.30
Scenario: Verify that Breadcrumbs are present and after clicking on home, We will land on home page(At a Glance)
      Given I am logged in as "PortalE2EUser"
        And I am on "Subject Management" page
        And I click on "Manage Study" link on the top navigation bar
        And I click on "Version Management" link
       When I am on "Software Version" page
       Then "Home" link is "Visible"
        And I click on "Home" link
        And I am on "At a Glance" page