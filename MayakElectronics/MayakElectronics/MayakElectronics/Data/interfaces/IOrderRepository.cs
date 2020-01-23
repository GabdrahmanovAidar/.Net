using MayakElectronics.Data.Models;
using MayakElectronics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Data.interfaces
{
    public interface IOrderRepository
    {
        void CreateOrder(OrderPickUp orderPickUp, string userId);
        void CreateOrder(OrderCourier orderCourier, string userId);
        void CreateOrder(OrderPost orderPost);

    }
}
