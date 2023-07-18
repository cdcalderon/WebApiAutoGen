Feature: Software And Configuration Release
    PBI 87838, 87839, 87840, 97445

    Background:
        Given I have logged in with user "PortalE2EUser"
        And I have all the correct permissons to view Device Management            
        And Site "Site 1" is assigned to Country "India" and has site number "100000"
        And Site "Site 2" is assigned to Country "China" and has site number "200000"
        And Site "Site 3" is assigned to Country "United States" and has site number "300000"
        And "Tablet" Device "Yp-MG000207" is assigned to Site "Site 1"        
        And "Phone" Device "Yp-M02313" is assigned to Site "Site 2"
        And "Tablet" Device "Yp-M0001" is assigned to Site "Site 3"
        And "Phone" Device "Yp-M0002" is assigned to Site "Site 3"    
        And "Phone" Device "YP-E2E-Device" is assigned to Site "Site 3"
        And The following Software Versions are assigned to study Yprime-Sandbox-e2e
            | Version  |
            | 4.1.0.95 |
            | 4.1.0.96 |
            | 4.1.0.97 |
            | 4.1.0.98 |
            | 4.1.0.99 |            
        And I am on Software Release Management page
        And Initial Release is displayed in grid
            | Release Date | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | 01-Jan-01    | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 5/5                      | 5/5                        |


    #1 #PBI 87838
    Scenario: Appropriate fields are displayed in Software Release Management page.
        Then "Release Name inputtextbox" has value ""
        And "Software Version" dropdown has placeholder "Please Select"
        And "Configuration Version" dropdown has placeholder "Please Select"

    
    #2 #PBI 87838
    Scenario: Configuration Version is displayed in Numerical order from greatest to smallest.
        When I click on "Configuration Version" dropdown
        Then "Configuration Version" is sorted from the "latest" to the "oldest"      
   

    #3 #PBI 87838
    Scenario: User is able to deselect a Configuration Version once selected.
        When I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "2.0-1.0" is selected value in "Configuration Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "Please Select" from "Configuration Version" dropdown
        Then "Please Select" placeholder is displayed in "Configuration Version" dropdown

    #4 #PBI 87838
    Scenario: User is able to deselect a Software Version once selected.
        When I click on "Software Version" dropdown
        And I select "0.0.0.1" from "Software Version" dropdown
        And "0.0.0.1" is selected value in "Software Version" dropdown
        And I click on "Software Version" dropdown
        And I select "Please Select" from "Software Version" dropdown
        Then "Please Select" placeholder is displayed in "Software Version" dropdown


    #5 #PBI 87838
    Scenario: Error message is displayed when mandatory field is left blank.
        When I click on "Create Release" button
        Then "Please enter a release name." validation message is displayed for "Release Name inputtextbox"
        And "Please select a software version." validation message is displayed for "Software Version" dropdown
        And "Please select a configuration version." validation message is displayed for "Configuration Version" dropdown

    #6 #PBI 97445
    Scenario: Verify that an error message is displayed when no fields are selected.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType | Message                                                   | ActionButtons |
            | Review    | Software release must be assigned to at least one device. | Cancel        |

    #7 #PBI 87839
    Scenario: Message for software and config version is displayed upon clicking Create Release button when study wide is assigned.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Device Type" dropdown is disabled
        And "Country(s)" dropdown is disabled
        And "Site(s)" dropdown is disabled
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 5 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.5 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |


    #8 #PBI 87839
    Scenario: Message for software and config version is displayed upon clicking Create Release when country is assigned.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And "Device Type" dropdown is empty
        And I click on "Country(s)" multi-select dropdown
        And I select "United States" from "Country(s)" multi-select dropdown
        And "Site(s)" dropdown is empty
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 3 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.3 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #9 #PBI 87839
    Scenario: Message for software and config version is displayed upon clicking Create Release button when site is assigned.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And "Device Type" dropdown is empty
        And "Country(s)" dropdown is empty
        And I click on "Site(s)" multi-select dropdown
        And I select "Site 3" from "Site(s)" multi-select dropdown
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 3 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.3 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #10 #PBI 97445
    Scenario: Verify that the site dropdown displays only sites that have that device type assigned.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Device Type" multi-select dropdown
        And I select "Tablet" from "Device Type" multi-select dropdown
        When I click on "Site(s)" multi-select dropdown
        Then "Site 1" value is displayed in "Site(s)" multi-select dropdown
        And "Site 2" value is not displayed in "Site(s)" multi-select dropdown

    #11 #PBI 97445
    Scenario: Verify that the country dropdown displays only countries that have that device type assigned.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Device Type" dropdown
        And I select "Phone" from "Device Type" multi-select dropdown
        When I click on "Country(s)" multi-select dropdown
        Then "China" value is displayed in "Country(s)" multi-select dropdown
        And "India" value is not displayed in "Country(s)" multi-select dropdown


    #12 #PBI 87839
    Scenario: Message for software and config message is displayed upon clicking the create release button when a device type is assigned
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.98" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Device Type" multi-select dropdown
        And I select "Tablet" from "Device Type" multi-select dropdown
        And "Site(s)" multi-select dropdown is empty
        And "Country(s)" multi-select dropdown is empty
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 2 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.2 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #13 #PBI 87840
    Scenario: Release is created successfully and is displayed in descending order.
        Given I enter "Release 1" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 5 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.5 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 1                 | 4.1.0.95         | 2.0-1.0               | ON     | false    | true       |             |            |         | 5/0                      | 5/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.96" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "3.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Country(s)" multi-select dropdown
        And I select "India" from "Country(s)" multi-select dropdown
        And "Site(s)" multi-select dropdown is empty
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 1 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.1 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 2                 | 4.1.0.96         | 3.0-1.0               | ON     | false    | false      |             | India      |         | 1/0                      | 1/0                        |
            | (Current Date) | Release 1                 | 4.1.0.95         | 2.0-1.0               | ON     | false    | true       |             |            |         | 4/0                      | 4/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
        And I enter "Release 3" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.96" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "4.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And "Country(s)" multi-select dropdown is empty
        And I click on "Site(s)" multi-select dropdown
        And I select "Site 3" from "Site(s)" multi-select dropdown
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 3 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.3 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 3                 | 4.1.0.96         | 4.0-1.0               | ON     | false    | false      |             |            | Site 3  | 3/0                      | 3/0                        |
            | (Current Date) | Release 2                 | 4.1.0.96         | 3.0-1.0               | ON     | false    | false      |             | India      |         | 1/0                      | 1/0                        |
            | (Current Date) | Release 1                 | 4.1.0.95         | 2.0-1.0               | ON     | false    | true       |             |            |         | 1/0                      | 1/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
        And I enter "Release 4" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.98" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "6.0-01.00" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Device Type" multi-select dropdown
        And I select "Tablet" from "Device Type" multi-select dropdown
        And I select "Phone" from "Device Type" multi-select dropdown
        And "Country(s)" multi-select dropdown is empty
        And "Site(s)" multi-select dropdown is empty
        And  I click on "Create Release" button
        And  a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 5 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.5 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type  | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 4                 | 4.1.0.98         | 6.0-01.00             | ON     | false    | false      | Tablet,Phone |            |         | 5/0                      | 5/0                        |
            | (Current Date) | Release 3                 | 4.1.0.96         | 4.0-1.0               | ON     | false    | false      |              |            | Site 3  | 0/0                      | 0/0                        |
            | (Current Date) | Release 2                 | 4.1.0.96         | 3.0-1.0               | ON     | false    | false      |              | India      |         | 0/0                      | 0/0                        |
            | (Current Date) | Release 1                 | 4.1.0.95         | 2.0-1.0               | ON     | false    | true       |              |            |         | 0/0                      | 0/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |              |            |         | 0/0                      | 0/0                        |
        And I enter "Release 5" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.99" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "7.0-01.00" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Device Type" multi-select dropdown
        And I select "Tablet" from "Device Type" multi-select dropdown
        And I select "Phone" from "Device Type" multi-select dropdown
        And I click on "Country(s)" multi-select dropdown
        And I select "United States" from "Country(s)" multi-select dropdown
        And I click on "Site(s)" multi-select dropdown
        And I select "Site 3" from "Site(s)" multi-select dropdown
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 3 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.3 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |
        When I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type  | Country(s)    | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 5                 | 4.1.0.99         | 7.0-01.00             | ON     | false    | false      | Tablet,Phone | United States | Site 3  | 3/0                      | 3/0                        |
            | (Current Date) | Release 4                 | 4.1.0.98         | 6.0-01.00             | ON     | false    | false      | Tablet,Phone |               |         | 2/0                      | 2/0                        |
            | (Current Date) | Release 3                 | 4.1.0.96         | 4.0-1.0               | ON     | false    | false      |              |               | Site 3  | 0/0                      | 0/0                        |
            | (Current Date) | Release 2                 | 4.1.0.96         | 3.0-1.0               | ON     | false    | false      |              | India         |         | 0/0                      | 0/0                        |
            | (Current Date) | Release 1                 | 4.1.0.95         | 2.0-1.0               | ON     | false    | true       |              |               |         | 0/0                      | 0/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |              |               |         | 0/0                      | 0/0                        |


    #14 #PBI 87839
    Scenario: Message for software and config version is displayed when the device has higher software and configuration versions.
        Given Software Release "Release 4.1.0.95" has been created with Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-MG000207" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M02313" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0001" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "YP-E2E-Device" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 0 device(s) will be assigned to this Software Release.5 device(s) have a higher Software Release.0 device(s) will be assigned to this Configuration Release.5 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #15 #PBI 87839
    Scenario: Review message for software and config version is displayed when the device has higher software versions.
        Given Software Release "Release 4.1.0.96" has been created with Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And Device "Yp-MG000207" is assigned to Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And Device "Yp-M02313" is assigned to Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And Device "Yp-M0001" is assigned to Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And Device "YP-E2E-Device" is assigned to Software Version "4.1.0.96" and Configuration Version "1.0-1.0"
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.96" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 0 device(s) will be assigned to this Software Release.5 device(s) have a higher Software Release.5 device(s) will be assigned to this Configuration Release.0 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #16 #PBI 87839
    Scenario: Message for software and config version is displayed when the device has higher configuration version.
        Given Software Release "Release 4.1.0.96" has been created with Software Version "4.1.0.96" and Configuration Version "2.0-1.0"
        And Software Release "Release 4.1.0.97" has been created with Software Version "4.1.0.97" and Configuration Version "2.0-1.0"
        And Device "Yp-MG000207" is assigned to Software Version "4.1.0.96" and Configuration Version "2.0-1.0"
        And Device "Yp-M02313" is assigned to Software Version "4.1.0.96" and Configuration Version "2.0-1.0"
        And Device "Yp-M0001" is assigned to Software Version "4.1.0.97" and Configuration Version "2.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.97" and Configuration Version "2.0-1.0"
        And Device "YP-E2E-Device" is assigned to Software Version "4.1.0.96" and Configuration Version "2.0-1.0"
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.97" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        When I click on "Create Release" button
        Then a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 3 device(s) will be assigned to this Software Release.2 device(s) have a higher Software Release.0 device(s) will be assigned to this Configuration Release.5 device(s) have a higher Configuration Release. | Cancel, Confirm |

    #17 #PBI 87839
    Scenario: When user clicks Cancel user is taken to Software Release Management page and previously entered data is retained.
        Given Software Release "Release 4.1.0.95" has been created with Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-MG000207" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M02313" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0001" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "YP-E2E-Device" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.96" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 5 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.0 device(s) will be assigned to this Configuration Release.5 device(s) have a higher Configuration Release. | Cancel, Confirm |
        When I click on "Cancel" button
        Then "Release 2" is displayed for "Release Name inputtextbox"
        And "4.1.0.96" is displayed for "Software Version" dropdown
        And "Required" togglebutton is disabled
        And "2.0-1.0" is displayed for "Configuration Version" dropdown
        And "Study Wide" togglebutton is enabled
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled


    #18 #PBI 87839
    Scenario: When user clicks X user is taken to Software Release Management page and previously entered data is retained
        Given Software Release "Release 4.1.0.95" has been created with Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-MG000207" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M02313" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0001" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And Device "YP-E2E-Device" is assigned to Software Version "4.1.0.95" and Configuration Version "2.0-1.0"
        And I enter "Release 2" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.96" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "2.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled
        And I click on "Create Release" button
        And a popup is displayed
            | popupType                                 | Message                                                                                                                                                                                                      | ActionButtons   |
            | Review Software and Configuration Release | 5 device(s) will be assigned to this Software Release.0 device(s) have a higher Software Release.0 device(s) will be assigned to this Configuration Release.5 device(s) have a higher Configuration Release. | Cancel, Confirm |
        When I click on "X" button
        Then "Release 2" is displayed for "Release Name inputtextbox"
        And "4.1.0.96" is displayed for "Software Version" dropdown
        And "2.0-1.0" is displayed for "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is enabled
        And "Country(s)" multi-select dropdown is disabled
        And "Site(s)" multi-select dropdown is disabled

    #19 PBI 102801
    Scenario: Verify that when we create a release specific to a country(Germany), pick a higher config version, and then create another release with study wide and select a lower config version. When a subject for a country (USA) is created it should be assigned to a studywide release with a lower config version.
        Given Site "Site 9" is assigned to Country "Germany" and has site number "10002"
        And "Phone" Device "Yp-M0002" is assigned to Site "Site 9"
        And Subject "S-10001-005" is created
        And Site "Initial Site" is assigned to Country "United States" and has site number "10001"
        And "Phone" Device "Yp-M02313" is assigned to Site "Initial Site"
        And I enter "Release 8" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "5.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Country(s)" multi-select dropdown
        And I select "Germany" from "Country(s)" multi-select dropdown
        And I click on "Create Release" button
        And I click on "Confirm" button
        And I enter "Release 9" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.95" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "4.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And I click on "Create Release" button
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 9                 | 4.1.0.95         | 4.0-1.0               | ON     | false    | true       |             |            |         | 4/0                      | 4/0                        |
            | (Current Date) | Release 8                 | 4.1.0.95         | 5.0-1.0               | ON     | false    | false      |             | Germany    |         | 1/0                      | 1/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
        
        When I click View Devices link
        And I am on "Device Management" page
        Then Device "Yp-M02313" is assigned to Software Version "4.1.0.95" and Configuration Version "4.0-1.0"

    #20 PBI 108395
    Scenario: Verify that higher configuration version is still assigned even if the software version is lower than the version currently on the device.
        Given Software Release "Release 10" has been created with Software Version "4.1.0.98" and Configuration Version "2.0-1.0"
        And Device "Yp-M0002" is assigned to Software Version "4.1.0.98" and Configuration Version "2.0-1.0"
        And "Phone" Device "Yp-M0002" is assigned to Site "Initial Site"
        And I enter "Release 11" for "Release Name inputtextbox"
        And I click on "Software Version" dropdown
        And I select "4.1.0.97" from "Software Version" dropdown
        And I click on "Configuration Version" dropdown
        And I select "3.0-1.0" from "Configuration Version" dropdown
        And "Required" togglebutton is disabled
        And "Study Wide" togglebutton is disabled
        And I click on "Study Wide" togglebutton
        And I click on "Create Release" button
        And I click on "Confirm" button
        And the following data is added to the grid
            | Release Date   | Release Name              | Software Version | Configuration Version | Active | Required | Study Wide | Device Type | Country(s) | Site(s) | Assigned/Reported Config | Assigned/Reported Software |
            | (Current Date) | Release 11                | 4.1.0.97         | 3.0-1.0               | ON     | false    | true       |             |            |         | 5/0                      | 5/0                        |
            | (Current Date) | Release 10                | 4.1.0.98         | 2.0-1.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
            | 01-Jan-01      | Initial Software Release  | 0.0.0.1          | 0.0-0.0               | ON     | false    | true       |             |            |         | 0/0                      | 0/0                        |
        When I click View Devices link
        And I am on "Device Management" page
        Then Device "Yp-M0002" is assigned to Software Version "4.1.0.97" and Configuration Version "3.0-1.0"
