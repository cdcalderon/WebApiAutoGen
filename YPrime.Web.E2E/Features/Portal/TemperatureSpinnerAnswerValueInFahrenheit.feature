@Portal

Feature: Temperature Spinner Display of the Answer Value in Fahrenheit 

Background: 
	 Given I have logged in with user "PortalE2EUser"
	 And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
     And "Tablet" Device "Yp-MG000207" is assigned to Site "Initial Site"
     And Subject "S-10001-001" is assigned to "Yp-MG000207" Device
     And Subject "S-10001-001" has completed "Temperature Spinner Questionnaire" questionnaire for question "Please record your highest temperature for the day" and choice "100.0 °F"

@ID:768
@MockStudyBuilder
#1
Scenario: Verify that the response value and appropriate suffix is displayed based of the site configuration. (US and India)
	Given I am on "At a Glance" page
	And I click on "Subject" link on the top navigation bar
	And I am on "Subject Management" page
	And I select Subject "S-10001-001"
	And I am on Subject "S-10001-001" page
	And I click on subject "Questionnaires" tab
	When I click on "Temperature Spinner Questionnaire" diary entry
	Then I am on "Diary Entry Details" page
	And following data is displayed in the data field table
      | Label                                              | Value    | Fieldtype |
      | Please record your highest temperature for the day | 100.0°F | text      |



