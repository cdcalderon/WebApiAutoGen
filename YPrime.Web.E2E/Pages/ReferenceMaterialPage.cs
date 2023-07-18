using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using YPrime.Config.Enums;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class ReferenceMaterialPage : BasePage
    {

        public readonly E2ESettings e2eSettings;
        public readonly E2ERepository e2eRepository;

        public override string PageUrl => $"{e2eSettings.PortalUrl}/ReferenceMaterial";

        public override string PageName => "Reference Material";

        public ReferenceMaterialPage(E2ESettings e2eSettings, ScenarioService scenarioService, E2ERepository e2eRepository) : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            this.e2eRepository = e2eRepository;
        }


        public override List<FieldMap> FieldMaps() => new List<FieldMap>
        { new FieldMap() { FieldType = "header", Label = "Reference Material Header", Id = "heading_{}", UiControl = "table", WaitForElement = false },
         new FieldMap() { FieldType = "body", Label = "Reference Material Body", Id = "collapse_{}", UiControl = "table", WaitForElement = false } };
    

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }
        public Guid GetReferenceMaterialTypeId(string referenceMaterialType)
        {
            Guid referenceMaterialTypeID = Guid.Empty;
            switch (referenceMaterialType)
            {
                case "Frequently Asked Questions":
                    referenceMaterialTypeID = ReferenceMaterialType.FrequentlyAskedQuestions.Id;
                    break;
                case "Training Manuals":
                    referenceMaterialTypeID = ReferenceMaterialType.TrainingManuals.Id;
                    break;
                case "Training Videos":
                    referenceMaterialTypeID = ReferenceMaterialType.TrainingVideos.Id;
                    break;
            }
            return referenceMaterialTypeID;
        }
        public List<string> GetReferenceMaterialNames(string referenceMaterialType,string page)
        {
            var referenceMaterialTypeID = GetReferenceMaterialTypeId(referenceMaterialType);
            var tableBodyID = FieldMaps().Find(f => f.Label == $"{page} Body").Id.Replace("{}", referenceMaterialTypeID.ToString());
            var bodyElements = GetWebElementById(tableBodyID).FindElements(By.TagName("ul"));
            List<string> referenceMaterialNames = new List<string>();
            
            foreach (var element in bodyElements)
            {
                try
                {
                    referenceMaterialNames.Add(element.FindElement(By.TagName("li")).Text);
                }
                catch { }

            }
            return referenceMaterialNames;
        }
    }
}


