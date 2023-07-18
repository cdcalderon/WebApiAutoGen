using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.VisitRepositoryTests
{
    [TestClass]
    public class VisitRepositoryTests : LegacyTestBase
    {
        private Mock<MockableDbSetWithExtensions<PatientVisit>> _patientVisit;
        private Mock<MockableDbSetWithExtensions<DiaryEntry>> _diaryEntry;
        private List<VisitQuestionnaireModel> _visitQuestionnaire;
        private Mock<IStudyDbContext> _dbContext;
        protected Mock<IVisitService> VisitService;


        [TestInitialize]
        public void Initialize()
        {
            VisitService = new Mock<IVisitService>();
            var visits = new VisitModel
                {
                    Id = Guid.Parse("62345678-6659-1234-5454-123456789124"),
                    Name = "SV",
                    DaysExpected = 3,
                    WindowBefore = 6,
                    Notes = "Note",
                    LastUpdate = DateTime.Today,
                    WindowOverride = true,
                    OverrideReason = "reason",
                    IsScheduled = true,
                    VisitOrder = 3,
                    VisitAnchor = Guid.Parse("62345678-4444-1234-4444-123456744444"),
                    VisitStop_HSN = "visit stop",
                    Questionnaires = new List<VisitQuestionnaireModel>()
                    {
                         new VisitQuestionnaireModel
                        {
                            Sequence = 1,
                            QuestionnaireId = new Guid()
                        }
                    }               
            };            

            VisitService.Setup(s => s.GetAll(It.IsAny<Guid?>()))
               .ReturnsAsync(new List<VisitModel>
               {
                   visits
               });

            _patientVisit = CreateDbSetMock(new List<PatientVisit>
            {
                new PatientVisit
                {
                    Id = Guid.Parse("62345678-1234-1234-1234-123456789124"),
                    PatientId = Guid.Parse("12345678-1234-1234-1234-123456789124"),
                    ProjectedDate = new DateTimeOffset(2016, 11, 17, 16, 00, 30, TimeSpan.Zero),
                    VisitId = Guid.Parse("62345678-6659-1234-5454-123456789124"),                  
                }              
            });

            _diaryEntry = CreateDbSetMock(new List<DiaryEntry>
            {
                new DiaryEntry
                {
                    Id = Guid.Parse("32345678-1234-1234-1234-123456789124"),
                    DiaryStatusId = 1,
                    DiaryDate = new DateTime(2016, 11, 17),
                    DataSourceId = DataSource.IRT.Id,
                    PatientId = Guid.Parse("12345678-1234-1234-1234-123456789123"),
                    QuestionnaireId = Guid.Parse("42345678-1234-1234-1234-123456789123"),
                    VisitId = Guid.Parse("62345678-6659-1234-5454-123456789124"),                  
                    DeviceId = Guid.Parse("52345678-1234-1234-1234-123456789123"),
                    StartedTime = new DateTimeOffset(2016, 11, 17, 16, 00, 30, TimeSpan.Zero),
                    CompletedTime = new DateTimeOffset(2016, 11, 17, 16, 2, 30, TimeSpan.Zero),
                    TransmittedTime = new DateTimeOffset(1900, 1, 31, 15, 30, 0, TimeSpan.Zero)
                }
            });


            _visitQuestionnaire = new List<VisitQuestionnaireModel>
            {
                new VisitQuestionnaireModel
                {
                    Sequence = 1,
                    QuestionnaireId = new Guid()
                }
            };

            _dbContext = new Mock<IStudyDbContext>();
            _dbContext.Setup(x => x.PatientVisits).Returns(_patientVisit.Object);
            _dbContext.Setup(x => x.DiaryEntries).Returns(_diaryEntry.Object);         
        }


        [TestMethod]
        public void GetVisitTest()
        {
            var getVisits = VisitService.Object.GetAll(); 

            Assert.AreEqual(Guid.Parse("62345678-6659-1234-5454-123456789124"), getVisits.Result.Find(x => x.Id == Guid.Parse("62345678-6659-1234-5454-123456789124")).Id);
        }
    }
}