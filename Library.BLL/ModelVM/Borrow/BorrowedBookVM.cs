using Library.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Borrow
{
    public class BorrowedBookVM
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Category Category { get; set; }
        public string? ImagePath { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }

        public bool ReturnRequested { get; set; }
    }
}
