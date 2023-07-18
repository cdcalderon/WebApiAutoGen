using System;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IAlarmService: IConfigServiceBase<AlarmModel, Guid>
    {
        Task<AlarmModel> GetTranslatedAlarmModel(
            Guid alarmId,
            Guid? languageId = null,
            Guid? configurationId = null);
    }
}
