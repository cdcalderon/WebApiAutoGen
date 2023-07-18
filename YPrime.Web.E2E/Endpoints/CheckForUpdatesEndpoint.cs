using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using YPrime.Web.E2E.Data;
using YPrime.Web.E2E.Models.Api;

namespace YPrime.Web.E2E.Endpoints
{
    [Binding]
    public class CheckForUpdatesEndpoint : BaseEndpoint
    {
        private const string endpoint = "CheckForUpdates";
        private readonly E2ESettings e2eSettings;

        public CheckForUpdatesEndpoint(IHttpClientFactory httpClientFactory, E2ESettings e2eSettings) : base(httpClientFactory, e2eSettings)
        {
            this.e2eSettings = e2eSettings;
        }

        public async Task<CheckForUpdatesResponse> CheckForUpdates(string request)
        {
            var client = httpClient();

            var body = JsonConvert.SerializeObject(request);
            var data = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{e2eSettings.ApiUrl}{endpoint}", data).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<CheckForUpdatesResponse>(json);

            return result;
        }
    }
}
