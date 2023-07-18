@WidgetRecentlyViewed
Feature: Widget Recently Viewed
	Verify recently viewed widget


Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Patient "patient 1" with patient number "S-10000-004" is associated with "Site 1"
    And Patient "patient 2" with patient number "S-10000-005" is associated with "Site 1"
    And Patient "patient 3" with patient number "S-10000-006" is associated with "Site 1"
    And Patient "patient 4" with patient number "S-10000-007" is associated with "Site 1"
    And Patient "patient 5" with patient number "S-10000-008" is associated with "Site 1"
    And User "PortalE2EUser" with role "YP" has access to site "10000"

@MockStudyBuilder
#VWD.02
Scenario: Upon initial login prior to any Subject selection, the Recently Viewed widget will have a default display of “No recently viewed Subjects.”
	Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I click on "At a Glance" link on the top navigation bar
    When I am on "At a Glance" page
    Then "NO RECENTLY VIEWED SUBJECTS." text is displayed

@MockStudyBuilder
#VWD.03,.04,.05,.06,.07
 Scenario: Recently viewed widget will display the 4 most recent Subjects viewed in current session, Clear Button functionality, 
    Given I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am back on "At a Glance" page
    And I am on At a Glance page
    When "Clear" button is displayed
    Then I am back on "Subject Management" page
    And I select Subject "S-10000-005"
    And I click on "Subject" link on the top navigation bar
    And I am on "Subject Management" page
    And I select Subject "S-10000-006"
    And I click on "Subject" link on the top navigation bar
    And I am on "Subject Management" page
    And I select Subject "S-10000-007"
    And I am back on "Subject Management" page
    And I click on "At a Glance" link on the top navigation bar
    And I am on At a Glance page
    And "S-10000-004" link is "Visible" 
    And "S-10000-005" link is "Visible" 
    And "S-10000-006" link is "Visible"
    And "S-10000-007" link is "Visible"
    And I click on "S-10000-005" link
    And I am on Subject "S-10000-005" page
    And I click on "Subject" link on the top navigation bar
    And I select Subject "S-10000-008"
    And I click on "At a Glance" link on the top navigation bar
    And "S-10000-004" link is "Not Visible"
    And "S-10000-005" link is "Visible"
    And "S-10000-006" link is "Visible"
    And "S-10000-007" link is "Visible"
    And "S-10000-008" link is "Visible" 
    And I click on "Clear" button
    And "NO RECENTLY VIEWED SUBJECTS." text is displayed
