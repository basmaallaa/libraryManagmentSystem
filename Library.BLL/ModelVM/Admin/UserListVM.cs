using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Admin
{
    public class UserListVM
    {
        public IEnumerable<UserRowVM> Users { get; set; } = Enumerable.Empty<UserRowVM>();
        public string? SearchQuery { get; set; }
    }
    public class UserRowVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public int TotalBorrows { get; set; }
        public int ActiveBorrows { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserBorrowHistoryVM
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<BorrowRecordRowVM> History { get; set; } = Enumerable.Empty<BorrowRecordRowVM>();
    }
}
