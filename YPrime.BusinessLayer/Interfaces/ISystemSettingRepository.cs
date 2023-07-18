using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISystemSettingRepository
    {
        string GetSystemSettingValue(string name);
        bool GetSystemSettingValueAsBool(string name);
        int AddSystemSetting(SystemSetting systemSetting);
    }
}
