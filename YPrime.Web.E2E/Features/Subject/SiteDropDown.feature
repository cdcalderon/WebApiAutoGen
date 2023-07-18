Feature: SiteDropDown
	NLA-112

@MockStudyBuilder
#PTSUB-01 WGR.05   
Scenario: Verify Site dropdown for Subject with multiple sites assigned
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Site "Site 2" is assigned to Country "United States" and has site number "20000"
    And Site "Site 3" is assigned to Country "United States" and has site number "30000"
    And User "PortalE2EUser" with role "YP" has access to site "10000"
    And User "PortalE2EUser" with role "YP" has access to site "20000"
    And Patient "patient 1" with patient number "S-10000-004" is associated with "Site 1"
    And Patient "patient 2" with patient number "S-20000-001" is associated with "Site 2"
    And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Subject" link on the top navigation bar
    And I am on "Subject Information" page
    And "All Sites" value is "VISIBLE"
    When I click on "All Sites" dropdown
    Then "Site 1" is displayed for "All Sites" dropdown
    And "Site 2" is displayed for "All Sites" dropdown
    And "Site 3" is not displayed for "All Sites" dropdown
    When I select "All Sites" from "All Sites" dropdown
    Then "S-10000-004" is "VISIBLE"
    And "S-20000-001" is "VISIBLE"
    And I click on "All Sites" dropdown
    When I select "Site 1" from "All Sites" dropdown
    Then "S-10000-004" is "VISIBLE"
    And "S-20000-001" text is "NOT VISIBLE"
    And I click on "All Sites" dropdown
    And I select "Site 2" from "All Sites" dropdown
    And "S-20000-001" is "VISIBLE"
    And "S-10000-004" text is "NOT VISIBLE"

@MockStudyBuilder
Scenario: Verify Site dropdown for Subject with single site assigned
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"
    And Site "Site 1" is assigned to Country "United States" and has site number "20000"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And Patient "patient 1" with patient number "S-10000-004" is associated with "Initial Site"
    And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Subject" link on the top navigation bar
    And I am on "Subject Information" page
    Then "Initial Site" value is "VISIBLE"
    When I click on "All Sites" dropdown
    Then "Initial Site" is displayed for "All Sites" dropdown
    And "Site 1" is not displayed for "All Sites" dropdown
    And "S-10000-004" is "VISIBLE"