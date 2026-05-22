using Library.BLL.Common;
using Library.BLL.ModelVM.Admin;
using Library.BLL.ModelVM.Borrow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<Response<DashboardVM>> GetDashboardAsync();
        Task<Response<BorrowRecordListVM>> GetBorrowRecordsAsync(string? searchUser, string? searchBook, bool? filterReturned);
        Task<Response<string>> MarkAsReturnedAsync(int borrowId);
        Task<Response<IEnumerable<PendingReturnVM>>> GetPendingReturnsAsync();

        Task<Response<UserListVM>> GetUsersAsync(string? searchQuery);
        Task<Response<string>> ToggleUserActiveAsync(string userId);
        Task<Response<UserBorrowHistoryVM>> GetUserBorrowHistoryAsync(string userId);
    }
}
