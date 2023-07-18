using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IConfigServiceBase<T, TID>
    {
        Task<List<T>> GetAll(Guid? configurationId = null);

        Task<T> Get(TID id, Guid? configurationId = null);

        Task<T> Get(Expression<Func<T, bool>> predicate, TID id, Guid? configurationId = null);
    }
}
