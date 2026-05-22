using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Admin
{
    public class DashboardVM
    {
        // ── Stat Cards ──
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveBorrowings { get; set; }
        public int ReturnedBooks { get; set; }

        // ── Recent Data ──
        public IEnumerable<RecentBookVM> RecentBooks { get; set; } = Enumerable.Empty<RecentBookVM>();
        public IEnumerable<RecentBorrowingVM> RecentBorrowings { get; set; } = Enumerable.Empty<RecentBorrowingVM>();

        // ── Chart Data (JSON-serialisable) ──
        public IEnumerable<MonthlyBorrowStatVM> MonthlyStats { get; set; } = Enumerable.Empty<MonthlyBorrowStatVM>();
        public IEnumerable<CategoryStatVM> CategoryStats { get; set; } = Enumerable.Empty<CategoryStatVM>();
    }
    public class RecentBookVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecentBorrowingVM
    {
        public int BorrowId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }

    public class MonthlyBorrowStatVM
    {
        public string Month { get; set; } = string.Empty;
        public int Borrowed { get; set; }
        public int Returned { get; set; }
    }

    public class CategoryStatVM
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
