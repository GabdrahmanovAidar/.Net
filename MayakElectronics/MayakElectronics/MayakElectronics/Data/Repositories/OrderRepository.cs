using MayakElectronics.Data.interfaces;
using MayakElectronics.Data.Models;
using MayakElectronics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ElectronicsDbContext _db;
        private readonly Cart _cart;

        public OrderRepository(ElectronicsDbContext db, Cart cart)
        {
            _db = db;
            _cart = cart;
        }
        public void CreateOrder(OrderPost order)
        {
            order.OrderPlaced = DateTime.Now;
            _db.OrdersPost.Add(order);
            _db.SaveChanges();

            order.OrderTotal = _cart.GetCartTotal();

            var cartItems = _cart.CartItems;

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetailPost()
                {
                    Amount = item.Amount,
                    ProductId = item.Product.Id,
                    OrderPostId = order.OrderPostId,
                    Price = item.Product.Price
                };

                _db.OrderDetailsPost.Add(orderDetail);
            }
            _db.SaveChanges();
        }

        public void CreateOrder(OrderPickUp order, string userId)
        {
            order.UserId = userId;
            order.OrderPlaced = DateTime.Now;
            _db.OrdersPickUp.Add(order);
            _db.SaveChanges();

            order.OrderTotal = _cart.GetCartTotal();

            var cartItems = _cart.CartItems;

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetailPickUp()
                {
                    Amount = item.Amount,
                    ProductId = item.Product.Id,
                    OrderPickUpId = order.OrderPickUpId,
                    Price = item.Product.Price
                };

                _db.OrderDetailsPickUp.Add(orderDetail);
            }
            _db.SaveChanges();
        }

        public void CreateOrder(OrderCourier order, string userId)
        {
            order.UserId = userId;
            order.OrderPlaced = DateTime.Now;
            _db.OrdersCourier.Add(order);
            _db.SaveChanges();

            order.OrderTotal = _cart.GetCartTotal();

            var cartItems = _cart.CartItems;

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetailCourier()
                {
                    Amount = item.Amount,
                    ProductId = item.Product.Id,
                    OrderCourierId = order.OrderCourierId,
                    Price = item.Product.Price
                };

                _db.OrderDetailsCourier.Add(orderDetail);
            }
            _db.SaveChanges();
        }
    }
}
