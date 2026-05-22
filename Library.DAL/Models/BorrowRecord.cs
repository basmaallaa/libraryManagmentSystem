using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DAL.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        public bool ReturnRequested { get; set; } = false;

        public bool IsReturned { get; set; } = false;

        public ApplicationUser? User { get; set; }

        public Book? Book { get; set; }
    }
}
