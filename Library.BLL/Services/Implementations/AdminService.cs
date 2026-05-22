using AutoMapper;
using Library.BLL.Common;
using Library.BLL.ModelVM.Admin;
using Library.BLL.ModelVM.Borrow;
using Library.BLL.Services.Interfaces;
using Library.DAL.Models;
using Library.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepo<Book> _bookRepo;
        private readonly IGenericRepo<BorrowRecord> _borrowRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private const int LATE_DAYS = 1;

        public AdminService(IGenericRepo<Book> bookRepo, IGenericRepo<BorrowRecord> borrowRepo, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _bookRepo = bookRepo;
            _borrowRepo = borrowRepo;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Response<DashboardVM>> GetDashboardAsync()
        {
            try
            {
                var allBooks = await _bookRepo.GetAll().ToListAsync();
                var allBorrows = await _borrowRepo.GetAll().Include(b => b.Book).ToListAsync();
                var allUsers =  _userManager.Users.ToList();

                var now = DateTime.UtcNow;
                var monthly = Enumerable.Range(0, 6)
                    .Select(i => now.AddMonths(-5 + i))
                    .Select(m => new MonthlyBorrowStatVM
                    {
                        Month = m.ToString("MMM"),
                        Borrowed = allBorrows.Count(b => b.BorrowDate.Year == m.Year && b.BorrowDate.Month == m.Month),
                        Returned = allBorrows.Count(b => b.IsReturned && b.ReturnDate.HasValue
                        && b.ReturnDate.Value.Year == m.Year && b.ReturnDate.Value.Month == m.Month)
                    });

                var categoryStats = allBooks
                    .GroupBy(b => b.Category.ToString())
                    .Select(g => new CategoryStatVM
                    {
                        Category = g.Key,
                        Count = g.Count()
                    });

                var vm = new DashboardVM
                {
                    TotalBooks = allBooks.Count,
                    AvailableBooks = allBooks.Sum(b =>b.Quantity),
                    BorrowedBooks = allBorrows.Count(),
                    TotalUsers = allUsers.Count(),
                    ActiveBorrowings = allBorrows.Count(b => !b.IsReturned),
                    ReturnedBooks = allBorrows.Count(b => b.IsReturned),

                    RecentBooks = allBooks
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(6)
                    .Select(b => _mapper.Map<RecentBookVM>(b)),

                    RecentBorrowings = allBorrows
                    .OrderByDescending(b => b.BorrowDate)
                    .Take(8)
                    .Select(b => _mapper.Map<RecentBorrowingVM>(b)),

                    MonthlyStats = monthly,
                    CategoryStats = categoryStats
                };

                return new Response<DashboardVM>(vm , "Ok", true);

            }
            catch (Exception ex)
            {
                return new Response<DashboardVM>(null, ex.Message, false);
            }
        }
        public async Task<Response<BorrowRecordListVM>> GetBorrowRecordsAsync(string? searchUser, string? searchBook, bool? filterReturned)
        {
            try
            {
                var query = _borrowRepo.GetAll()
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchUser))
                {
                    var s = searchUser.Trim().ToLower();
                    query = query.Where(b =>
                        (b.User != null && b.User.FullName.ToLower().Contains(s)) ||
                        (b.User != null && b.User.Email != null && b.User.Email.ToLower().Contains(s)));
                }
                if (!string.IsNullOrWhiteSpace(searchBook))
                {
                    var s = searchBook.Trim().ToLower();
                    query = query.Where(b => b.Book != null && b.Book.Title.ToLower().Contains(s));
                }

                if (filterReturned.HasValue)
                    query = query.Where(b => b.IsReturned == filterReturned.Value);

                var records = await query.OrderByDescending(b =>b .BorrowDate)
                    .ToListAsync();

                var now = DateTime.UtcNow;

                var vm = new BorrowRecordListVM
                {
                    Records = records.Select(b =>
                    {
                        var vm = _mapper.Map<BorrowRecordRowVM>(b);
                        vm.DaysOut = (int)(now - b.BorrowDate).TotalDays;
                        vm.IsLate = !b.IsReturned && vm.DaysOut > LATE_DAYS;

                        return vm;
                    }),
                    SearchUser = searchUser,
                    SearchBook = searchBook,
                    FilterReturned = filterReturned
                };
                return new Response<BorrowRecordListVM>(vm, "OK", true);
            }catch (Exception ex)
            {
                return new Response<BorrowRecordListVM>(null, ex.Message, false);

            }
        }

        public async Task<Response<string>> MarkAsReturnedAsync(int borrowId)
        {
            try
            {
                var record = await _borrowRepo.GetAll(b => b.Id == borrowId)
                    .Include(b => b.Book).FirstOrDefaultAsync();

                if (record == null)
                    return new Response<string>(null, "Record not found.", false);

                if (record.IsReturned)
                    return new Response<string>(null, "Already returned.", false);
                if(!record.ReturnRequested)
                    return new Response<string>(null, "No pending return request found for this record.", false);


                record.IsReturned = true;
                record.ReturnDate = DateTime.UtcNow;
                record.ReturnRequested = false;
                if (record.Book != null)
                {
                    record.Book.Quantity++;
                    _bookRepo.Update(record.Book);
                }
                _borrowRepo.Update(record);
                await _unitOfWork.SaveChangesAsync();

                return new Response<string>(null, "Marked as returned.", true);
            }
            catch (Exception ex)
            {
                return new Response<string>(null, ex.Message, false);
            }
        }
        public async Task<Response<IEnumerable<PendingReturnVM>>> GetPendingReturnsAsync()
        {
            try
            {
                var pending = await _borrowRepo
                    .GetAll(b => b.ReturnRequested && !b.IsReturned)
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderBy(b => b.BorrowDate)
                    .Select(b => new PendingReturnVM
                    {
                        BorrowId = b.Id,
                        BookId = b.BookId,
                        BookTitle = b.Book != null ? b.Book.Title : "—",
                        BookImage = b.Book != null ? b.Book.ImagePath : null,
                        UserName = b.User != null ? b.User.FullName : "—",
                        UserEmail = b.User != null ? b.User.Email : "—",
                        BorrowDate = b.BorrowDate,
                        DaysOut = (int)(DateTime.Now - b.BorrowDate).TotalDays
                    })
                    .ToListAsync();

                return new Response<IEnumerable<PendingReturnVM>>(pending, "OK", true);
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<PendingReturnVM>>(
                    null, $"An error occurred: {ex.Message}", false);
            }
        }
        public async Task<Response<UserListVM>> GetUsersAsync(string? searchQuery)
        {
            try
            {
                var allBorrows = await _borrowRepo.GetAll().ToListAsync();
                var users = _userManager.Users.ToList();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var s = searchQuery.Trim().ToLower();
                    users = users.Where(u =>
                        u.FullName.ToLower().Contains(s) ||
                        (u.Email != null && u.Email.ToLower().Contains(s))).ToList();

                }

                var vm = new UserListVM
                {
                    Users = users.Select(u =>
                    {
                        var map = _mapper.Map<UserRowVM>(u);

                        map.TotalBorrows = allBorrows.Count(b => b.UserId == u.Id);
                        map.ActiveBorrows = allBorrows.Count(b => b.UserId == u.Id && !b.IsReturned);
                        return map;
                    }).OrderByDescending(u => u.CreatedAt),
                    SearchQuery = searchQuery
                };
                return new Response<UserListVM>(vm, "OK", true);
            }
            catch (Exception ex)
            {
                return new Response<UserListVM>(null, ex.Message, false);
            }
        }

        public async Task<Response<string>> ToggleUserActiveAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return new Response<string>(null, "User not found.", false);

                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);

                var status = user.IsActive ? "activated" : "deactivated";
                return new Response<string>(null, $"User {status} successfully.", true);
            }
            catch (Exception ex)
            {
                return new Response<string>(null, ex.Message, false);
            }
        }

        public async Task<Response<UserBorrowHistoryVM>> GetUserBorrowHistoryAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null) 
                    return new Response<UserBorrowHistoryVM>(null, "User not found.", false);

                var records = await _borrowRepo
                    .GetAll(b => b.UserId == userId)
                    .Include(b => b.Book)
                    .OrderByDescending(b => b.BorrowDate)
                    .ToListAsync();

                var now = DateTime.UtcNow;
                var vm = new UserBorrowHistoryVM
                {
                    UserId = userId,
                    FullName = user.FullName,
                    History = records.Select(b =>
                    {
                        var map = _mapper.Map<BorrowRecordRowVM>(b);

                        map.UserName = user.FullName;
                        map.UserEmail = user.Email ?? "—";

                        map.DaysOut = (int)(now - b.BorrowDate).TotalDays;
                        map.IsLate = !b.IsReturned && map.DaysOut > LATE_DAYS;

                        return map;
                    })
                };
                return new Response<UserBorrowHistoryVM>(vm, "OK", true);
            }
            catch (Exception ex)
            {
                return new Response<UserBorrowHistoryVM>(null, ex.Message, false);
            }
        }
    }
}
