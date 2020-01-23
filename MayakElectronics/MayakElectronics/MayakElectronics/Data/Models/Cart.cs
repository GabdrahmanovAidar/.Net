using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Models
{
    public class Cart
    {
        private readonly ElectronicsDbContext _db;

        private Cart(ElectronicsDbContext db)
        {
            _db = db;
        }
        public string CartId { get; set; }
        public List<CartItem> CartItems { get; set; }

        public static Cart GetCart(IServiceProvider service)
        {
            ISession session = service.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            var context = service.GetService<ElectronicsDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new Cart(context) { CartId = cartId };
        }

        public void AddToCart(Product product, int amount)
        {
            var cartItem = _db.CartItems.SingleOrDefault(
                s => s.Product.Id == product.Id && s.CartId == CartId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = CartId,
                    Product = product,
                    Amount = 1
                };

                _db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Amount++;
            }
            _db.SaveChanges();
        }

        public int RemoveFromCart(Product product)
        {
            var cartItem =
                _db.CartItems.SingleOrDefault(
                    s => s.Product.Id == product.Id && s.CartId == CartId);

            var localAmount = 0;

            if (cartItem != null)
            {
                if (cartItem.Amount > 1)
                {
                    cartItem.Amount--;
                    localAmount = cartItem.Amount;
                }
                else
                {
                    _db.CartItems.Remove(cartItem);
                }
            }

            _db.SaveChanges();
            return localAmount;
        }

        public void RemoveTotally(Product product)
        {
            var cartItem =
                _db.CartItems.SingleOrDefault(
                    s => s.Product.Id == product.Id && s.CartId == CartId);
            _db.CartItems.Remove(cartItem);
            _db.SaveChanges();
        }

        public List<CartItem> GetCartItems()
        {
            return CartItems ??
                (CartItems =
                    _db.CartItems.Where(c => c.CartId == CartId)
                        .Include(s => s.Product)
                        .ToList());
        }

        public void ClearCart()
        {
            var cartItems = _db.CartItems.Where(cart => cart.CartId == CartId);

            _db.CartItems.RemoveRange(cartItems);
            
            _db.SaveChanges();
        }

        public double GetCartTotal()
        {
            var total = _db.CartItems.Where(c => c.CartId == CartId)
                .Select(c => c.Product.Price * c.Amount).Sum();
            return total;
        }
    }
}
