using Serilog;
using System;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly IKeyVault _keyVault;
        protected readonly ILogger _logger;

        public KeyVaultService(IKeyVault keyVault)
        {
            _keyVault = keyVault;
            _logger = ExceptionLogger.CreateLogger();
        }

        public async Task<string> GetSecretValueFromKey(string key)
        {
            try
            {
                var secretValue = await _keyVault.Client.GetSecretAsync(key);
                return secretValue?.Value?.Value;
            }
            catch (Exception ex)
            {
                _logger.ForContext<KeyVaultService>().Error(ex.Message);

                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return string.Empty;
        }
    }
}
