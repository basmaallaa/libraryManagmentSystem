using Library.BLL.Common;
using Library.BLL.ModelVM.Borrow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Interfaces
{
    public interface IBorrowService
    {
        Task<Response<string>> BorrowBookAsync (BorrowVM VM , string userId);
        Task<Response<string>> RequestReturnAsync(int borrowId, string userId);

        /// <summary>Get all borrow records for a specific user.</summary>
        Task<Response<IEnumerable<BorrowedBookVM>>> GetUserBorrowedBooksAsync(string userId);

        /// <summary>Check whether a specific user currently has a book borrowed (not returned).</summary>
        Task<bool> IsBookBorrowedByUserAsync(int bookId, string userId);
    }
}
