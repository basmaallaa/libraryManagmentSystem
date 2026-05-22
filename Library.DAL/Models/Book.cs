using Library.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DAL.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public Category Category { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}
