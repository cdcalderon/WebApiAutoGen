using System;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IConfigurationVersionService : IConfigServiceBase<ConfigurationVersion, Guid>
    {
        Task<Guid> GetLatest();
    }
}
