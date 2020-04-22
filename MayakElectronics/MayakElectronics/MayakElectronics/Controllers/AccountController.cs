using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MayakElectronics.Database;
using MayakElectronics.Models;
using MayakElectronics.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElectronicsDbContext _db;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;
        private Task<AppIdentityUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AccountController(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager, ElectronicsDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                        return RedirectToAction("Index", "Electronics");
                    return Redirect(loginViewModel.ReturnUrl);
                }
            }
            ModelState.AddModelError("", "Username/Password is not correct");
            return View(loginViewModel);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                var user = new AppIdentityUser() { UserName = registerViewModel.UserName, Email = registerViewModel.Email };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Electronics"); ;
                }
            }
            ModelState.AddModelError("", "Username/Email already exist");
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Electronics");
        }

        public IActionResult YourAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserName = userName;
            ViewBag.Email = userEmail;
            var ordersC = _db.OrdersCourier.Where(x => x.UserId == userId).ToList();
            var ordersP = _db.OrdersPickUp.Where(x => x.UserId == userId).ToList();
            if (ordersC.Count != 0)
            {
                foreach (var order in ordersC)
                {
                    order.OrderDetails = _db.OrderDetailsCourier.Where(x => x.OrderCourierId == order.OrderCourierId).ToList();
                    foreach(var detail in order.OrderDetails)
                    {
                        detail.Product = _db.Products.Where(x => x.Id == detail.ProductId).FirstOrDefault();
                    }
                }
            }
            if (ordersP.Count != 0)
            {
                foreach (var order in ordersP)
                {
                    order.OrderDetails = _db.OrderDetailsPickUp.Where(x => x.OrderPickUpId == order.OrderPickUpId).ToList();
                    foreach (var detail in order.OrderDetails)
                    {
                        detail.Product = _db.Products.Where(x => x.Id == detail.ProductId).FirstOrDefault();
                    }
                }
            }
            var acVMD = new AccountViewModel() { orderCouriers = ordersC, orderPickUp = ordersP };
            return View(acVMD);
        }
    }
}