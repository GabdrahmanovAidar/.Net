using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MayakElectronics.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Models
{
    public class Product : IComponents
    {
        public bool Hide { get; set; }
        
        [HiddenInput(DisplayValue = false)] 
        public int Id { get; set; }

        [Display(Name = "Название товара")]
        [Required(ErrorMessage = "Пожалуйста, введите название товара")]
        public string Name { get; set; }

        [Display(Name = "Цена (руб)")]
        [Required]
        [Range(0, maximum: double.MaxValue, ErrorMessage = "Пожалуйста, введите положительное значение для цены")]
        public double Price { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Пожалуйста, введите описание для товара")]
        public string Description { get; set; }

        [Display(Name = "Категория")] 
        public string CategoryName { get; set; }

        public Category Category { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageMimeType { get; set; }

        public double Rate { get; set; }
        
        public virtual List<CartItem> CartItems { get; set; }
        public virtual List<CharValue> CharValues { get; set; }
        public virtual List<Review> Reviews { get; set; }
    }
}