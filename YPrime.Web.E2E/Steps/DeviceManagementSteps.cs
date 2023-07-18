using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Pages;

namespace YPrime.Web.E2E.Steps
{
    [Binding]
    public class DeviceManagementSteps
    {
        private readonly DeviceManagementPage deviceManagementPage;
        private readonly ScenarioService scenarioService;
        private readonly E2ERepository e2eRepository;

        public DeviceManagementSteps(DeviceManagementPage deviceManagementPage,
            ScenarioService scenarioService,
            E2ERepository e2eRepository)
        {
            this.deviceManagementPage = deviceManagementPage;
            this.e2eRepository = e2eRepository;
            this.scenarioService = scenarioService;
        }


        [Given(@"""(.*)"" Device ""(.*)"" last sync date is \(Current Date\) with sync Action ""(.*)""")]
        public void GivenDeviceLastSyncDateIsCurrentDate(string deviceType, string assetTag, string syncAction)
        {
            e2eRepository.SetDeviceLastSyncDate(assetTag, syncAction);
        }

        [Given("I have all the correct permissons to view Device Management")]
        public async Task GivenAllPermissionsAreSetForDeviceManagementAsync()
        {
            List<string> deviceManagementPermissions = new List<string>() {"CAN VIEW THE LIST OF DEVICES IN THE SYSTEM.", "CAN VIEW THE DETAILS PAGE FOR A DEVICE ." };

            foreach(var permission in deviceManagementPermissions)
            {
                await e2eRepository.EnablePermissionForRole(permission);
            }
        }


        [Given(@"I click on ""Hamburger icon"" in ""Device Inventory"" widget")]
        public void GivenIClickOnHamburgerIcon()
        {
            deviceManagementPage.GetWebElementById("view-device-icon").Click();
        }

        [Given(@"I click on View Devices button under Hamburger icon")]
        [When(@"I click on View Devices button under Hamburger icon")]
        [Then(@"I click on View Devices button under Hamburger icon")]
        public void GivenIClickOnViewDevices()
        {
            deviceManagementPage.GetWebElementById("view-device-link").Click();

        }

     


        [Given(@"following data is displayed in the grid")]
        [Then(@"following data is displayed in the grid")]
        public void ThenFollowingDataIsDisplayedInTheGrid(Table table)
        {
            foreach (var row in table.Rows)
            {
                var siteName = row["Site Name"];
                var deviceName = row["Device Name"];

                IWebElement elem = null;
                foreach (var element in scenarioService.ChromeDriver.FindElements(By.CssSelector("tr td:nth-child(2)")))
                {
                    var text = element.Text;
                    if (text == deviceName)
                    {
                        elem = element;
                        break;
                    }
                }

                Assert.IsNotNull(elem);

                var rowElem = elem.FindElement(By.XPath(".."));
                var deviceType = row["Device Type"];
                var releaseName = row["Release Name"];
                var softwareVersion = row["Software Version"];
                var configurationVersion = row["Configuration Version"];
                var lastReportedSoftware = row["Last Reported Software Version"];
                var lastReportedConfiguration = row["Last Reported configuration Version"];
                var lastDataSync = row["Last Data Sync"];

                var index = 0;
                foreach (var columnElem in rowElem.FindElements(By.CssSelector("td")))
                {
                    var colText = columnElem.Text;
                    var elemFull = columnElem.TagName;
                    switch (index)
                    {
                        case 0:
                            Assert.AreEqual(siteName.ToUpper(), colText.ToUpper(), "Site Name");
                            break;
                        case 1:
                            Assert.AreEqual(deviceName.ToUpper(), colText.ToUpper(), "Device Name");
                            break;
                        case 2:
                            Assert.AreEqual(deviceType.ToUpper(), colText.ToUpper(), "Device Type");
                            break;
                        case 3:
                            Assert.AreEqual(releaseName.ToUpper(), colText.ToUpper(), "Release Name");
                            break;
                        case 4:
                            Assert.AreEqual(softwareVersion.ToUpper(), colText.ToUpper(), "Software Version");
                            break;
                        case 5:
                            Assert.AreEqual(configurationVersion.ToUpper(), colText.ToUpper(), "Configuration Version");
                            break;
                        case 6:
                            Assert.AreEqual(lastReportedSoftware.ToUpper(), colText.ToUpper(), "Last Reported Software Version");
                            break;
                        case 7:
                            Assert.AreEqual(lastReportedConfiguration.ToUpper(), colText.ToUpper(), "Last Reported Configuration Version");
                            break;
                        case 8:
                            if (lastDataSync == "(Current Date)")
                            {
                                DateTime dataDate;
                                DateTime.TryParse(colText, out dataDate);
                                var dateNow = DateTime.UtcNow.Date;
                                var syncDate = dataDate.ToUniversalTime().Date;
                                Assert.IsTrue(syncDate.ToString("dd-MMM-y").Equals(dateNow.ToString("dd-MMM-y")), $"Last Data Sync Date: {syncDate} should equal to Current Date: {dateNow}");
                            }
                            else
                            {
                                Assert.IsTrue(string.IsNullOrWhiteSpace(colText), colText);
                            }
                            break;
                        default:
                            break;
                    }
                    index++;
                }
            }
        }

        [Then(@"""(.*)"" Devices is displayed for ""(.*)""")]
        [Given(@"""(.*)"" Devices is displayed for ""(.*)""")]
        [When(@"""(.*)"" Devices is displayed for ""(.*)""")]
        public void ThenDevicesIsDisplayedFor(string count, string device)
        {
            if (device.Equals("Phone"))
            {
                Assert.AreEqual(deviceManagementPage.GetWebElementById("phoneDeviceCount").Text, count);
            }
            else if (device.Equals("Tablet"))
            {
                Assert.AreEqual(deviceManagementPage.GetWebElementById("tabletDeviceCount").Text, count);
            }
            else
            {
                Assert.AreEqual(deviceManagementPage.GetWebElementById("totalDeviceCount").Text, count);
            }

        }


        [Then(@"Device ""(.*)"" is assigned to Software Version ""(.*)"" and Configuration Version ""(.*)""")]
        public async Task ThenSoftwareReleaseIsVerifiedForDevice(string deviceName, string softwareVersion, string configurationVersion)
        {
            IWebElement elem = null;
            foreach (var element in scenarioService.ChromeDriver.FindElements(By.CssSelector("tr td:nth-child(2)")))
            {
                var text = element.Text;
                if (text == deviceName)
                {
                    elem = element;
                    break;
                }
            }

            var index = 0;
            var rowElem = elem.FindElement(By.XPath(".."));
            foreach (var columnElem in rowElem.FindElements(By.CssSelector("td")))
            {
                var colText = columnElem.Text;
                switch (index)
                {
                    case 1:
                        Assert.AreEqual(deviceName.ToUpper(), colText.ToUpper(), "Device Name");
                        break;
                    case 4:
                        Assert.AreEqual(softwareVersion.ToUpper(), colText.ToUpper(), "Software Version");
                        break;
                    case 5:
                        Assert.AreEqual(configurationVersion.ToUpper(), colText.ToUpper(), "Configuration Version");
                        break;
                    default:
                        break;
                }
                index++;
            }
        }
    }
}
