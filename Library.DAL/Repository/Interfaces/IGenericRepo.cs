using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Library.DAL.Repository.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T,bool>>? filter = null);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
