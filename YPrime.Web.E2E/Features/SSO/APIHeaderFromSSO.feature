@ApiTest
Feature: API Header From SSO
	
Background: 
 Given Site "Site 1" is assigned to Country "United States" and has site number "100000"

@ID:140
#1
Scenario: If the header is present in the API call, then the Audit table will show the correct User performing the action. 
	Given API request contains study user data
	 | StudyUserId | StudyUserName |
	 | Guid        | PortalE2EUser |
	And API request contains ids for study user role data
	 | StudyUser     | StudyRole | Site   |
	 | PortalE2EUser | YP        | Site 1 |
	And API request contains header
	| Header             | Value  |
	| YP-Study-User-ID   | "Guid" |
	When the POST request is made and the response is successful
	Then Study User Role audit table has new record for following data
	| ModifiedBy       | StudyUserId | StudyRoleId | SiteId | AuditAction |
	| YP-Study-User-ID | ypadmin     | YP          | Site 1 | A           |

@ID:140
#2
Scenario: If the header is updated in the API call, then the Audit table will show the updated User. 
	 Given API request contains study user data
	 | StudyUserId | StudyUserName |
	 | Guid        | PortalE2EUser |
	 And API request contains ids for study user role data
	 | StudyUser     | StudyRole | Site   |
	 | PortalE2EUser | YP        | Site 1 |
	 And API request contains header
	 | Header			| Value  |
	 | YP-Study-User-ID | "Guid" |
	 And the POST request is made and the response is successful
	 And Study User Role audit table has new record for following data
	 | ModifiedBy		| StudyUserId | StudyRoleId | SiteId | AuditAction |
	 | YP-Study-User-ID | ypadmin	  | YP			| Site 1 | A		   |

	 And API request contains study user data
	 | StudyUserId | StudyUserName |
	 | Guid        | PortalE2EUser |
	 And API request contains ids for study user role data
	 | StudyUser     | StudyRole | Site   |
	 | PortalE2EUser | IV        | Site 1 |
	 And API request contains header
	 | Header			| Value	 |
	 | YP-Study-User-ID | "Guid" |
	 When the POST request is made and the response is successful
	 Then Study User Role audit table has new record for following data
	 | ModifiedBy       | StudyUserId | StudyRoleId | SiteId | AuditAction |
	 | YP-Study-User-ID | ypadmin     | IV          | Site 1 | A           |
	 | YP-Study-User-ID | ypadmin     | YP          | Site 1 | D           |



@ID:140
#3
Scenario: If the header is not present in the API call, the Audit table will show "System".
	Given API request contains study user data
	  | StudyUserId | StudyUserName |
	  | Guid        | PortalE2EUser |
	And API request contains ids for study user role data
	 | StudyUser     | StudyRole | Site   |
	 | PortalE2EUser | YP        | Site 1 |
	And API request contains header 
	 | Header             | Value |
	 |				      |       |
	When the POST request is made and the response is successful
	Then Study User Role audit table has new record for following data
	| ModifiedBy | StudyUserId | StudyRoleId | SiteId | AuditAction |
	| System     | ypadmin     | YP          | Site 1 | A           |