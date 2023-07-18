using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal
{
    public interface IApplicationInitialization
    {
        Task Execute(HttpContext context);
    }

    public class CacheInitializer : IApplicationInitialization
    {
        private readonly IStudySettingService _studySettingService;

        public CacheInitializer(IStudySettingService studySettingService)
        {
            _studySettingService = studySettingService;
        }

        public async Task Execute(HttpContext context)
        {
            context.Cache.Insert("StudyID", Guid.Parse(ConfigurationManager.AppSettings["StudyID"]));
        }
    }
}