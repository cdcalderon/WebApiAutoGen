using Azure.Security.KeyVault.Secrets;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Models
{
    public class KeyVaultClient : IKeyVault
    {
        public SecretClient Client { get; set; }
    }
}
