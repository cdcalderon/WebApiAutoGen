using System;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.ViewModel;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IAlarmRepository
    {
        Task<ApiRequestResultViewModel> ExecuteAlarmBusinessRule(AlarmRuleDto alarmDto);
    }
}
