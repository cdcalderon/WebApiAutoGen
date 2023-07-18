using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace YPrime.Core.BusinessLayer.Extensions
{
    /// <summary>
    /// Extensions for HttpClient
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Set the HttpClient authorization header required for HMAC authentication
        /// </summary>
        /// <param name="clientId">Can be any convenient Id</param>
        /// <param name="body">Should be the final body of the request that will be used, as a string</param>
        /// <param name="sharedKey">Private key that is shared between client and server</param>
        public static void SetHmacAuthorizationHeader(this HttpClient client, string clientId, string body, string sharedKey)
        {
            string requestTimeStamp = DateTimeOffset.UtcNow.ToString();

            //clientId + utc timestamp + new guid
            string nonce = clientId + requestTimeStamp + Guid.NewGuid().ToString();

            var requestContentBase64String = GetRequestContentHashAsBase64(Encoding.UTF8.GetBytes(body));

            //Creating the raw signature string
            byte[] signature = GetRequestSignature(clientId, nonce, requestTimeStamp, requestContentBase64String);

            client.DefaultRequestHeaders.Authorization = GetRequestAuthorizationHeader(signature, clientId, nonce, requestTimeStamp, sharedKey);
        }

        private static string GetRequestContentHashAsBase64(byte[] bodyBytes)
        {
            MD5 md5 = MD5.Create();

            byte[] requestContentHash = md5.ComputeHash(bodyBytes);
            return Convert.ToBase64String(requestContentHash);
        }

        private static byte[] GetRequestSignature(string clientId, string nonce, string requestTimeStamp, string requestContentBase64String)
        {
            string signatureRawData = string.Format("{0}{1}{2}{3}", clientId, nonce, requestTimeStamp, requestContentBase64String);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);
            return signature;
        }

        private static AuthenticationHeaderValue GetRequestAuthorizationHeader(byte[] signature, string clientId, string nonce, string requestTimeStamp, string sharedKey)
        {
            byte[] secretKeyBytes = Convert.FromBase64String(sharedKey);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                return new AuthenticationHeaderValue("hmac", string.Format("{0};{1};{2};{3}", requestSignatureBase64String, clientId, nonce, requestTimeStamp));
            }
        }
    }
}
