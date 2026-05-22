using AutoMapper;
using Library.BLL.Common;
using Library.BLL.Helper;
using Library.BLL.ModelVM.Book;
using Library.BLL.Services.Interfaces;
using Library.DAL.Models;
using Library.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Library.BLL.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IGenericRepo<Book> _bookRepo;
        private readonly IGenericRepo<BorrowRecord> _borrowRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IGenericRepo<Book> bookRepo, IGenericRepo<BorrowRecord> borrowRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookRepo = bookRepo;
            _borrowRepo = borrowRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<string>> AddBookAsync(AddBookVM vm)
        {
            try
            {
                if (vm.Quantity < 0)
                {
                    return new Response<string>
                    (
                        null,
                        "Quantity cannot be negative.",
                        false
                    );
                }
                    var book = _mapper.Map<Book>(vm);    

                    if(vm.Image != null)
                    {
                        book.ImagePath = await FileHelper.UploadFileAsync(vm.Image, "files");
                    }

                    await _bookRepo.AddAsync(book);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<string>
                    (
                        null,
                        "Book added successfully.",
                        true
                    );
                

            }
            catch(Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred while adding the book: {ex.Message}",
                    false
                );
            }
        }

        public async Task<Response<string>> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _bookRepo.GetByIdAsync(id);
                if(book == null)
                {
                    return new Response<string>
                    (
                        null,
                        "Book not found.",
                        false
                    );
                }
                var isBorrowed = await _borrowRepo.GetAll(b =>b.BookId ==id && !b.IsReturned).AnyAsync();
                if(isBorrowed) 
                {
                    return new Response<string>
                    (
                        null,
                        "Cannot delete the book as it is currently borrowed.",
                        false
                    );
                }
                _bookRepo.Delete(book);
                await _unitOfWork.SaveChangesAsync();
                return new Response<string>
                (
                    null,
                    "Book deleted successfully.",
                    true
                );

            }
            catch(Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred while deleting the book: {ex.Message}",
                    false
                );
            }
        }

        public async Task<Response<IEnumerable<BookVM>>> GetAllBooksAsync()
        {
            try
            {
                var books =await _bookRepo.GetAll().ToListAsync();
                var mappedBooks = _mapper.Map<IEnumerable<BookVM>>(books);
                return new Response<IEnumerable<BookVM>>
                (
                    mappedBooks,
                    "Books retrieved successfully.",
                    true
                );
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<BookVM>>
                (
                    null,
                    $"An error occurred while retrieving books: {ex.Message}",
                    false
                );
            }
        }

        public async Task<Response<BookVM>> GetBookByIdAsync(int id)
        {
            try
            {
                var book =await _bookRepo.GetByIdAsync(id);
                if (book == null)
                {
                    return new Response<BookVM>
                    (
                        null,
                        "Book not found.",
                        false
                    );
                }
                var mappedBook = _mapper.Map<BookVM>(book);
                return new Response<BookVM>
                (
                    mappedBook,
                    "Book retrieved successfully.",
                    true
                );
            }
            catch (Exception ex)
            {
                return new Response<BookVM>
                (
                    null,
                    $"An error occurred while retrieving the book: {ex.Message}",
                    false
                );
            }
        }

        public async Task<Response<string>> UpdateBookAsync(UpdateBookVM vm)
        {
            try
            {
                var book = await _bookRepo.GetByIdAsync(vm.Id);
                if (book == null)
                {
                    return new Response<string>
                    (
                        null,
                        "Book not found.",
                        false
                    );
                }

                book.Title = vm.Title;
                book.Author = vm.Author;
                book.Category = vm.Category;
                book.Description = vm.Description;
                book.Quantity = vm.Quantity;

                if (vm.Image != null)
                {
                    book.ImagePath = await FileHelper.UploadFileAsync(vm.Image, "files");
                }

                _bookRepo.Update(book);
                await _unitOfWork.SaveChangesAsync();
                return new Response<string>
                (
                    null,
                    "Book updated successfully.",
                    true
                );
            }
            catch (Exception ex)
            {
                return new Response<string>
                (
                    null,
                    $"An error occurred while updating the book: {ex.Message}",
                    false
                );
            }
        }
    }
}
