using Library.BLL.Common;
using Library.BLL.ModelVM.Borrow;
using Library.BLL.Services.Interfaces;
using Library.DAL.Models;
using Library.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Implementations
{
    public class BorrowService : IBorrowService
    {
        private readonly IGenericRepo<Book> _bookRepo;
        private readonly IGenericRepo<BorrowRecord> _borrowRecordRepo;
        private readonly IGenericRepo<ApplicationUser> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public BorrowService(IGenericRepo<Book> bookRepo, IGenericRepo<BorrowRecord> borrowRecordRepo, IGenericRepo<ApplicationUser> userRepo, IUnitOfWork unitOfWork)
        {
            _bookRepo = bookRepo;
            _borrowRecordRepo = borrowRecordRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> BorrowBookAsync(BorrowVM VM, string userId)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return new Response<string>(
                        null,
                        "User not found or inactive.",
                        false
                        );
                }
                var book = await _bookRepo.GetByIdAsync(VM.BookId);
                if (book == null)
                {
                    return new Response<string>(
                        null,
                        "Book not found.",
                        false
                        );
                }
                if (book.Quantity <= 0)
                {
                    return new Response<string>(
                        null,
                        "Book is currently unavailable.",
                        false
                        );
                }
                var aleadyBorrowed = await _borrowRecordRepo
                    .GetAll(b =>
                    b.Id == VM.BookId &&
                    b.UserId == userId &&
                    !b.IsReturned)
                    .AnyAsync();
                if (aleadyBorrowed)
                {
                    return new Response<string>
                    (
                        null,
                         "You already borrowed this book",
                         false
                    );
                }
                var borrow = new BorrowRecord
                {
                    UserId = userId,
                    BookId = VM.BookId,
                    BorrowDate = DateTime.Now,
                    IsReturned = false
                };
                await _borrowRecordRepo.AddAsync(borrow);
                book.Quantity--;
                _bookRepo.Update(book);
                await _unitOfWork.SaveChangesAsync();
                return new Response<string>
                (
                    null,
                    "Book borrowed successfully.",
                    true
                );

            }
            catch (Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred: {ex.Message}",
                    false
                );
            }
        }


        public async Task<Response<string>> RequestReturnAsync(int borrowId, string userId)
        {
            try
            {
                var borrow = await _borrowRecordRepo
                    .GetAll(b => b.Id == borrowId && b.UserId == userId)
                    .FirstOrDefaultAsync();

                if (borrow == null)
                    return new Response<string>(null,"Borrow record not found.", false);

                if (borrow.IsReturned)
                    return new Response<string>(null,"This book has already been returned.", false);

                if (borrow.ReturnRequested)
                    return new Response<string>(null,"A return request is already pending admin approval.", false);

                borrow.ReturnRequested = true;
                _borrowRecordRepo.Update(borrow);
                await _unitOfWork.SaveChangesAsync();

                return new Response<string>(null,"Return request submitted. The admin will confirm your return shortly.", true);
            }
            catch (Exception ex) { return new Response<string>(null,$"An error occurred: {ex.Message}", false); }
        }

        public async Task<Response<IEnumerable<BorrowedBookVM>>> GetUserBorrowedBooksAsync(string userId)
        {
            try
            {
                var borrowedBooks = await _borrowRecordRepo
                    .GetAll(b => b.UserId == userId )
                    .Include(b => b.Book)
                    .OrderByDescending(b => b.BorrowDate)
                    .Select(b => new BorrowedBookVM
                    {
                        BorrowId = b.Id,
                        BookId = b.BookId,
                        Title = b.Book.Title,
                        Author = b.Book.Author,
                        Category = b.Book.Category,
                        ImagePath = b.Book.ImagePath,
                        BorrowDate = b.BorrowDate,
                        ReturnDate = b.ReturnDate,
                        IsReturned = b.IsReturned
                    })
                    .ToListAsync();
                return new Response<IEnumerable<BorrowedBookVM>>(
                    borrowedBooks,
                    "Borrowed books retrieved successfully.",
                    true
                    );
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<BorrowedBookVM>>(
                    null,
                    $"An error occurred: {ex.Message}",
                    false
                    );
            }
        }

        public async Task<bool> IsBookBorrowedByUserAsync(int bookId, string userId)
        {
            try
            {
                return await _borrowRecordRepo
                    .GetAll(b => b.BookId == bookId && b.UserId == userId && !b.IsReturned)
                    .AnyAsync();

            }
            catch (Exception)
            {
                // Log the exception as needed
                return false; // Assume not borrowed if there's an error
            }
        }

    }
}

