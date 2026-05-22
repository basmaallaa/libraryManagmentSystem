using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DAL.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
