using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class WidgetSystemActionConfiguration
    {
        public WidgetSystemActionConfiguration(EntityTypeConfiguration<WidgetSystemAction> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'WidgetSystemAction Seeding'");

            context.WidgetSystemActions.AddOrUpdate(dt => dt.Id,
                /*
                new WidgetSystemAction()
                {
                    //    Name = "DrugTypeWidget",
                    Id = Guid.Parse("A7266341-8A2D-470D-9ABC-12495B3051D7"),
                    SystemActionId = Guid.Parse("612BF889-7408-403C-998E-51D375D9D81F"),
                    WidgetId = Guid.Parse("B8249F24-47AC-4717-99AD-4936879C287C")
                },
                new WidgetSystemAction()
                {
                    //    Name = "RandomizationScheduleApprovalWidget",
                    Id = Guid.Parse("C533CE09-4F74-4830-A22C-3653964E39B6"),
                    SystemActionId = Guid.Parse("9C936D88-2741-4F16-AE35-8842438238BB"),
                    WidgetId = Guid.Parse("B1BEA9F7-35EE-42C6-BB1A-DFC75B1FC295")
                },
                new WidgetSystemAction()
                {
                    //    Name = "DrugDispensationWidget",
                    Id = Guid.Parse("F6BFBFB6-D126-4A04-8036-39D738B71C5D"),
                    SystemActionId = Guid.Parse("F4A8B416-4149-433B-89FB-0DC42D95F857"),
                    WidgetId = Guid.Parse("BF3AF407-5D6F-467A-87DC-5BDDFAEFF3B5")
                },
                new WidgetSystemAction()
                {
                    //    Name = "DoseLevelsWidget",
                    Id = Guid.Parse("7EEA6204-7EC7-42D0-8029-551B3C47ADE4"),
                    SystemActionId = Guid.Parse("4090FFC9-6AD2-44A9-A044-58F6590D728F"),
                    WidgetId = Guid.Parse("4E8311E4-332F-45DA-9595-A1A7A620DB4E")
                },
                new WidgetSystemAction()
                {
                    //    Name = "LabelGroupWidget",
                    Id = Guid.Parse("C3C6F0FB-0AF9-4170-9FD1-612353BAFA2E"),
                    SystemActionId = Guid.Parse("B1D52FE8-DEDA-473A-B558-8D0B4BE5A186"),
                    WidgetId = Guid.Parse("805F141C-CFA1-4D86-AEE8-913CF1B322EB")
                },
                new WidgetSystemAction()
                {
                    //    Name = "DepotManagementWidget",
                    Id = Guid.Parse("E789D27E-215B-4A11-8837-86A6D3A71DF8"),
                    SystemActionId = Guid.Parse("90D98154-DE6B-46EB-A784-2B1EFFCE6E26"),
                    WidgetId = Guid.Parse("3B61BBC9-286B-4687-9B3C-EF3662986EAF")
                },
                new WidgetSystemAction()
                {
                    //    Name = "LotManagementWidget",
                    Id = Guid.Parse("59791F56-EF9C-4F83-BC7A-AA2F56D2EC21"),
                    SystemActionId = Guid.Parse("B75E7A8B-7449-4008-908B-E4EB02F621D5"),
                    WidgetId = Guid.Parse("079FEA6C-EA4C-41C6-A910-4FD59447DF0F")
                },
                new WidgetSystemAction()
                {
                    //    Name = "MaterialListApprovalWidget",
                    Id = Guid.Parse("DFC15805-DE00-4259-98C4-D6A04CC8CA9C"),
                    SystemActionId = Guid.Parse("5B8901B6-0140-4ABB-8253-86BADD78E52E"),
                    WidgetId = Guid.Parse("DDB249FB-BFD8-4550-8A24-96DFF62ECE0C")
                },
                new WidgetSystemAction()
                {
                    //    Name = "DrugAssignmentBatchReleaseWidget",
                    Id = Guid.Parse("ba653317-8971-4462-9c17-55a0243749b8"),
                    SystemActionId = Guid.Parse("80788308-146F-458D-B2C3-FFFE512CD0C3"),
                    WidgetId = Guid.Parse("46C0B3E6-97A6-403D-A3B4-ED4823799ADA")
                },
                */
                new WidgetSystemAction
                {
                    //Name = "OrdersToAcknowledgeWidget",
                    Id = Guid.Parse("36795115-35C2-44B3-9224-DC780F088673"),
                    SystemActionId = Guid.Parse("F23B2BDB-BDF4-45CF-AE44-D0232DD0291D"),
                    WidgetId = Guid.Parse("5A8AD7DF-49E8-46A4-94DD-DAF98A64EA93")
                },
                new WidgetSystemAction
                {
                    //Name = "OrdersToAcknowledgeWidget",
                    Id = Guid.Parse("DA91530E-93A7-4E17-B492-E322EDF42E1A"),
                    SystemActionId = Guid.Parse("C23F6381-C540-47E1-98EB-A2728819293D"),
                    WidgetId = Guid.Parse("5A8AD7DF-49E8-46A4-94DD-DAF98A64EA93")
                },
                new WidgetSystemAction
                {
                    //Name = "OrdersToAcknowledgeWidget",
                    Id = Guid.Parse("101AAA4C-7637-43A3-AB23-524F90D73245"),
                    SystemActionId = Guid.Parse("f23b2bdb-bdf4-45cf-ae44-d0232dd0291d"),
                    WidgetId = Guid.Parse("5A8AD7DF-49E8-46A4-94DD-DAF98A64EA93")
                },
                new WidgetSystemAction
                {
                    //Name = "OrdersToAcknowledgeWidget",
                    Id = Guid.Parse("E15D8A66-297C-4DA7-B8F7-4EEB7692F17A"),
                    SystemActionId = Guid.Parse("A11A6C07-E384-4AEC-969D-2105A24C3AED"),
                    WidgetId = Guid.Parse("5A8AD7DF-49E8-46A4-94DD-DAF98A64EA93")
                },
                new WidgetSystemAction
                {
                    //Name = "ScreeningMilestoneWidget",
                    Id = Guid.Parse("3AECD7B8-1D0A-4D37-BE89-C560CDCC8133"),
                    SystemActionId = Guid.Parse("BA9B546A-54C0-425B-92A9-B6186638334E"),
                    WidgetId = Guid.Parse("86087532-2F51-4750-94A8-2E3B109490B1")
                },
                new WidgetSystemAction
                {
                    //Name = "RandomizationMilestoneWidget",
                    Id = Guid.Parse("BCE3C7DC-FCB1-4560-9F07-65136DC4DBCE"),
                    SystemActionId = Guid.Parse("65A2157C-4357-4220-A62D-C9E263D2C0B0"),
                    WidgetId = Guid.Parse("26DBD097-174A-4508-81E2-2C3DE3540AA9")
                },
                new WidgetSystemAction
                {
                    //Name = "ScreenFailMilestoneWidget",
                    Id = Guid.Parse("C51602B9-556B-4FD6-8710-F85063B011A0"),
                    SystemActionId = Guid.Parse("4381ED19-B2E5-4914-9B42-88B5382F00A4"),
                    WidgetId = Guid.Parse("0E402619-8C16-4B97-A128-542AE177F582")
                },
                new WidgetSystemAction
                {
                    //Name = "CompletedMilestoneWidget",
                    Id = Guid.Parse("22E45791-391E-4C21-81C2-7AEE69D46201"),
                    SystemActionId = Guid.Parse("7928361A-849D-47DE-B5BD-5D16FAE9B7A3"),
                    WidgetId = Guid.Parse("816BD9F4-92DF-43F7-9BFF-5C8BB0B49E3C")
                },
                new WidgetSystemAction
                {
                    //Name = "DiscontinuedMilestoneWidget",
                    Id = Guid.Parse("64ED1FF1-6702-46DE-A12F-ED38455D109C"),
                    SystemActionId = Guid.Parse("446C1D0E-DB42-44FA-926A-4B9B52BA19C9"),
                    WidgetId = Guid.Parse("A63BE5C8-E670-46F3-8573-027E39DEDACE")
                },
                new WidgetSystemAction
                {
                    Id = Guid.Parse("78E967E9-D352-4AAF-BF87-BB24463DC4F9"),
                    SystemActionId = Guid.Parse("E2208120-DD0A-46E6-BD86-2257E961EE6A"),
                    WidgetId = Guid.Parse("EE0A8882-7D03-4C4A-9898-AF80D568C630")
                });

            context.SaveChanges();
        }
    }
}

/*************
 * 
 * USE THIS TO AUTO GENERATE
 * 
 * select
	'new WidgetSystemAction() { Id = Guid.Parse("' + cast(id as nvarchar(max)) + '"), SystemActionId = Guid.Parse("' + cast(systemactionid as nvarchar(max)) + '"), WidgetId = Guid.Parse("' + cast(widgetid as nvarchar(max)) + '")},' 
	,* 
from WidgetSystemAction

 * 
 * 
 * *********************/