using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.Repositories
{
    public class ApiRepository : BaseRepository, IApiRepository
    {
        private const string BasicAuthenticationScheme = "Basic";

        public ApiRepository(IStudyDbContext db) : base(db)
        { }

        public async Task<T> Get<T>(string baseApiUrl, string address, string basicAuthorizationHeader = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(basicAuthorizationHeader))
                {
                    var byteArray = Encoding.ASCII.GetBytes(basicAuthorizationHeader);
                    var base64Value = Convert.ToBase64String(byteArray);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthenticationScheme, base64Value);
                }

                HttpResponseMessage response = await client.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserializedValues = (T)JsonConvert.DeserializeObject<T>(json);
                    return deserializedValues;
                }

                var textReponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Api call failure : {baseApiUrl}{address} - {textReponse}");
            }
        }

        public string Post(string baseApiUrl, string address, object postData)
        {
            var Rtn = String.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(address, content).Result;

                Rtn = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new System.Web.HttpException($"Api call failure : {baseApiUrl}{address} - {Rtn}"));
                }

            }

            return Rtn;
        }

        public async Task<T> Post<T>(string baseApiUrl, string address, object postData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(address, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deserializedValues = JsonConvert.DeserializeObject<T>(json);
                    return deserializedValues;
                }

                var textReponse = await response.Content.ReadAsStringAsync();

                throw new Exception($"Api call failure : {baseApiUrl}{address} - {textReponse}");
            }
        }
    }
}