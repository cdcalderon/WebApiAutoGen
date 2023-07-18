using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IApiRepository : IBaseRepository
    {
        Task<T> Get<T>(string baseApiUrl, string address, string basicAuthorizationHeader = null);
        Task<T> Post<T>(string baseApiUrl, string address, object postData);
        string Post(string baseApiUrl, string address, object postData);
    }
}
