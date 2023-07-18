using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IAuthenticationUserRepository : IBaseRepository
    {
        Task<IEnumerable<dynamic>> GetUsersAsync(IEnumerable<Guid> userIds);
    }
}