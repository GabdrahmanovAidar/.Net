using System.ComponentModel.DataAnnotations;
using MayakElectronics.Models;

namespace MayakElectronics.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Name{ get; set; }

        public int Rate { get; set; }

        public Product Product { get; set; }
    }
}