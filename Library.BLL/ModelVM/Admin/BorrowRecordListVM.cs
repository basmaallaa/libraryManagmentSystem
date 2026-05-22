using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Admin
{
    public class BorrowRecordListVM
    {
        public IEnumerable<BorrowRecordRowVM> Records { get; set; } = Enumerable.Empty<BorrowRecordRowVM>();

        // Filter/search state
        public string? SearchUser { get; set; }
        public string? SearchBook { get; set; }
        public bool? FilterReturned { get; set; }  // null = all, true = returned, false = active
    }

    public class BorrowRecordRowVM
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookImage { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsLate { get; set; }   // > 14 days and not returned
        public int DaysOut { get; set; }
    }
}
