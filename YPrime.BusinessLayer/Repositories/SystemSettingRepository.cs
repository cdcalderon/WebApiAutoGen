using System;
using System.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Data.Entity;
using YPrime.Core.BusinessLayer.Models;
using System.Collections.Generic;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class SystemSettingRepository : BaseRepository, ISystemSettingRepository
    {
        public SystemSettingRepository(IStudyDbContext db) : base(db)
        {

        }

        public int AddSystemSetting(SystemSetting systemSetting)
        {
            var entity = _db.SystemSettings.SingleOrDefault(d => d.Name == systemSetting.Name);

            if (entity == null)
            {
                entity = _db.SystemSettings.Add(systemSetting);
                _db.SaveChanges(null);
            }

            return entity.Id;
        }

        public string GetSystemSettingValue(string name)
        {
            return _db.SystemSettings.FirstOrDefault(s => s.Name == name)?.Value;
        }

        public bool GetSystemSettingValueAsBool(string name)
        {
            var value = GetSystemSettingValue(name);
            bool ret;
            return bool.TryParse(value, out ret) ? ret : false;
        }
    }
}
