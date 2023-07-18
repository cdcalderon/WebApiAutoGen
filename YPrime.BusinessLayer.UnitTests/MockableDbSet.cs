using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace YPrime.BusinessLayer.UnitTests
{
    public abstract class MockableDbSet<TEntity> : DbSet<TEntity>
        where TEntity : class
    {
        // add or update are not mockable without this wrapper class since they are extensions
        public abstract void AddOrUpdate(
            TEntity entity);

        public abstract void AddOrUpdate(
            params TEntity[] entities);

        public abstract void AddOrUpdate(
            Expression<Func<TEntity, object>> identifierExpression, 
            params TEntity[] entities);
    }
}
