using Azure.Security.KeyVault.Secrets;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IKeyVault
    {
        SecretClient Client { get; set; }
    }
}