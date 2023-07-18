using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IDiaryPageRepository
    {
        Task<List<DiaryPageModel>> GetQuestionnaireDiaryPages(Guid QuestionnaireId, Guid ConfigId);

        Task<List<DiaryPageModel>> GetQuestionnaireDiaryPages(Guid QuestionnaireId, Guid ConfigId, string CultureCode);
    }
}