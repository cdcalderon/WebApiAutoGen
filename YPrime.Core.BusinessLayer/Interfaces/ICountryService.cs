using System;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface ICountryService : IConfigServiceBase<CountryModel, Guid>
    { }
}
