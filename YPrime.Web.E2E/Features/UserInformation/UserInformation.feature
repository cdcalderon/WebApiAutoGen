Feature: UserInformation
	
Background:
	Given I have logged in with user "PortalE2EUser"
    And I am on "At a Glance" page
	And I click on "User Information" link on the top navigation bar

#1
# USIN001.20. User Profile Icon is located to the right of the navigation bar
# USIN001.30  Clicking the User Profile icon will display the following User Information and buttons: 
#	User Information: ( Logged in user email address (Role),  Study Name,    System Version,    Date/Time: day-month-year HH:MM:SS AM/PM (UTC) )
@MockStudyBuilder
Scenario: User Information Popover displays email, StudyName, systemVersion and date and time
	Given "PortalE2EUser" text is displayed
	And "eCOA_e2e" text is displayed
	And version is displayed with a valid format 
	When date and time displayed matches "d-MMMM-yyyy h:mm:ss tt (UTC)" format
	Then date matches todays UTC date
	
#2
#USIN001.40 The Privacy button will navigate the user to https://www.yprime.com/privacy/ . 
#USIN001.60 The Support button will navigate the user to https://www.yprime.com/support/ .
#USIN001.70. The Logout button will navigate the user to the Portal login page.   Validate by URL and title of the page (header), create the method for the URL validation.
@MockStudyBuilder
Scenario: User Information Popup can navigate to Navigation to Privacy, Support, Logout
	Given I click on "Support" button
	And I switch to the tab or window number "2"
	And current URL is "https://www.yprime.com/support/"
	And "Support" text is displayed in the page
	And I close the current tab 

	And I click on "Privacy Policy" button
	And I switch to the tab or window number "2"
	And current URL is "https://www.yprime.com/privacy/"
	And "Privacy Policy" text is displayed in the page
	And I close the current tab
	
	When I click on " Logout" button
	Then I validate that the current Url contains "Login" text
	And I validate that I am in the Login page

#3
#USIN001.50  ONLY VALIDATE THE NAVIGATION TO THE PAGE, if it launches the page.
#	The Change Password button will navigate the user to Change Password form with the following fields( Current password: , New password:, Confirm new password: )

#PART OF THE REQUIREMENT BUT WONT BE TESTED BY THIS FEATURE FILE SINCE ITS PART OF ANOTHER SSO MODULE:
#	Input Symbol – A captcha text box for the user to enter the characters listed about the refresh link.
#	Change Password – When the button is clicked and correct information is entered in all fields, the user’s password will be updated.
#	When invalid information is entered for any of the input fields, a detailed message will be displayed in red text and the password will not change.
@MockStudyBuilder
Scenario: User Information Popup can navigate to Change Password	
	When I click on "Change Password" button
	Then I validate that the current Url contains "ChangePassword" text
	Then "Change Password" text is displayed in the page 
	