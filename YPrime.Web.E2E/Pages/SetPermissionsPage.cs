using System.Collections.Generic;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models;

namespace YPrime.Web.E2E.Pages
{
    [Binding]
    public class SetPermissionsPage : BasePage
    {

        public readonly E2ESettings e2eSettings;
        public readonly E2ERepository e2eRepository;
        public readonly TestData testData;

        public override string PageUrl => $"{e2eSettings.PortalUrl}/Role/SetPermissions/{testData.SelectedRole}";

        public override string PageName => "Manage Role Permissions";

        public override string GetDropdownSelectedValue(string control)
        {
            throw new System.NotImplementedException();
        }

        public SetPermissionsPage(
            E2ESettings e2eSettings,
            ScenarioService scenarioService,
            E2ERepository e2eRepository,
            TestData testData)
            : base(scenarioService)
        {
            this.e2eSettings = e2eSettings;
            this.e2eRepository = e2eRepository;
            this.testData = testData;
        }

        public override List<FieldMap> FieldMaps()
        {
            var systemActions = e2eRepository.GetSystemActionIds();
            var fieldMaps = new List<FieldMap>();

            foreach (var systemAction in systemActions)
            {
                var map = new FieldMap() { FieldType = "Toggle", Label = systemAction.Description.ToUpper(), Id = $"toggle-{systemAction.Id}", UiControl = "toggle" };
                fieldMaps.Add(map);
            }
            
            return fieldMaps;
        }
    }
}