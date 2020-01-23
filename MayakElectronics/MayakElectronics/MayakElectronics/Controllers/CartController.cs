using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MayakElectronics.Models;
using MayakElectronics.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Controllers
{

    public class CartController : Controller
    {
        private Cart _cart;
        private readonly ElectronicsDbContext _db;
        public CartController(ElectronicsDbContext db, Cart cart)
        {
            _db = db;
            _cart = cart;

        }
        public ViewResult Index()
        {
            var items = _cart.GetCartItems();
            _cart.CartItems = items;

            var cVM = new CartViewModel
            {
                Cart = _cart,
                CartTotal = _cart.GetCartTotal()
            };

            return View(cVM);
        }

        public void AddToCart(int productId)
        {
            var selectedProduct =_db.Products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct != null)
            {
                _cart.AddToCart(selectedProduct, 1);
            }
        }
        
        public RedirectToActionResult AddToCartInCart(int productId)
        {
            var selectedProduct =_db.Products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct != null)
            {
                _cart.AddToCart(selectedProduct, 1);
            }

            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromCart(int productId)
        {
            var selectedProduct = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct != null)
            {
                _cart.RemoveFromCart(selectedProduct);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveTotally(int productId)
        {
            var selectedProduct = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct != null)
            {
                _cart.RemoveTotally(selectedProduct);
            }
            return RedirectToAction("Index");
        }
    }
}