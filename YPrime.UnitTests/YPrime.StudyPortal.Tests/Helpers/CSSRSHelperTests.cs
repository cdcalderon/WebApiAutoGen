using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Collections.Generic;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Helpers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Extensions
{
    [TestClass]
    public class CSSRSHelperTests
    {

        [TestMethod]
        public void FilterPlaceholderQuestionsNoCSSRSTest()
        {
            var diaryEntryDto = new DiaryEntryDto()
            {
                Answers = new List<AnswerDto>()
                {
                    new AnswerDto
                    {
                       DisplayQuestion = "Question 1"
                    },
                    new AnswerDto
                    {
                       DisplayQuestion = "Question 2"
                    },
                    new AnswerDto
                    {
                       DisplayQuestion = "Question 3"
                    },
                }
            };

            var expectedAnnswers = diaryEntryDto.Answers.ToList();

            CSSRSHelper.FilterPlaceholderQuestions(diaryEntryDto);

            Assert.IsTrue(expectedAnnswers.Except(diaryEntryDto.Answers).Count() == 0 && diaryEntryDto.Answers.Except(expectedAnnswers).Count() == 0);
        }                

        [TestMethod]
        public void FilterPlaceholderQuestionsWithCSSRSTest()
        {
            var cssrsKey = "6MCSSRSSevereValue";
            var diaryEntryDto = new DiaryEntryDto()
            {
                Answers = new List<AnswerDto>()
                {
                    new AnswerDto
                    {
                       DisplayQuestion = "Question 1"
                    },
                    new AnswerDto
                    {
                       DisplayQuestion = cssrsKey
                    },
                    new AnswerDto
                    {
                       DisplayQuestion = "Question 3"
                    },
                }
            };

            var expectedAnnswers = diaryEntryDto.Answers.ToList();
            expectedAnnswers.RemoveAll(a => a.DisplayQuestion == cssrsKey);

            CSSRSHelper.FilterPlaceholderQuestions(diaryEntryDto);

            Assert.IsTrue(diaryEntryDto.Answers.Count == 2);
            Assert.IsTrue(expectedAnnswers.Except(diaryEntryDto.Answers).Count() == 0 && diaryEntryDto.Answers.Except(expectedAnnswers).Count() == 0);
        }
    }
}
