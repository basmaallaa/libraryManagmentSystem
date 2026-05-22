using AutoMapper;
using Library.BLL.Common;
using Library.BLL.ModelVM.Book;
using Library.BLL.Services.Interfaces;
using Library.DAL.Enums;
using Library.DAL.Models;
using Library.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Implementations
{
    public class LibraryService : ILibraryService
    {
        private readonly IGenericRepo<Book> _bookRepo;
        private readonly IMapper _mapper;

        public LibraryService(IGenericRepo<Book> bookRepo, IMapper mapper)
        {
            _bookRepo = bookRepo;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<BookVM>>> GetFilteredBooksAsync(string? searchTitle, string? searchAuthor, Category? category)
        {
            try
            {
                var books = _bookRepo.GetAll();
                if (!string.IsNullOrWhiteSpace(searchTitle))
                {
                    var titleLower = searchTitle.Trim().ToLower();
                    books = books.Where(b => b.Title.ToLower().Contains(titleLower));
                }
                if (!string.IsNullOrWhiteSpace(searchAuthor))
                {
                    var authorLower = searchAuthor.Trim().ToLower();
                    books = books.Where(b => b.Author.ToLower().Contains(authorLower));
                }
                if (category.HasValue)
                {
                    books = books.Where(b => b.Category == category.Value);
                }

                var bookVMs = await books.OrderBy(b => b.Title).ToListAsync();
                var mapped = _mapper.Map<IEnumerable<BookVM>>(bookVMs);
                return new Response<IEnumerable<BookVM>>
                (
                     mapped,
                     "Books retrieved successfully.",
                     true
                );

            }
            catch (Exception ex) 
            {
                return new Response<IEnumerable<BookVM>>
                (
                    null,
                    $"An error occurred while retrieving books: {ex.Message}",
                    false
                );
            }
        }
    }
}

