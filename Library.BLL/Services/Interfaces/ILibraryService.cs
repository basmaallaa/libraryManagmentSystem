using Library.BLL.Common;
using Library.BLL.ModelVM.Book;
using Library.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Interfaces
{
    public interface ILibraryService
    {
        Task<Response<IEnumerable<BookVM>>> GetFilteredBooksAsync(
            string? searchTitle,
            string? searchAuthor,
            Category? category);
    }
}
