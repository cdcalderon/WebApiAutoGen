Feature: Assign Caregiver
    #ECOA-2031
    #Able to assign new caregiver within portal 
Background:
    Given Site "Site 1" is assigned to Country "United States" and has site number "10000"
    And Patient "patient 1" with patient number "S-10000-004" is associated with "Site 1"
    And User "PortalE2EUser" with role "YP" has access to site "10000"
    And Caregiver study setting "Caregivers Enabled" is "Enabled"
    And "CAN VIEW CAREGIVER DETAILS." permission is "Enabled"



@ID:2031 @MockStudyBuilder
 #CDV.06 Upon selecting Caregiver Management and if the user role has the following permission "Can Create Caregiver in Portal then "Select Caregiver Type" will be displayed with a dropdown.
 #CDV.07 All caregiver types configured within the study configuration will be displayed in” Select Caregiver type” dropdown in alphabetical order. 
 #1
Scenario: Verify that Select A Caregiver type dropdown is displayed and the caregiver type is listed in alphabetical order when "Can Create Caregiver In Portal " permission is enabled and Assign Caregiver and Cancel button are displayed.
    Given "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And "Select A Caregiver Type" dropdown is "Visible"
    And "Select A Caregiver Type" dropdown has placeholder "Please Select"
    When I click on "Select A Caregiver Type" dropdown 
    Then "Select A Caregiver Type" dropdown is displayed in alphabetical order 
    And "Assign Caregiver" button is displayed 
    And "Cancel" button is displayed

@ID:2031 @MockStudyBuilder
 #CDV.06 If "Can Create Caregiver in Portal" is disabled, "Select Caregiver Type" will not be displayed with a   dropdown.
 #2
Scenario Outline: Verify that Select A Caregiver type dropdown is not displayed, when "Can Create Caregiver In Portal " permission is disabled.
    Given "CAN CREATE CAREGIVER IN PORTAL" permission is "Disabled" 
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    When I click on "Caregiver Management" tab
    Then "Select A Caregiver Type" dropdown is not "Visible"
 

@ID:2031 @MockStudyBuilder 
 #CDV.08 Once a user selects a caregiver type and clicks “cancel” button, then “Assign caregiver” button will be deselected, and user will remain on the Caregiver Management tab
 #3
Scenario: Verify that when the user selects a caregiver and clicks the cancel button, the caregiver is not displayed in the grid. The Please select is displayed in the dropdown, and the user remains on the caregiver management tab.
    Given "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    When I click on "Cancel" button
    Then "Please Select" placeholder is displayed in "Select A Caregiver Type" dropdown
    
@ID:2031 @MockStudyBuilder 
 #CDV.09 Once a user selects a caregiver type and clicks “Assign Caregiver” button, the user will receive the following popup:Note: Caregiver’s PIN is 1234/123456 (correct PIN   length displayed based off of configuration) by default. Caregiver will be   prompted to update upon initial login. Only a parent or legal guardian who   has provided informed consent may complete the Parent/Caregiver questionnaire   on behalf of Translation:lblPatientLowerCase. (lblPatientLowerCase signifies patient/participant/subject   text) By selecting "Save" you are confirming that the Caregiver   being set up has provided informed consent.
 #CDV.11 Upon selection of “Cancel” on the pop-up, the   caregiver will not be added to “Caregiver grid” section and will be available   in the “Select Caregiver Type” dropdown, the user remains on Caregiver   Management tab.
 #4
 Scenario Outline: Verify that based on the study configuration, the popup message displays the correct default PIN value when the user selects a caregiver and clicks the Assign Caregiver button and pop up is dismissed when user clicks Cancel button.
    Given I have a study configured using pin length of "<Pin Length>"
    And "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    And I click on "Assign Caregiver" button
    And pop up "AssignCaregiver" is displayed with message "<AssignCaregiverMessage>" and buttons "<ActionButtons>"
    When I click on "Cancel" button
    Then popup is dismissed
    And "Assign Caregiver" button is not selected
    And I remain on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And "Parent" is displayed for "Select A Caregiver Type" dropdown
    
    Examples: 
    | Pin Length | AssignCaregiverMessage                                                                                                                                                                                                                                                                                                                             | ActionButtons |
    | 4          | Note: Caregiver's PIN is 1234 by default. Caregiver will be prompted to update upon initial login. Only a parent or legal guardian who has provided informed consent may complete the Parent/Caregiver questionnaire on behalf of subject. By selecting "Save" you are confirming that the Caregiver being set up has provided informed consent.   | Save, Cancel  |
    | 6          | Note: Caregiver's PIN is 123456 by default. Caregiver will be prompted to update upon initial login. Only a parent or legal guardian who has provided informed consent may complete the Parent/Caregiver questionnaire on behalf of subject. By selecting "Save" you are confirming that the Caregiver being set up has provided informed consent. | Save, Cancel  |
    
@ID:2031 @MockStudyBuilder 
 #CDV.07 Once the caregiver type is assigned, it will no longer be displayed in the dropdown and will be displayed in the grid.
 #CDV.09 Once a user selects a caregiver type and clicks   “Assign Caregiver” button, the user will receive the following popup:Note: Caregiver’s PIN is 1234/123456 (correct PIN   length displayed based off of configuration) by default. Caregiver will be   prompted to update upon initial login. Only a parent or legal guardian who   has provided informed consent may complete the Parent/Caregiver questionnaire   on behalf of Translation:lblPatientLowerCase. (lblPatientLowerCase signifies patient/participant/subject   text) By selecting "Save" you are confirming that the Caregiver   being set up has provided informed consent.
 #CDV.10 Upon selection of “Save” on the pop-up, the caregiver is displayed within the “Caregiver grid” section and the user remains on Caregiver Management tab. 
 #5
 Scenario Outline: Verify that when the user clicks "Save" on the pop up the caregiver is displayed in the grid, and the Please select is displayed in the dropdown. The assigned caregiver is not listed in the dropdown, and the user remains on the caregiver management tab.
    Given I have a study configured using pin length of "<Pin Length>"
    And "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    And I click on "Assign Caregiver" button
    And pop up "AssignCaregiver" is displayed with message "<AssignCaregiverMessage>" and buttons "<ActionButtons>"
    When I click on "Save" button
    Then Caregiver "Parent" is displayed in the grid 
    And I remain on "Caregiver Management" tab
    And "Please Select" placeholder is displayed in "Select A Caregiver Type" dropdown 
    And "Parent" is not displayed for "Select A Caregiver Type" dropdown
    
    Examples: 
    | Pin Length | AssignCaregiverMessage                                                                                                                                                                                                                                                                                                                             | ActionButtons |
    | 4          | Note: Caregiver's PIN is 1234 by default. Caregiver will be prompted to update upon initial login. Only a parent or legal guardian who has provided informed consent may complete the Parent/Caregiver questionnaire on behalf of subject. By selecting "Save" you are confirming that the Caregiver being set up has provided informed consent.   | Save, Cancel  |
    | 6          | Note: Caregiver's PIN is 123456 by default. Caregiver will be prompted to update upon initial login. Only a parent or legal guardian who has provided informed consent may complete the Parent/Caregiver questionnaire on behalf of subject. By selecting "Save" you are confirming that the Caregiver being set up has provided informed consent. | Save, Cancel  |
    
@ID:2031 @MockStudyBuilder 
 #CDV.03 Clicking the Reset PIN button will display message above the Caregiver grid and Update Pin button throws a pop-up message stating “<Caregivertype> Temporary PIN has been reset.” With an ‘OK’ button. Clicking ‘OK’ closes the pop-up and clears the message above the grid   
 #6
 Scenario Outline: Verify that PIN reset message will appear with "OK" button when user clicks on Update Pin after clicking on Reset Pin and clicking "OK" closes the pop-up.
    Given I have a study configured using pin length of "<Pin Length>"
    And "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And "CAN RESET THE PIN FOR A CAREGIVER." permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    And I click on "Assign Caregiver" button
    And I click on "Save" button
    And Caregiver "Parent" is displayed in the grid   
    And Reset PIN button is "displayed" for caregiver for subject "S-10000-004"
    When I "click" on Reset PIN button for subject "S-10000-004"
    Then Message "<Message>" is displayed above "Caregiver" grid with "<ActionButtons>"
    And I click on "Update PIN" button
    And "UpdatePin" pop up is displayed with message "<UpdatePinMessage>" and buttons "<Ok>"
    And I click "Ok" button in the popup
    And popup is not displayed
    And Message "<Message>" is not displayed above the "Caregiver" grid 
    
    Examples: 
    | Pin Length | Message                                | ActionButtons      | UpdatePinMessage                     | Ok |
    | 4          | RESET PIN NUMBER FOR PARENT TO 1234?   | Update PIN, Cancel | Parent Temporary PIN has been reset. | Ok |
    | 6          | RESET PIN NUMBER FOR PARENT TO 123456? | Update PIN, Cancel | Parent Temporary PIN has been reset. | Ok |

        
@ID:2031 @MockStudyBuilder 
 #CDV.03 Clicking the Reset PIN button will display message above the Caregiver grid and Click’ Cancel’ clears the message above the grid, and does not reset the Caregivers PIN.
 #7
 Scenario Outline: Verify that PIN will not be reset when user clicks "Cancel" button after clicking on Reset Pin .
    Given I have a study configured using pin length of "<Pin Length>"
    And "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And "CAN RESET THE PIN FOR A CAREGIVER." permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    And I click on "Assign Caregiver" button
    And I click on "Save" button
    And Caregiver "Parent" is displayed in the grid   
    And Reset PIN button is "displayed" for caregiver for subject "S-10000-004"
    When I "click" on Reset PIN button for subject "S-10000-004"
    Then Message "<Message>" is displayed above "Caregiver" grid with "<ActionButtons>"
    And I click on "Cancel" button
    And Message "<Message>" is not displayed above the "Caregiver" grid 

    Examples: 
    | Pin Length | Message                               | ActionButtons      |
    | 4          | RESET PIN NUMBER FOR PARENT TO 1234?  | Update PIN, Cancel | 
    | 6          | RESET PIN NUMBER FOR PARENT TO 123456?| Update PIN, Cancel |


@ID:2031 @MockStudyBuilder 
 #CDV.02  Handheld Training Complete,Tablet Training Complete displays true when training is complete and Account locked displays true if user locked the account. 
 #8
 Scenario: Verify the data displayed in the grid .
    Given "CAN CREATE CAREGIVER IN PORTAL" permission is "Enabled"
    And I am logged in as "PortalE2EUser"
    And I am on "Subject Management" page
    And I select Subject "S-10000-004"
    And I am on Subject "S-10000-004" page
    And I click on "Caregiver Management" tab
    And I click on "Select A Caregiver Type" dropdown
    And I select "Parent" from "Select A Caregiver Type" dropdown
    And I click on "Assign Caregiver" button
    When I click on "Save" button
    Then the following data is displayed in the caregiver grid for subject "S-10000-004"
    | Caregiver | HandheldTrainingComplete | TabletTrainingComplete | AccountLocked | ResetPin  |
    | Parent    | False                    | False                  | False         | Reset PIN |
    And I completed "Handheld training" for "CareGivers"
    And I completed "Tablet training" for "CareGivers"
    And I locked "Account" for "Caregivers"
    And I sync the data 
    And I click on "Caregiver Management" tab
    Then the following data is displayed in the caregiver grid for subject "S-10000-004"
    | Caregiver | HandheldTrainingComplete | TabletTrainingComplete | AccountLocked | ResetPin  |
    | Parent    | True                     | True                   | True          | Reset PIN |
    

    
