using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EF.CustomMappingToolkit
{
    public class CustomMappingCompatibleQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable
    {
        readonly CustomMappingCompatibleQueryProvider<T> _provider;
        readonly IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery { get { return _inner; } }

        internal CustomMappingCompatibleQuery(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new CustomMappingCompatibleQueryProvider<T>(this);
        }

        Expression IQueryable.Expression { get { return _inner.Expression; } }

        Type IQueryable.ElementType { get { return typeof(T); } }

        IQueryProvider IQueryable.Provider { get { return _provider; } }

        /// <summary> IQueryable enumeration </summary>
        public IEnumerator<T> GetEnumerator() { return _inner.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _inner.GetEnumerator(); }

        /// <summary> IQueryable string presentation.  </summary>
        public override string ToString() { return _inner.ToString(); }
    }

    internal class CustomMappingCompatibleQueryOfClass<T> : CustomMappingCompatibleQuery<T>
        where T : class
    {
        public CustomMappingCompatibleQueryOfClass(IQueryable<T> inner) : base(inner)
        {
        }

        public IQueryable<T> Include(string path)
        {
            return InnerQuery.Include(path).AsCustomMappingCompatible();
        }
    }

    class CustomMappingCompatibleQueryProvider<T> : IQueryProvider, IDbAsyncQueryProvider
    {
        readonly CustomMappingCompatibleQuery<T> _query;

        internal CustomMappingCompatibleQueryProvider(CustomMappingCompatibleQuery<T> query)
        {
            _query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.
        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            var customMappingCompatible = expression.MakeCustomMappingCompatible();
            return _query.InnerQuery.Provider.CreateQuery<TElement>(customMappingCompatible).AsCustomMappingCompatible();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.MakeCustomMappingCompatible());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            var customMappingCompatible = expression.MakeCustomMappingCompatible();
            return _query.InnerQuery.Provider.Execute<TResult>(customMappingCompatible);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            var customMappingCompatible = expression.MakeCustomMappingCompatible();
            return _query.InnerQuery.Provider.Execute(customMappingCompatible);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            var asyncProvider = _query.InnerQuery.Provider as IDbAsyncQueryProvider;
            var customMappingCompatible = expression.MakeCustomMappingCompatible();
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync(customMappingCompatible, cancellationToken);
            return Task.FromResult(_query.InnerQuery.Provider.Execute(customMappingCompatible));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var asyncProvider = _query.InnerQuery.Provider as IDbAsyncQueryProvider;

            var customMappingCompatible = expression.MakeCustomMappingCompatible();
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync<TResult>(customMappingCompatible, cancellationToken);

            return Task.FromResult(_query.InnerQuery.Provider.Execute<TResult>(customMappingCompatible));
        }
    }
}
