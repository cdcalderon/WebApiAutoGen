using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.PowerBi;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.Factories
{
    public class KeyVaultBasedContextFactory : IKeyVaultBasedContextFactory
    {
        private readonly IKeyVaultService _keyVaultService;
        private readonly IServiceSettings _serviceSettings;

        private readonly ConcurrentDictionary<Type, IKeyVaultBasedContext> contextMap = new ConcurrentDictionary<Type, IKeyVaultBasedContext>();

        private IReadOnlyCollection<Type> supportedContextTypes = new HashSet<Type>
        {
            typeof(PowerBiContext)
        };

        public KeyVaultBasedContextFactory(IKeyVaultService keyVaultService, IServiceSettings serviceSettings)
        {
            _keyVaultService = keyVaultService;
            _serviceSettings = serviceSettings;
        }

        public async Task<TContext> GetCurrentContext<TContext>()
            where TContext : class, IKeyVaultBasedContext
        {
            if (!contextMap.TryGetValue(typeof(TContext), out var context))
            {
                context = await TryCreateContext<TContext>();
            }

            return (TContext)context;
        }

        private static void ValidateContext(IKeyVaultBasedContext context)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(context);

            Validator.TryValidateObject(context, validationContext, validationResults);
            validationResults.AddRange(context.Validate(validationContext));

            if (validationResults.Any())
            {
                throw new ValidationException($"The context of type {context.GetType()} was invalid and could not be used.", null, validationResults);
            }
        }

        private async Task<TContext> TryCreateContext<TContext>()
                    where TContext : class, IKeyVaultBasedContext
        {
            TContext context = null;

            if (supportedContextTypes.Contains(typeof(TContext)))
            {
                var studyId = _serviceSettings.StudyId;

                var keyVaultKey = $"{typeof(TContext).Name}-{studyId}";
                var keyVaultReference = await _keyVaultService.GetSecretValueFromKey(keyVaultKey);

                if (!string.IsNullOrEmpty(keyVaultReference))
                {
                    context = JsonConvert.DeserializeObject<TContext>(keyVaultReference);
                    ValidateContext(context);

                    contextMap.TryAdd(typeof(TContext), context);
                }
            }

            return context;
        }
    }
}