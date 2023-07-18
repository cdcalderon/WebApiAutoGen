using System;

namespace YPrime.Data.Study.Proxies
{
    public interface IDbContextTransactionProxy : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
