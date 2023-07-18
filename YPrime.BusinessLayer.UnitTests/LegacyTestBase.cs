using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests
{
    public abstract class LegacyTestBase
    {
        public string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public Mock<MockableDbSetWithExtensions<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<MockableDbSetWithExtensions<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            dbSetMock.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(elements.GetEnumerator()));

            dbSetMock.Setup(m => m.AddOrUpdate(It.IsAny<T[]>()));
            dbSetMock.Setup(m => m.AddOrUpdate(It.IsAny<Expression<Func<T, object>>>(), It.IsAny<T[]>()));

            return dbSetMock;
        }
    }

    public abstract class MockableDbSetWithExtensions<T> : DbSet<T> where T : class
    {
        public abstract void AddOrUpdate(params T[] entities);
        public abstract void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities);
    }

    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {

        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
        public IQueryable CreateQuery(Expression expression) { return new TestDbAsyncEnumerable<TEntity>(expression); }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) { return new TestDbAsyncEnumerable<TElement>(expression); }
        public object Execute(Expression expression) { return _inner.Execute(expression); }
        public TResult Execute<TResult>(Expression expression) { return _inner.Execute<TResult>(expression); }
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) { return Task.FromResult(Execute(expression)); }
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) { return Task.FromResult(Execute<TResult>(expression)); }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestDbAsyncEnumerable(Expression expression) : base(expression) { }
        public IDbAsyncEnumerator<T> GetAsyncEnumerator() { return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator()); }
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() { return GetAsyncEnumerator(); }
        public IQueryProvider Provider { get { return new TestDbAsyncQueryProvider<T>(this); } }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
        public void Dispose() { _inner.Dispose(); }
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) { return Task.FromResult(_inner.MoveNext()); }
        public T Current { get { return _inner.Current; } }
        object IDbAsyncEnumerator.Current { get { return Current; } }
    }
}