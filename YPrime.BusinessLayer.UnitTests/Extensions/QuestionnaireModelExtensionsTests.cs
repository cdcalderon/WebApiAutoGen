using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class QuestionnaireModelExtensionsTests
    {
        [TestMethod]
        public void QuestionnaireModelExtensionsHasRequiredVisitTrueTest()
        {
            var questionnaire = new QuestionnaireModel
            {
                QuestionnaireTypeId = QuestionnaireType.Clinician.Id
            };

            Assert.IsTrue(questionnaire.TypeRequiresVisit());
        }

        [TestMethod]
        public void QuestionnaireModelExtensionsHasRequiredVisitFalseTest()
        {
            var questionnaire = new QuestionnaireModel
            {
                QuestionnaireTypeId = QuestionnaireType.PatientHandheld.Id
            };

            Assert.IsFalse(questionnaire.TypeRequiresVisit());
        }

        [TestMethod]
        public void QuestionnaireModelExtensionsHasRequiredVisitNullTest()
        {
            QuestionnaireModel questionnaire = null;

            Assert.IsFalse(questionnaire.TypeRequiresVisit());
        }

        [TestMethod]
        public void QuestionnaireModelExtensionsGetSortedquestionsTest()
        {
            var firstPageQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 0
            };

            var firstPageQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1
            };

            var firstPage = new DiaryPageModel
            {
                Number = 1,
                Questions = new List<QuestionModel>
                {
                    firstPageQuestion1,
                    firstPageQuestion2
                }
            };

            var secondPageQuestion1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 0
            };

            var secondPageQuestion2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1
            };

            var secondPage = new DiaryPageModel
            {
                Number = 2,
                Questions = new List<QuestionModel>
                {
                    secondPageQuestion2,
                    secondPageQuestion1
                }
            };

            var questionnaire = new QuestionnaireModel
            {
                Pages = new List<DiaryPageModel>
                {
                    firstPage,
                    secondPage
                }
            };

            var result = questionnaire.GetSortedquestions();

            Assert.AreEqual(firstPageQuestion1.Id, result.First().Id);
            Assert.AreEqual(firstPageQuestion2.Id, result.Skip(1).First().Id);
            Assert.AreEqual(secondPageQuestion1.Id, result.Skip(2).First().Id);
            Assert.AreEqual(secondPageQuestion2.Id, result.Skip(3).First().Id);
        }

        [TestMethod]
        public void QuestionnaireModelExtensionsGetSortedquestionsNullTest()
        {
            var questionnaire = new QuestionnaireModel
            {
                Pages = null
            };

            var results = questionnaire.GetSortedquestions();

            Assert.IsNotNull(results);
            Assert.IsFalse(results.Any());
        }

        [TestMethod]
        public void RemoveNonDcfQuestionsTest()
        {
            var question1 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionType = InputFieldType.DurationSpinner.Id
            };

            var question2 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionType = InputFieldType.Camera.Id
            };

            var question3 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionType = InputFieldType.Checkbox.Id
            };

            var question4 = new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionType = InputFieldType.NumberSpinner.Id
            };

            var page = new DiaryPageModel
            {
                Number = 1,
                Questions = new List<QuestionModel>
                {
                    question1,
                    question2,
                    question3,
                    question4
                }
            };

            var questionnaire = new QuestionnaireModel
            {
               Pages = new List<DiaryPageModel>
               { 
                   page
               }
            };

            questionnaire.RemoveNonDcfQuestions();

            Assert.IsTrue(questionnaire.Pages.First().Questions.Any(q => q.Id == question1.Id));
            Assert.IsFalse(questionnaire.Pages.First().Questions.Any(q => q.Id == question2.Id));
            Assert.IsTrue(questionnaire.Pages.First().Questions.Any(q => q.Id == question3.Id));
            Assert.IsTrue(questionnaire.Pages.First().Questions.Any(q => q.Id == question4.Id));
        }

        [TestMethod]
        public void RemoveNonDcfQuestionsNullTest()
        {
            var questionnaire = new QuestionnaireModel
            {
                Pages = null
            };

            questionnaire.RemoveNonDcfQuestions();

            Assert.IsNull(questionnaire.Pages);
        }
    }
}