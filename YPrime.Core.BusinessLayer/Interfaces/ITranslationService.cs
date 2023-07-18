using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface ITranslationService : IConfigServiceBase<TranslationModel, string>
    {
        Task<string> GetByKey(string id, Guid? configurationId = null, Guid? languageId = null);

        Task LoadIntoCache(string source, Guid configurationId, Guid languageId);

        Task<List<TranslationModel>> GetByLanguage(Guid languageId, Guid? configurationId = null);
    }
}
