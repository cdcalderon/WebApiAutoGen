@NavigationBar
Feature: At A Glance
	
@MockStudyBuilder
#1
#WIEC.01 : "DCFs, Device Inventory, Recently Viewed and Enrollment" widgets will be available on the At a Glance page.
Scenario: Widgets will be available on the "At A Glance" page
     Given I am logged in as "PortalE2EUser"
     And I am on "Subject Management" page
     And I click on "At a Glance" link on the top navigation bar
     When I am on At a Glance page
     Then the following text is displayed on the page
        |    Value                    |
        |DCFs Requiring Your Attention|
        |Device Inventory             |
        |Recently Viewed              |  
        |Enrollment                   |  
