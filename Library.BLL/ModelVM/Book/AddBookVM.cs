using Library.DAL.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Library.BLL.ModelVM.Book
{
    public class AddBookVM
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }
        [Required]
        [EnumDataType(typeof(Category))]
        public Category Category { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Range(0,int.MaxValue)]
        public int Quantity { get; set; }
        public IFormFile? Image { get; set; }
    }
}
