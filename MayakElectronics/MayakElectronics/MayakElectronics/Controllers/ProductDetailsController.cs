using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MayakElectronics.Data.Models;
using MayakElectronics.Database;
using MayakElectronics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly ElectronicsDbContext _db;
        private readonly EfProductRepository _efProductRepository;

        public ProductDetailsController(ElectronicsDbContext db)
        {
            _db = db;
            _efProductRepository = new EfProductRepository();
        }

        [HttpGet]
        public IActionResult SingleProductDetails(int productId)
        {
            _efProductRepository.Products
                .ToList();
            var product = _efProductRepository.Products
                .First(p => p.Id == productId);
            _efProductRepository.Characteristics
                .ToList();
            _efProductRepository.CharValues
                .Where(cv => cv.Product == product)
                .ToList();
            _efProductRepository.Reviews
                .Where(r => r.Product == product)
                .ToList();
            bool flag = true;
            var userName = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.UserName = userName;
            var allReviews = _efProductRepository.Reviews
                .Where(r => r.Product.Id == productId)
                .ToList();
            foreach (var e in allReviews)
            {
                if (e.Name == userName)
                    flag = false;
            }

            ViewBag.Flag = flag;

            if (product.Reviews == null)
                product.Reviews = new List<Review>();
            if (product.CharValues == null)
                product.CharValues = new List<CharValue>();
            return View(new Review {Product = product});
        }

        [Authorize]
        [HttpPost]
        public IActionResult SingleProductDetails(int productId, string ratting, string content)
        {
            _efProductRepository.Products
                .ToList();
            var product = _efProductRepository.Products
                .First(p => p.Id == productId);
            bool flag = true;
            var userName = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.UserName = userName;
            var allReviews = _efProductRepository.Reviews
                .Where(r => r.Product.Id == productId)
                .ToList();
            foreach (var e in allReviews)
            {
                if (e.Name == userName)
                    flag = false;
            }

            ViewBag.Flag = flag;

            var review = new Review
            {
                Content = content,
                Name = userName,
                Product = product,
                Rate = int.Parse(ratting),
            };
            _efProductRepository.SaveReview(review);
            product.Reviews = _efProductRepository.Reviews
                .Where(r => r.Product.Id == productId)
                .ToList();
            _efProductRepository.UpdateProductRate(product);
            return RedirectToAction("SingleProductDetails", "ProductDetails", new {productId});
        }

        public FileContentResult GetImage(int productId)
        {
            var product = _efProductRepository.Products
                .FirstOrDefault(p => p.Id == productId);
            return product != null ? File(product.ImageData, product.ImageMimeType) : null;
        }
    }
}