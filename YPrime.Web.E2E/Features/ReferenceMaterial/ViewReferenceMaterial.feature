Feature: View Reference Material

#NLA-301   
#REFM.02,#REFM.03,#REFM.04
#1
@MockStudyBuilder
Scenario: Verify Reference Material file is opened in new tab
    Given Site "Initial Site" is assigned to Country "United States" and has site number "10001"    
	And User "PortalE2EUser" with role "YP" has access to site "10001"
	And I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Reference Materials" on the top navigation bar
    And I am on "Reference Material" page
    And "No Reference Materials Uploaded" text is displayed in the page
    And I click on "Manage Study" on the top navigation bar
    And I click on "Upload Reference Material" link
    And I am on "Upload Reference Material" page
    And I enter "yprime reference material pdf" for "Name"
    And I upload a file with file format "PDF"
    And "Frequently Asked Questions" value is "Visible"
    And I click on "Add New" button
    And "Info" popup is displayed with message "Reference Material 'yprime reference material pdf' was successfully uploaded."
    And I click "Ok" button in the popup
    And I enter "yprime reference material Manual" for "Name"
    And I upload a file with file format "pdf"
    And I click on "Reference Materials" Dropdown
    And I select the given "Training Manuals" from "ReferenceMaterialTypeId" dropdown
    And I click on "Add New" button
    And "Info" popup is displayed with message "Reference Material 'yprime reference material Manual' was successfully uploaded."
    And I click "Ok" button in the popup
    And I enter "yprime reference material videos" for "Name"
    And I upload a file with file format "MP4"
    And I click on "Reference Materials" Dropdown
    And I select the given "Training Videos" from "ReferenceMaterialTypeId" dropdown
    And I click on "Add New" button
    And "Info" popup is displayed with message "Reference Material 'yprime reference material videos' was successfully uploaded."
    And I click "Ok" button in the popup
    And I click on "Reference Materials" on the top navigation bar
    When I am on "Reference Material" page
    Then following data is displayed in "Reference Material" page
        | Referenece Material Type   | Reference Material               |
        | Frequently Asked Questions | yprime reference material pdf    |
        | Training Manuals           | yprime reference material Manual |
        | Training Videos            | yprime reference material videos |
    And I click on "yprime reference material pdf" link
    And "yprime reference material pdf" file is opened in new tab
    And I click on "yprime reference material Manual" link
    And "yprime reference material Manual" file is opened in new tab
    And I click on "yprime reference material videos" link
    And "yprime reference material videos" file is opened in new tab