//using System;
//using System.Collections.Generic;
//using System.Data.Entity.Migrations;
//using System.Data.Entity.ModelConfiguration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YPrime.Data.Study.Models;

//namespace YPrime.Data.Study.Configurations.Entity
//{
//    public class WorkflowSequenceTypeConfiguration
//    {
//        public WorkflowSequenceTypeConfiguration(EntityTypeConfiguration<WorkflowSequenceType> entityConfiguration)
//        {
//            entityConfiguration.Property(e => e.Id).IsRequired();
//        }

//        public static void Seed(StudyDbContext context)
//        {
//            context.Database.ExecuteSqlCommand("PRINT 'Workflow Sequence Type Seeding'");

//            context.WorkflowSequenceTypes.AddOrUpdate(dt => dt.Id,
//                new WorkflowSequenceType() { Id = Guid.Parse("CAEF5348-51C8-42EB-A3ED-4C6217EDB5C4"), Name = "Parallel" },
//                new WorkflowSequenceType() { Id = Guid.Parse("3C523585-B0CE-4F10-8EA6-B883A315015A"), Name = "Serial" }
//                );

//            context.SaveChanges();
//        }
//    }
//}

