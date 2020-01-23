using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Models
{
    public class Category : IComponents
    {
        [Key]
        public string Name { get; set; }
        
        public virtual List<Product> Products { get; set; }
        
        [BindProperty]
        public virtual List<Characteristic> Characteristics { get; set; }
    }
}