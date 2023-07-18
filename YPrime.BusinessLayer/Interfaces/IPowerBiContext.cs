using System;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IPowerBiContext : IKeyVaultBasedContext
    {
        string ApiUrl { get; }

        string ApplicationId { get; }

        string ApplicationSecret { get; }

        string AuthorityUrl { get; }

        string ResourceUrl { get; }

        string TenantId { get; }

        Guid? WorkspaceId { get; }

        string ExcludedTerms { get; }

        string ExcludedPrefix { get; }

        string[] ExcludedTermSet { get; }
    }
}
