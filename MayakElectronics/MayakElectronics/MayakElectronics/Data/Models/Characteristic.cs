using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MayakElectronics.Models
{
    public class Characteristic : IComponents
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название характеристики")]
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public Category Category { get; set; }
        public ValueType Type { get; set; }

        public virtual List<CharValue> CharValues { get; set; }
    }

    public enum ValueType
    {
        @string,
        number
    }
}