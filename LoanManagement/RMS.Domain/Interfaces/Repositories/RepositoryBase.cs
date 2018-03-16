using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RMS.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T obj);
        void Update(T obj);
        void Remove(T obj);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    }
}
