using Library.DAL.Context;
using Library.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DAL.Repository.Implementations
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
