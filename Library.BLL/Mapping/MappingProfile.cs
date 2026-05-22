using AutoMapper;
using Library.BLL.ModelVM.Admin;
using Library.BLL.ModelVM.Book;
using Library.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddBookVM, Book>();
            CreateMap<UpdateBookVM, Book>();
            CreateMap<Book, BookVM>();

            // ───────── Books ─────────
            CreateMap<Book, RecentBookVM>()
                .ForMember(dest => dest.ImagePath,
                           opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.Category,
                           opt => opt.MapFrom(src => src.Category.ToString()));

            // ───────── Borrow Records ─────────
            CreateMap<BorrowRecord, RecentBorrowingVM>()
                .ForMember(dest => dest.BookTitle,
                           opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : "—"))
                .ForMember(dest => dest.UserName,
                           opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "—"))
                .ForMember(dest => dest.UserEmail,
                           opt => opt.MapFrom(src => src.User != null ? src.User.Email : "—"));

            // ───────── Borrow Record Row ─────────
            CreateMap<BorrowRecord, BorrowRecordRowVM>()
                .ForMember(dest => dest.BookTitle,
                           opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : "—"))
                .ForMember(dest => dest.BookImage,
                           opt => opt.MapFrom(src => src.Book != null ? src.Book.ImagePath : null))
                .ForMember(dest => dest.UserName,
                           opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "—"))
                .ForMember(dest => dest.UserEmail,
                           opt => opt.MapFrom(src => src.User != null ? src.User.Email : "—"));

            // ───────── User Row ─────────
            CreateMap<ApplicationUser, UserRowVM>()
                .ForMember(d => d.Email,
                    o => o.MapFrom(s => s.Email ?? "—"))
                .ForMember(d => d.ProfileImage,
                    o => o.MapFrom(s => s.ProfileImage))
                .ForMember(d => d.IsActive,
                    o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.CreatedAt,
                    o => o.MapFrom(s => s.CreatedAt));
        }
    }
}
