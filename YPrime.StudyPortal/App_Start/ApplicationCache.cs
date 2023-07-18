using System;
using System.Web;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal
{
    public interface IApplicationCache
    {
        object Get(string key);
        void Set(string key, object value);
    }

    public class ApplicationCache : IApplicationCache
    {
        private readonly IStudySettingService _studySettingService;

        public ApplicationCache(IStudySettingService studySettingService)
        {
            _studySettingService = studySettingService;
        }

        public object Get(string key)
        {
            object item = null;

            try
            {
                item = HttpContext.Current.Cache.Get(key);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return item ?? _studySettingService.GetStringValue(key);
        }

        public void Set(string key, object value)
        {
            HttpContext.Current.Cache.Insert(key, value);
        }
    }
}