using Library.BLL.Common;
using Library.BLL.ModelVM.Book;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Interfaces
{
    public interface IBookService
    {
        Task<Response<string>> AddBookAsync(AddBookVM vm);
        Task<Response<IEnumerable<BookVM>>> GetAllBooksAsync();
        Task<Response<BookVM>> GetBookByIdAsync(int id);
        Task<Response<string>> UpdateBookAsync(UpdateBookVM vm);
        Task<Response<string>> DeleteBookAsync(int id);
    }
}
