using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Interfaces;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class DiaryPageRepository : BaseRepository, IDiaryPageRepository
    {
        IQuestionnaireService _questionnaireService;
        public DiaryPageRepository(
            IStudyDbContext db,
            IQuestionnaireService questionnaireService) 
            : base(db)
        {
            _questionnaireService = questionnaireService;
        }

        public async Task<List<DiaryPageModel>> GetQuestionnaireDiaryPages(Guid QuestionnaireId, Guid ConfigId)
        {
            return await GetQuestionnaireDiaryPages(QuestionnaireId, ConfigId, TranslationConstants.DefaultCultureCode);
        }

        public async Task<List<DiaryPageModel>> GetQuestionnaireDiaryPages(Guid QuestionnaireId, Guid ConfigId, string CultureCode)
        {
            var results = await _questionnaireService.GetInflatedQuestionnaire(QuestionnaireId, configurationId: ConfigId);

            return results.Pages;
        }
    }
}