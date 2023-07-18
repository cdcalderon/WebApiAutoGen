using System.Data;
using System.Data.Entity;

namespace YPrime.Data.Study.Proxies
{
    public class DbContextTransactionProxy : IDbContextTransactionProxy
    {
        private readonly DbContextTransaction _transaction;

        public DbContextTransactionProxy(
            IStudyDbContext context)
        {
            _transaction = context.Database.BeginTransaction();
        }

        public DbContextTransactionProxy(
            IStudyDbContext context,
            IsolationLevel isolationLevel)
        {
            _transaction = context.Database.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
