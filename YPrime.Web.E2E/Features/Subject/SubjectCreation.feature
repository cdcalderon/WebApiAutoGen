Feature: Subject Creation
    NLA-114

Background: 
    Given Site "Initial Site" is assigned to Country "India" and has site number "10001"
    And User "PortalE2EUser" with role "YP" has access to site "10001"
    And Patient "patient 1" with patient number "S-10001-004" is associated with "Initial Site"

@MockStudyBuilder
#1 
#CreateSub.05 CreateSub.06 CreateSub.01 WGR.01

Scenario: The subject is successfully created in the portal with valid 4 digit pin length for a non-BYOD configured study. 
        Given I have a study configured using pin length of "4"
        And I have a study with BYOD as "Disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Information" page
        And "Initial Site" value is "VISIBLE"
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And I click on "Cancel" button
        And I am on "Subject Information" page
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        When I enter the following data
            | Label                                       | Value                                       | FieldType    |
            | Subject Number                              | 123                                         | Numberinput  |
            | Gender                                      | Female                                      | Radio Button |
            | Date of Birth                               | CurrentDate                                 | datepicker   |
            | Weight                                      | 100.00                                      | Numberinput  |
            | Height                                      | 100.00                                      | Numberinput  |
        And I click on "Create" button
        And Enrolled Date for Patient "S-10001-123" is saved with local Time Zone in DB
        Then "Success" popup is displayed with message "Subject S-10001-123 has been added successfully with the default PIN of 1234."
        And I click "Ok" button in the popup
        And I am on "Subject Information" page


@MockStudyBuilder
    #3 CreateSub.05 CreateSub.01 CreateSub.01 
    Scenario: The subject is successfully created in the portal with valid 6 digit pin length and "Warning" popup is displayed with duplicate Subject number
        Given I have a study configured using pin length of "6"
        And I have a study with BYOD as "Disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Information" page
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        When I enter the following data
            | Label                                       | Value                                     | FieldType    |
            | Subject Number                              | 123                                       | Numberinput  |
            | Gender                                      | Female                                    | Radio Button |
            | Date of Birth                               | CurrentDate                              | datepicker   |
            | Weight                                      | 100.00                                    | Numberinput  |
            | Height                                      | 100.00                                    | Numberinput  |
        And I click on "Create" button
        Then "Success" popup is displayed with message "Subject S-10001-123 has been added successfully with the default PIN of 123456."
        And I click "Ok" button in the popup
        And I am on "Subject Information" page
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page  
        When I enter the following data
            | Label                                       | Value                                     | FieldType    |
            | Subject Number                              | 123                                       | Numberinput  |
            | Gender                                      | Female                                    | Radio Button |
            | Date of Birth                               | CurrentDate                              | datepicker   |
            | Weight                                      | 100.00                                    | Numberinput  |
            | Height                                      | 100.00                                    | Numberinput  |
        And I click on "Create" button  
        Then "Warning" popup is displayed with message "This subject number already exists."
        And I click "Ok" button in the popup
        And I am on "Add New Subject" page
        
        
@MockStudyBuilder
  #3  CreateSub.02 CreateSub.03
  # Weight Max value is "100" and Number length is "3"
  # Height Max value is "200" and Number length is "3"
  # Subject Number set to "123"
  # Gender set to "Female"
  # Date Of birth set to "CurrentDate"
  # Height set to "100.00"
     Scenario: Verify Validation is displayed for numeric Field
        Given I have a study with BYOD as "Disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Information" page
        And "Add New Subject" button is enabled
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        When I enter the following data
            | Label                                       | Value                                     | FieldType    |
            | Subject Number                              | 123                                       | Numberinput  |
            | Gender                                      | Female                                    | Radio Button |
            | Date of Birth                               | CurrentDate                              | datepicker   |
            | Height                                      | 100.00                                    | Numberinput  |      
       And popup is displayed on entering following data in "Weight" field
            | Input  | PopUpTitle | PopUpMessage                              |
            |        | Warning    | Weight is a required field                |
            | 101    | Warning    | Weight Contains an invalid numeric value. |
            | 101.01 | Warning    | Weight Contains an invalid numeric value. |
            | abc    | Warning    | Weight Contains an invalid numeric value. |
            | 1      | Warning    | Weight Field needs to be 3-digits long. If needed, please use preceeding zeros to make your selection. |
            | 0010   | Warning    | Weight Field needs to be 3-digits long. If needed, please use preceeding zeros to make your selection. |

#3  CreateSub.01 CreateSub.02
@MockStudyBuilder
  # Subject Number Length is "3"
    Scenario: Verify Validation for "Subject Number" Field
        Given I have a study with BYOD as "Disabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Information" page
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And "10001" text is "VISIBLE"
        And I enter the following data
            | Label                                       | Value                                     | FieldType    |
            | Gender                                      | Female                                    | Radio Button |
            | Date of Birth                               | CurrentDate                              | datepicker   |
            | Weight                                      | 100.00                                    | Numberinput  |
            | Height                                      | 100.00                                    | Numberinput  |
         And popup is displayed on entering following data in "Subject Number" field
            | Input  | PopUpTitle   | PopUpMessage                              |
            |        | Warning      | Subject Number field needs to be 3-digits long.|
            | 01     | Warning      | Subject Number field needs to be 3-digits long. |
            | 000    | Warning      | Subject number must be greater than 0. |
            


@MockStudyBuilder
#CreateSub.04 CreateSub.02
 Scenario: Verify Device choices when "BYOD" is enabled
       Given I have a study with BYOD as "Enabled"
        And I am logged in as "PortalE2EUser"
        And I am on "At a Glance" page
        And I click on "Subject" link on the top navigation bar
        And I am on "Subject Information" page
        And I click on "Add New Subject" button
        And I am on "Add New Subject" page
        And the following text is displayed on the page
            |    Value                                                 |
            |Yes, subject will use their personal device               |
            |No, subject will use a provisioned device                 |
        When I enter the following data
            | Label                                       | Value                                       | FieldType    |
            | Subject Number                              | 123                                         | Numberinput  |
            | Gender                                      | Female                                      | Radio Button |
            | Date of Birth                               | CurrentDate                                 | datepicker   |
            | Weight                                      | 100.00                                      | Numberinput  |
            | Height                                      | 100.00                                      | Numberinput  |
        And I click on "Create" button
        Then "Warning" popup is displayed with message ""Will the subject use their personal device?" is a required field"
        And I click "Ok" button in the popup
        When I click on "Yes, subject will use their personal device" button
        And I click on "Create" button
        Then I am on "Enrollment Information" page