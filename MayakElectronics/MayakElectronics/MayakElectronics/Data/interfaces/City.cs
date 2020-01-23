using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Models
{
    public class City : IComponents
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual List<StoreLocation> StoreLocations { get; set; }
    }
}