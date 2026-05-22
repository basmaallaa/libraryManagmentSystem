using Library.BLL.ModelVM.Admin;
using Library.BLL.ModelVM.Borrow;
using Library.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── GET /Admin/Dashboard ───────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View(new Library.BLL.ModelVM.Admin.DashboardVM());
            }
            return View(result.Data);
        }

        // ── GET /Admin/Borrowings ──────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Borrowings(
            string? searchUser, string? searchBook, bool? filterReturned)
        {
            var result = await _adminService.GetBorrowRecordsAsync(searchUser, searchBook, filterReturned);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View(new BorrowRecordListVM()); 
            }
            var pendingResult = await _adminService.GetPendingReturnsAsync();

            ViewBag.PendingReturns = pendingResult.IsSuccess
                ? pendingResult.Data
                : Enumerable.Empty<PendingReturnVM>();
            return View(result.Data);
        }

        // ── POST /Admin/MarkReturned ───────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int borrowId)
        {
            var result = await _adminService.MarkAsReturnedAsync(borrowId);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Borrowings");
        }

        // ── GET /Admin/Users ───────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Users(string? searchQuery)
        {
            var result = await _adminService.GetUsersAsync(searchQuery);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View(new Library.BLL.ModelVM.Admin.UserListVM());
            }
            return View(result.Data);
        }

        // ── POST /Admin/ToggleActive ───────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string userId)
        {
            var result = await _adminService.ToggleUserActiveAsync(userId);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Users");
        }

        // ── GET /Admin/UserHistory/{id} ────────────────────────
        [HttpGet]
        public async Task<IActionResult> UserHistory(string id)
        {
            var result = await _adminService.GetUserBorrowHistoryAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        // ── AJAX partial for user history modal ───────────────
        [HttpGet]
        public async Task<IActionResult> UserHistoryPartial(string id)
        {
            var result = await _adminService.GetUserBorrowHistoryAsync(id);
            if (!result.IsSuccess) return NotFound();
            return PartialView("_UserHistoryPartial", result.Data);
        }
    }
}
