using Library.BLL.ModelVM.Book;
using Library.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.ModelVM.Library
{
    public class LibraryBooksVM
    {
        public IEnumerable<BookVM> Books { get; set; } = Enumerable.Empty<BookVM>();

        // Search & filter state (preserved across requests)
        public string? SearchTitle { get; set; }
        public string? SearchAuthor { get; set; }
        public Category? SelectedCategory { get; set; }
    }
}
