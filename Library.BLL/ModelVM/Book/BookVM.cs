using Library.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Book
{
    public class BookVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public Category Category { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string? ImagePath { get; set; }
    }
}
