using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using YPrime.Web.E2E.Data;

namespace YPrime.Web.E2E.Endpoints
{
    public class BaseEndpoint
    {
        protected readonly IHttpClientFactory HttpClientFactory;
        private readonly E2ESettings e2eSettings;
        private const string E2EHttpClientName = "e2eHttpClient";

        public BaseEndpoint(IHttpClientFactory httpClientFactory, E2ESettings e2ESettings)
        {
            this.e2eSettings = e2ESettings;
            HttpClientFactory = httpClientFactory;
        }

        public HttpClient httpClient()
        {
            return HttpClientFactory.CreateClient(E2EHttpClientName);
        }

    }
}
