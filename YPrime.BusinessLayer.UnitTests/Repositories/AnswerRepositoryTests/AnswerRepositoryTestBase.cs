using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.AnswerRepositoryTests
{
    [TestClass]
    public abstract class AnswerRepositoryTestBase
    {
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly AnswerRepository Repository;

        protected readonly Mock<IQuestionnaireService> MockQuestionnaireService;
        protected readonly Mock<ITranslationService> MockTranslationService;

        protected AnswerRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();

            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockTranslationService = new Mock<ITranslationService>();        

            Repository = new AnswerRepository(
                Context.Object,
                MockQuestionnaireService.Object,
                MockTranslationService.Object,
                new Mock<IFileService>().Object);
        }

        protected void SetupData(
            IEnumerable<Answer> answers,
            IEnumerable<DiaryEntry> diaryEntries,
            IEnumerable<QuestionChoiceModel> choices,
            IEnumerable<QuestionModel> questions,
            IEnumerable<DiaryPageModel> diaryPages)
        {
            
            MockTranslationService.Reset();

            var dbAnswerSet = new FakeDbSet<Answer>(answers);
            var dbDiarEntrySet = new FakeDbSet<DiaryEntry>(diaryEntries);

            Context.Setup(ctx => ctx.Answers)
                .Returns(dbAnswerSet.Object);

            Context.Setup(ctx => ctx.DiaryEntries)
                .Returns(dbDiarEntrySet.Object);
            MockQuestionnaireService.Reset();

            foreach (var question in questions)
            {
                question.Choices = choices
                    .Where(c => c.QuestionId == question.Id)
                    .ToList();
            }            

            MockQuestionnaireService
                .Setup(s => s.GetQuestions(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(questions.ToList());

            MockQuestionnaireService
                .Setup(s => s.GetInflatedQuestionnaire(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(new QuestionnaireModel
                {
                    Pages = new List<DiaryPageModel>
                    {
                        new DiaryPageModel
                        {
                            Questions = questions.ToList()
                        }
                    }
                });
        }
    }
}
