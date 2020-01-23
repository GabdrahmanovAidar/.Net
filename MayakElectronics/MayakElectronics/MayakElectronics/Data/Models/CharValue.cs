using System.Collections.Generic;

namespace MayakElectronics.Models
{
    public class CharValue
    {
        public int Id { get; set; }
        public Characteristic Characteristic { get; set; }
        public Product Product { get; set; }
        
        public string strValue { get; set; }
        public decimal numValue { get; set; }
    }
}