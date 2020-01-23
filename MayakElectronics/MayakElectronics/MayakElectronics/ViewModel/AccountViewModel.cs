using MayakElectronics.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.ViewModel
{
    public class AccountViewModel
    {
        public List<OrderPickUp> orderPickUp { get; set; }
        public List<OrderCourier> orderCouriers { get; set; }
    }
}
