
Feature: Site Facing Text Languages field added in Site Details page
Background:
	Given I have logged in with user "PortalE2EUser"
	And I have multiple languages set up in the study
	

@ID:3987
@MockStudyBuilder
#1
Scenario: Verify that the Site facing text display languages field is displayed and all the languages configured in Study Builder is displayed in the dropdown.
	 Given I have set system setting "SiteFacingTranslationsEnabled" to "true"
	 And I am on "At a Glance" page
	And I click on "Sites" link on the top navigation bar
	And I am on "Site Management" page
	And I click on "10001" from the sites table
	And I am on "Initial Site" detail page
	When I click on "Site-Facing Text Display Language" dropdown
	Then all the languages configured are displayed for "Site-Facing Text Display Language" dropdown


@ID:3987
@MockStudyBuilder
#2
Scenario: Verify that selected language is displayed when user clicks back from the save page. 
    Given I have set system setting "SiteFacingTranslationsEnabled" to "true"
	And I am on "At a Glance" page
	And I click on "Sites" link on the top navigation bar
	And I am on "Site Management" page
	And I click on "10001" from the sites table
	And I am on "Initial Site" detail page
	And I click on "Site-Facing Text Display Language" dropdown
	And I select "Arabic" from "Site-Facing Text Display Language" dropdown
	And I click on "Site Languages" tab
	And "en-US" togglebutton is enabled 
	And I click on "Next" button 
	When I click on "Site Details" tab 
	Then "Arabic" is displayed for "Site-Facing Text Display Language" dropdown

@ID:3987
@MockStudyBuilder
#3
Scenario: Verify that Site Facing Text Display Languages dropdown is not displayed when "Site Facing Translation Toggle" is disabled in Study Builder.
	Given I have set system setting "SiteFacingTranslationsEnabled" to "false"
	And I am on "At a Glance" page
	And I click on "Sites" link on the top navigation bar
	And I am on "Site Management" page
	And I click on "10001" from the sites table
	And I am on "Initial Site" detail page
	And "Site-Facing Text Display Language" dropdown is not "Visible"

@ID:3987
@MockStudyBuilder
#4
Scenario: Verify that the default langauge is set to English when no selection is made.	
    Given I have set system setting "SiteFacingTranslationsEnabled" to "true"
	And I am on "At a Glance" page
	And I click on "Sites" link on the top navigation bar
	And I click on "Add New Site" button
	And I am on "Site Details" page
	And I enter "Site 1" in "Name" inputtextbox field
	And I enter "10000" in "Site Number" inputtextbox field
	And I enter "Inv" in "Primary Contact" inputtextbox field
	And I enter "YP" in "Investigator" inputtextbox field
	And I enter "Address 1" in "Address1" inputtextbox field
	And I enter "Malvern" in "City" inputtextbox field
	And I enter "PA" in "State" inputtextbox field
	And I enter "19425" in "Zip" inputtextbox field
	And I click on "Country" dropdown
	And I select "United States" from "Country" dropdown
	And I enter "212121212" in "Phone Number" inputtextbox field
	And I click on "TimeZone" dropdown
	And I select "(UTC-05:00) Eastern Time (US & Canada)" from "TimeZone" dropdown
	And I click on "Active" togglebutton
	And I click on "Next Button To Site Language" button
	When I click on "Site Details" tab 
	Then "English (United States)" is displayed for "Site-Facing Text Display Language" dropdown

