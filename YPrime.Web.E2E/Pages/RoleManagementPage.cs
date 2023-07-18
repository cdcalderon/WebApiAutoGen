using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class RoleManagementPage : BasePage
    {
        public readonly E2ESettings e2eSettings;
        public readonly E2ERepository e2eRepository;

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Role";

        public override string PageName => "Role Management";

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

        public RoleManagementPage(E2ESettings e2eSettings, ScenarioService scenarioService, E2ERepository e2eRepository) : base(scenarioService) 
        {
            this.e2eSettings = e2eSettings;
            this.e2eRepository = e2eRepository;
            
        }

        public override List<FieldMap> FieldMaps()
        {

            return new List<FieldMap>
            {
                new FieldMap() { FieldType = "Button", Label = "CAN CREATE CAREGIVER IN PORTAL", Id = "CanCreateCaregiverInPortal", UiControl = "toggle" },
                new FieldMap() { FieldType = "Button", Label = "roles", Id = "roles", UiControl = "button" },
            };
        }

        public void updatePermission(string action, string permission)
        {
            var togglebutton = ScenarioService.ChromeDriver.FindElement(By.XPath("//*[text()=\""+ permission+"\"]//following-sibling::span//label"));
            var elementStatus = ScenarioService.ChromeDriver.FindElement(By.XPath("//label[text()=\"" + permission + "\"]//parent::h4//input"));
            switch (action)
            {
                case "Enable":
                    if (!elementStatus.Selected)
                        togglebutton.Click();
                    else
                        Assert.Fail("Toggle is already enabled");
                    break;
                case "Disable":
                    if (elementStatus.Selected)
                    {
                        togglebutton.Click();
                    }
                    else
                    {

                        Assert.Fail("Toggle is already disabled");
                    }
                    break;
            }
        }

        public void columnsAreDisplayedForGridMenu(string grid, Table table)
        {

           
        }
    }
}
