using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Borrow
{
    public class PendingReturnVM
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookImage { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public int DaysOut { get; set; }
    }
}
