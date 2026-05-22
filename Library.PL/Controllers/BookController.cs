using Library.BLL.ModelVM.Book;
using Library.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _bookService.GetAllBooksAsync();
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
            }
            return View(result.Data);
        }

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            return PartialView("_DetailsBookPartial", result.Data);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddBookVM vm)
        {
            if (!ModelState.IsValid)
            {
                // AJAX request → return partial with validation errors
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    Request.ContentType?.Contains("multipart/form-data") == true)
                {
                    return PartialView("_AddBookPartial", vm);
                }
                return View(vm);
            }

            var result = await _bookService.AddBookAsync(vm);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    Request.ContentType?.Contains("multipart/form-data") == true)
                {
                    return PartialView("_AddBookPartial", vm);
                }
                return View(vm);
            }
            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }
            var vm = new UpdateBookVM
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                Author = result.Data.Author,
                Category = result.Data.Category,
                Description = result.Data.Description,
                Quantity = result.Data.Quantity,
                ExistingImage = result.Data.ImagePath
            };
            return PartialView("_EditBookPartial", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPartial(UpdateBookVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditBookPartial", vm);
            }

            var result = await _bookService.UpdateBookAsync(vm);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return PartialView("_EditBookPartial", vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result.IsSuccess) 
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
