using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Data.Models
{
    public class AreBought
    {
        public int Id { get; set; }
        public int FirstProductId { get; set; }
        public int SecondProductId { get; set; }
        public int Count { get; set; }
    }
}
