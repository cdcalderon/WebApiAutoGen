using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IStudySettingService : IConfigServiceBase<StudySettingModel, string>
    {
        Task<string> GetStringValue(string key, Guid? configurationId = null);

        Task<int> GetIntValue(string key, int defaultValue = 0, Guid? configurationId = null);

        Task<Guid> GetGuidValue(string key, Guid? configurationId = null);

        Task<bool> GetBoolValue(string key, bool defaultValue = false, Guid? configurationId = null);

        Task<List<StudyCustomModel>> GetAllStudyCustoms(Guid? configurationId = null);

        Task LoadIntoCache(Guid configurationId);
    }
}
