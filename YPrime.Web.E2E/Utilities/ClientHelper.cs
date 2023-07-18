using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YPrime.Web.E2E.Data;

namespace YPrime.Web.E2E.Utilities
{
    public class ClientHelper
    {
        public static async Task<HttpClient> CreateAuthClient(IHttpClientFactory httpClientFactory, string httpClientName, E2ESettings e2eSettings)
        {
            var client = httpClientFactory.CreateClient(httpClientName);
            await SetAuthHeader(client, httpClientFactory, e2eSettings);
            return client;
        }

        public static async Task<string> GetAccessToken(IHttpClientFactory httpClientFactory, E2ESettings e2eSettings)
        {
            var client = httpClientFactory.CreateClient();
            string url = $"{e2eSettings.Auth0Url}";
            var body = new
            {
                client_id = $"{e2eSettings.Auth0ClientId}",
                client_secret = $"{e2eSettings.Auth0ClientSecret}",
                audience = $"{e2eSettings.Auth0Audience}",
                grant_type = "client_credentials"
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, request.Content);
            JObject jsonResult = response.Content.ReadAsAsync<JObject>().Result;
            string auth0MgtToken = jsonResult.Value<string>("access_token");

            return auth0MgtToken;
        }

        public static async Task SetAuthHeader(HttpClient client, IHttpClientFactory httpClientFactory, E2ESettings e2eSettings)
        {
            var accessToken = await GetAccessToken(httpClientFactory, e2eSettings);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }
    }
}
