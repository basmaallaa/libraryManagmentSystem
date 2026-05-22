using Library.BLL.ModelVM.Book;
using Library.BLL.ModelVM.Borrow;
using Library.BLL.ModelVM.Library;
using Library.BLL.Services.Interfaces;
using Library.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.PL.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IBorrowService _borrowService;

        public LibraryController(ILibraryService libraryService, IBorrowService borrowService)
        {
            _libraryService = libraryService;
            _borrowService = borrowService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? searchTitle,
            string? searchAuthor,
            Category? selectedCategory)
        {
            var result = await _libraryService.GetFilteredBooksAsync(searchTitle, searchAuthor, selectedCategory);

            var vm = new LibraryBooksVM
            {
                Books = result.IsSuccess ? result.Data : new List<BookVM>(),
                SearchTitle = searchTitle,
                SearchAuthor = searchAuthor,
                SelectedCategory = selectedCategory
            };
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Message;
            }
            return View(vm);
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> MyBorrowedBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var result = await _borrowService.GetUserBorrowedBooksAsync(userId);
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(new List<BorrowedBookVM>());
            }
            return View(result.Data);
        }
    }
}
