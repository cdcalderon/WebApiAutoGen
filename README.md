# YPrime.eCOA.Web

## Table of Contents
+ [About](#about)
+ [Getting Started](#getting_started)
+ [Contributing](#contrinuting)
+ [Release Notes](#release_notes)

## About <a name = "about"></a>
YPrime.eCOA.Web is a code repository for eCOA Web Portal and eCOA API. 

## Getting Started <a name = "getting_started"></a>

Clone the Repository using any git command-line or UI tool. 

### <a name="prerequisites"></a>Prerequisites

+ Visual Studio 2017/2019
+ .NET Core 


The following Nuget Sources need to be added on your Visual Studio Package Manager Sources

+ Nuget - https://api.nuget.org/v3/index.json
+ YPrime Legacy Nuget Source - http://yprimenuget.eclinicalcloud.net/nuget
+ YPrime DevOps Nuget Source - https://y-prime.pkgs.visualstudio.com/_packaging/YPrime.Nuget/nuget/v3/index.json

### Installing

The YPrime DevOps Nuget Source needs a username (Typically your yprime email) and a password ( Personal Access Token ).
<br/>
You can use the following link to set up a Personal Access Token in Azure DevOps https://docs.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/pats?view=azure-devops
<br/>
Note: Make sure that the Personal Access Token has been granted access to "Read Packaging"

### Deploying <a name = "deployment"></a>
Use Azure Pipelines for building and deployment.

## Contributing <a name = "contributing"></a>
Pull requests are welcome. For all changes, please open a work item/bug and link the commits to the work item.
Please coordinate with the Product Manager for visibilty and to avoid duplication of tasks.

Please make sure to update tests (Unit Tests and E2E) as appropriate.

## Release Notes <a name = "release_notes"></a>

#### **Release 5.0.0.383**
> What's New in Release 5.0.0.383:
> + Study Builder Configuration Versioning Support
> + Added YPrime.Core.BusinessLayer project for Study Builder API calls
> + Added YPrime.Web.E2E for E2E testing
> + Deprecated unused repositories and data models
> + <a href='https://yprime.atlassian.net/wiki/spaces/TB/pages/811270366/Release+5.0+Overview'>Release 5.0 Overview</a>
> + <a href='https://yprime.atlassian.net/wiki/spaces/PE/pages/781221897/DevOps+-+CI+CD+Pipelines'>Release 5.0 CI/CD Documentation</a>

## Development Notes

### Custom Extensions
Starting in `Release 5.0.0`, there is an option to include custom properties with the `Questionnaire`, `Question`, `Choice`, and `Visit` entities in the StudyBuilder. In the base release, these will be parsed by the device and stored in associated tables with an autoincremented integer ID. It is not recommended to create any foreign key references to this autoincremented ID, as it will be regenerated for every new configuration release (and will also not necessarily be consistent from device to device). It is instead recommened to reference either the custom extension `Name` or the `Value` field from other code/data instead.