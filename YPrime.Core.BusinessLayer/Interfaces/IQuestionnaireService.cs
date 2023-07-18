using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IQuestionnaireService : IConfigServiceBase<QuestionnaireModel, Guid>
    {
        Task<List<QuestionModel>> GetQuestions(Guid questionnaireId, Guid? configurationId = null);

        Task<List<QuestionnaireModel>> GetAllWithPages(Guid? configurationId = null);

        Task<QuestionnaireModel> GetInflatedQuestionnaire(Guid questionnaireId, Guid? languageId = null, Guid? configurationId = null);
        Task<List<QuestionnaireModel>> GetAllInflatedQuestionnaires(Guid? languageId = null, Guid? configurationId = null);
    }
}
