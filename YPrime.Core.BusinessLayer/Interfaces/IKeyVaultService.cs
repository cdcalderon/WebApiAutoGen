using System.Threading.Tasks;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretValueFromKey(string key);
    }
}
