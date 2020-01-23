using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MayakElectronics.Data.interfaces;
using MayakElectronics.Data.Models;
using MayakElectronics.Database;
using MayakElectronics.Models;
using MayakElectronics.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ElectronicsDbContext _db;
        private readonly IOrderRepository _orderRepository;
        private readonly Cart _cart;

        public OrderController(IOrderRepository orderRepository, Cart cart, ElectronicsDbContext db)
        {
            _db = db;
            _orderRepository = orderRepository;
            _cart = cart;
        }

        public IActionResult SelectOrder()
        {
            var items = _cart.GetCartItems();
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;

            return View();
        }

        public IActionResult PickUp()
        {
            var items = _cart.GetCartItems();
            var user = _db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));            
            ViewBag.Email = user.Email;
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;
            return View();
        }

        [HttpPost]
        public IActionResult Pickup(OrderPickUp order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var text = "Your type of order - pick up. Please take your order. For more information follow the link: https://mayakelectronics20191126070610.azurewebsites.net/Account/YourAccount";
            var items = _cart.GetCartItems();
            var user = _db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = user.Email;
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;
            if (items.Count == 0)
            {
                ModelState.AddModelError("", "Cart is empty");
            }

            new EfProductRepository().UpdateAreBoughts(items.Select(i => i.Product).ToList());

            if (ModelState.IsValid)
            {
                _orderRepository.CreateOrder(order, userId);
                EmailSender.SendEmail(order.Email, text);
                _cart.ClearCart();
                return RedirectToAction("Complete");
            }

            return View(order);
        }

        public IActionResult CourierDelivery()
        {
            var items = _cart.GetCartItems();
            var user = _db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = user.Email;
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;
            return View();
        }

        [HttpPost]
        public IActionResult CourierDelivery(OrderCourier order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var text = "Your type of order - courier delivery. For more information follow the link: https://mayakelectronics20191126070610.azurewebsites.net/Account/YourAccount";
            var items = _cart.GetCartItems();
            ViewBag.Total = _cart.GetCartTotal();
            var user = _db.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.Email = user.Email;
            ViewBag.Items = items;
            if (items.Count == 0)
            {
                ModelState.AddModelError("", "Cart is empty");
            }

            new EfProductRepository().UpdateAreBoughts(items.Select(i => i.Product).ToList());

            if (ModelState.IsValid)
            {
                EmailSender.SendEmail(order.Email, text);
                _cart.ClearCart();
                _orderRepository.CreateOrder(order, userId);
                return RedirectToAction("Complete");
            }

            return View(order);
        }

        public IActionResult PostDelivery()
        {
            var items = _cart.GetCartItems();
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;
            return View();
        }

        [HttpPost]
        public IActionResult PostDelivery(OrderPost order)
        {
            var text = "Your type of order - post delivery. For more information follow the link: https://mayakelectronics20191126070610.azurewebsites.net/Account/YourAccount";
            var items = _cart.GetCartItems();
            ViewBag.Total = _cart.GetCartTotal();
            ViewBag.Items = items;
            if (items.Count == 0)
            {
                ModelState.AddModelError("", "Cart is empty");
            }

            new EfProductRepository().UpdateAreBoughts(items.Select(i => i.Product).ToList());

            if (ModelState.IsValid)
            {
                EmailSender.SendEmail(order.Email, text);
                _cart.ClearCart();
                _orderRepository.CreateOrder(order);
                return RedirectToAction("Complete");
            }

            return View(order);
        }

        public IActionResult Complete()
        {
            ViewBag.Message = "Congratulation! Your Order has been finished successfully";
            return View();
        }
    }
}