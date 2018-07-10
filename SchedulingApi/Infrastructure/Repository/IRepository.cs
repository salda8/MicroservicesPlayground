using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IdentityServer4.Quickstart.Repository
{
    public interface IRepository
    {
        void Add<T>(IEnumerable<T> items) where T : class, new();
        void Add<T>(T item) where T : class, new();
        IQueryable<T> All<T>() where T : class, new();
        bool CollectionExists<T>() where T : class, new();
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        T Single<T>(Expression<Func<T, bool>> expression) where T : class, new();
        IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new();
    }
}