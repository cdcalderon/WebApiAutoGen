using Elmah;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.Repositories
{
    public class TimeZoneRepository : ITimeZoneRepository
    {
        private readonly string _timeZonefieldName;
        private readonly string _timeZoneAddress;
        private readonly string _timeZoneServiceUrl;
        private readonly IApiRepository _apiRepository;

        public TimeZoneRepository(IApiRepository apiRepository)
        {
            _timeZonefieldName = (ConfigurationManager.AppSettings["YPrimeTimezoneFieldName"] ?? "timezone").ToString();
            _timeZoneAddress = (ConfigurationManager.AppSettings["YPrimeTimezoneAddress"] ?? "~TZ~/json").ToString();
            _timeZoneServiceUrl = (ConfigurationManager.AppSettings["YPrimeIPTimezoneURL"] ?? string.Empty).ToString();
            _apiRepository = apiRepository;
        }
        public async Task<string> GetTimeZoneId(string ipAddress)
        {
            return await GetTimeZoneIdWithDefault(ipAddress, string.Empty);
        }

        public async Task<string> GetTimeZoneIdWithDefault(string ipAddress, string defaultTimeZone)
        {
            var addressParams = _timeZoneAddress.Replace("~TZ~", ipAddress);
            string timeZone;

            try
            {
                var result = await _apiRepository.Get<dynamic>(_timeZoneServiceUrl, addressParams);
                timeZone = result[_timeZonefieldName] ?? defaultTimeZone;
            }
            catch (Exception e)
            {
                SignalFromCurrentContext(e);
                timeZone = defaultTimeZone;
            }

            return timeZone;
        }

        public virtual void SignalFromCurrentContext(Exception e)
        {
            if (HttpContext.Current != null)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
    }
}
