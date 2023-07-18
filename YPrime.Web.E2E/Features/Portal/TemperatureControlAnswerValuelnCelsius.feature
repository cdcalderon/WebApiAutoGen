@Portal

Feature: Temperature Control Display of Answer Value in Celsius

Background: 
	 Given I have logged in with user "PortalE2EUser"
	 And Site "Site 1" is assigned to Country "India" and has site number "20001"
	 And User "PortalE2EUser" with role "YP" has access to site "20001"
     And "Tablet" Device "Yp-MG000207" is assigned to Site "Site 1"
     And Subject "S-20001-001" is assigned to "Yp-MG000207" Device
	 And Patient "patient 1" with patient number "S-20001-001" is associated with "Site 1"
     And Subject "S-20001-001" has completed "Temperature Spinner Questionnaire" questionnaire for question "Please record your highest temperature for the day" and choice "38.1 °C"

@ID:768
@MockStudyBuilder
#1
Scenario: Verify that the response value and appropriate suffix is displayed based of the site configuration. (US and India)
	Given I am on "At a Glance" page
	And I click on "Subject" link on the top navigation bar
	And I am on "Subject Management" page
	And I click on "All Sites" dropdown
    And I select "Site 1" from "All Sites" dropdown
	And I select Subject "S-20001-001"
	And I am on Subject "S-20001-001" page
	And I click on subject "Questionnaires" tab
	When I click on "Temperature Spinner Questionnaire" diary entry
	Then I am on "Diary Entry Details" page
	And following data is displayed in the data field table
      | Label                                              | Value   | Fieldtype |
      | Please record your highest temperature for the day | 38.1°C | text      |
