using Library.BLL.ModelVM.Borrow;
using Library.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.PL.Controllers
{
    [Authorize(Roles = "User")]
    public class BorrowController : Controller
    {
        private readonly IBorrowService _borrowService;

        public BorrowController(IBorrowService borrowService)
        {
            _borrowService = borrowService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(BorrowVM vm, string? returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var result = await _borrowService.BorrowBookAsync(vm, userId);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Library");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReturn(int BorrowId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            var result = await _borrowService.RequestReturnAsync(BorrowId,userId);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction("MyBorrowedBooks", "Library");
        }
        // ── GET /Borrow/Index (kept for admin use if needed) ───
        //[Authorize(Roles = "Admin")]
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
