using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace YPrime.BusinessLayer.UnitTests
{
    // This class encapsulates all of the examples provided within the following Microsoft blog, including allowing for async querying.
    // https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking#testing-with-async-queries
    public class FakeDbSet<TEntity> : Mock<MockableDbSet<TEntity>> where TEntity : class
    {
        public FakeDbSet(IEnumerable<TEntity> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            SetupData(data.AsQueryable());
        }

        private void SetupData(IQueryable<TEntity> data)
        {
            As<IDbAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<TEntity>(data.GetEnumerator()));
            As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider(data.Provider));
            As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => data.GetEnumerator());

            Setup(m => m.Include(It.IsAny<string>()))
                .Returns(Object);
        }

        private class TestDbAsyncQueryProvider : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestDbAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        private class TestDbAsyncEnumerable<TElement> : EnumerableQuery<TElement>, IDbAsyncEnumerable<TElement>,
            IQueryable<TElement>
        {
            public TestDbAsyncEnumerable(IEnumerable<TElement> enumerable) : base(enumerable)
            {
            }

            public TestDbAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IDbAsyncEnumerator<TElement> GetAsyncEnumerator()
            {
                return new TestDbAsyncEnumerator<TElement>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider(this);
        }

        private class TestDbAsyncEnumerator<TElement> : IDbAsyncEnumerator<TElement>
        {
            private readonly IEnumerator<TElement> _inner;

            public TestDbAsyncEnumerator(IEnumerator<TElement> inner)
            {
                _inner = inner;
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_inner.MoveNext());
            }

            public TElement Current => _inner.Current;
            object IDbAsyncEnumerator.Current => Current;
        }
    }
}