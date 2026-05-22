using Library.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BorrowRecord>()
                .HasOne(b => b.Book)
                .WithMany(br => br.BorrowRecords)
                .HasForeignKey(b => b.BookId);

            builder.Entity<BorrowRecord>()
                .HasOne(b => b.User)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(b => b.UserId);
        }
    }
}
