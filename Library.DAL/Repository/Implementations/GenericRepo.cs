using Library.DAL.Context;
using Library.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Library.DAL.Repository.Implementations
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
             await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
             _dbSet.Remove(entity);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            var query = _dbSet.AsQueryable();
            try
            {
                if(filter != null)
                {
                    query = query.Where(filter);
                    return query;
                }
                else
                {
                    return query;
                }
            }
            catch(Exception ex) 
            {
                throw ;
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
