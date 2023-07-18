@UploadReferenceMaterial
Feature: UploadReferenceMaterial
#NLA-140 Upload reference Material

Background: 
    Given I am logged in as "PortalE2EUser"
    And I am on "At a Glance" page
    And I click on "Manage Study" on the top navigation bar
    And I click on "Upload Reference Material" link

 #1  
 #REQ URM.02
 #Name: Displays default, “New Reference Material” until text box is selected. User is able to enter unique naming as desired.
 #If a duplicate file is attempted to be uploaded, red text will display underneath "Upload Reference Material" header which states, "The file [insert file name here] already exists."
 #User: The user field will be automatically completed and unable to be changed. User field will default to the logged in users’ Portal username.
 #Type: Displays default, “Frequently Asked Questions”, upon selection of text box a drop down with additional selections is available. Additional selections include, “Training Videos” and “Training Manuals”
 #Add New: Upon selection of “Add New” the  message "Reference Material 'TestFile' was successfully uploaded. will display with Ok button.
 #All fields within the Upload Reference Material section are mandatory and must be completed prior to being able to add a new file.
 #Select a file: Upon selection, a box will appear allowing the user to browse within their computer for the specific file they would like to upload. Upon upload of a file, the “select a file” box will be updated to “Change file. If a user attempts to upload a file without having a file selected, an error message will display on the screen, “please select a file.”
 @MockStudyBuilder
Scenario Outline:Validate upload reference material section fields and success message when user add new reference material.
     Given I am on "Upload Reference Material" page
     And following labels are displayed
        | Label  | 
        | Name   |
        | User   |
        | Type   |
        | Upload |
     And the following choices are displayed in "ReferenceMaterialTypeId" dropdown
        | Value                      |
        | Frequently Asked Questions |
        | Training Manuals           |
        | Training Videos            |
     And "Frequently Asked Questions" value is "Visible"
     And "User" field is disabled
     And "PortalE2EUser" is displayed in "User" textbox field
     And "New Reference Material" displayed in "Name" placeholder
     And "Select a file" button is displayed
     When I click on "Add New" button
     Then "Please select a file to upload" text is displayed 
     And "Please enter a file name." text is displayed 
     And I enter "<FileName>" for "Name"
     And I click on "Add New" button
     And "Please select a file to upload" text is displayed 
     And I upload a file with file format "<FileFormat>"
     And "Change file" button is displayed
     When I click on "Add New" button
     Then "Info" popup is displayed with message "Reference Material '<FileName>' was successfully uploaded."
     And I click "Ok" button in the popup
     And popup is dismissed
     And following data is displayed in "Reference Material" Grid
     | Name       | User          | Type                       | Created Time | Action |
     | <FileName> | PortalE2EUser | Frequently Asked Questions | Current Date | Delete |
     And I enter "<FileName>" for "Name"
     And I upload existing file with file format "<FileFormat>"
     And I click on "Add New" button
     And "The name '<FileName>' already exists." text is displayed 
     And "<DuplicateFileErrorMessage>" text is displayed
     And I click on "Delete" link
     And "Confirm Delete" popup is displayed with message "Are you sure you want to delete the ReferenceMaterial '<FileName>'? "
     And I click "Ok" button in the popup

     Examples: 
     | FileFormat | DuplicateFileErrorMessage               | FileName  |
     | PDF        | The file 'PDF_File.pdf' already exists. | UploadPDF |
     | MP4        | The file 'MP4_File.mp4' already exists. | UploadMP4 |


 #2
 #REQ URM.04
 #File formats support for upload:PDF and MP4
Scenario: Validate error message when user upload invaild file.
     Given I am on "Upload Reference Material" page
     And I enter "Upload123" for "Name"
     And I upload a file with file format "PNG"
     When I click on "Add New" button
     Then "File 'PNG_File.png' is not supported for upload. (.pdf.mp4 Only)" is displayed for invalid file

 #3
 #REQ URM.03
 @MockStudyBuilder
 # Name: Displays a listing of the Name of all files uploaded as created within URM.02 (a. Name).
 # User: Displays a listing of all users which uploaded the associated documents to the Reference Materials section of Portal. Refers to selection within URM.02 (b. User).
 # Type: Displays a listing of the types of reference materials uploaded as selected within URM.02 (c. Type).
 # Created Time: Displays the time of file upload, DD-Mmm-YYYY HH:MM.
 # Action: Allows out of date or irrelevant reference material previously uploaded to be removed from the reference material listing. If selected, a confirmation message, "Are you sure you wish to delete [file name]" will occur.
 #The number associated with the Reference Materials display tab (screen below) will be a total count of uploaded reference materials available.
 # Column selction icon: Allows for columns that display within Completed Exports Section to be customized by individual user by “checking” and “unchecking” the header options listed below.
Scenario: Validate reference material table
     Given I am on "Upload Reference Material" page
     And I enter "UploadPDF" for "Name"
     And I upload a file with file format "PDF"
     And I click on "Add New" button
     When I click "Ok" button in the popup
     Then following data is displayed in "Reference Material" Grid
     | Name      | User          | Type                       | Created Time | Action |
     | UploadPDF | PortalE2EUser | Frequently Asked Questions | Current Date | Delete |
     And I click on "GridMenuicon" button
     And grid menu is displayed with the following functionality for export
        | ButtonName	|
		| Excel			|
		| CSV 		    |
		| PDF      		|
		| Print			|
      And grid menu is displayed with the following functionality for Visibilty
        | ButtonName   |
        | Name         |
        | User         |
        | Type         |
        | Created Time |
        | Action       |
     And I click on "Created Time" link
     And I click on the background screen
     And "Created Time" column of reference material grid is not visible
     And following data is displayed in "Reference Material" Grid
     | Name      | User          | Type                       | Action |
     | UploadPDF | PortalE2EUser | Frequently Asked Questions | Delete |
     And "1" entries under the table grid displayed in "Reference Materials tab" 
     And I click on "Delete" link
     And "Confirm Delete" popup is displayed with message "Are you sure you want to delete the ReferenceMaterial 'UploadPDF'?" 
     And I click "Cancel" button in the popup
     And popup is dismissed
     And I click on "Delete" link
     And "Confirm Delete" popup is displayed with message "Are you sure you want to delete the ReferenceMaterial 'UploadPDF'? "
     And I click "Ok" button in the popup
     And "Info" popup is displayed with message "Reference Material 'UploadPDF' was successfully deleted."
     And I click "Ok" button in the popup
     And popup is dismissed
     And "UploadPDF" is not visible in the grid list
     And "0" entries under the table grid displayed in "Reference Materials tab" 
     
   
     

