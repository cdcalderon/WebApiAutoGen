using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class QuestionnaireModelExtensions
    {
        private static readonly List<int> QuestionnaireTypesWithoutVisitRequirement = new List<int>
        {
            QuestionnaireType.PatientHandheld.Id,
            QuestionnaireType.ObserverHandheld.Id,
            QuestionnaireType.PatientAndObserverHandheld.Id,
            QuestionnaireType.PatientHandheldTraining.Id,
            QuestionnaireType.ObserverHandheldTraining.Id,
            QuestionnaireType.PatientTabletTraining.Id,
            QuestionnaireType.ObserverTabletTraining.Id
        };

        private static readonly List<int> QuestionTypesNotDisplayableOnDcf = new List<int>
        {
            InputFieldType.Camera.Id,
            InputFieldType.None.Id
        };

        public static bool TypeRequiresVisit(this QuestionnaireModel model)
        {
            var typeInList = model == null || QuestionnaireTypesWithoutVisitRequirement
                .Any(r => r == model.QuestionnaireTypeId);

            return !typeInList;
        }

        public static List<QuestionModel> GetSortedquestions(this QuestionnaireModel model)
        {
            var questions = new List<QuestionModel>();

            if (model.Pages != null && model.Pages.Any())
            {
                questions = model
                    .Pages
                    .OrderBy(p => p.Number)
                    .SelectMany(p => p.Questions.OrderBy(q => q.Sequence))
                    .ToList();
            }

            return questions;
        }

        public static void RemoveNonDcfQuestions(this QuestionnaireModel model)
        {
            if (model == null || model.Pages == null || !model.Pages.Any())
            {
                return;
            }

            foreach (var page in model.Pages)
            {
                page.Questions.RemoveAll(q => QuestionTypesNotDisplayableOnDcf.Contains(q.QuestionType));
            }
        }
    }
}
