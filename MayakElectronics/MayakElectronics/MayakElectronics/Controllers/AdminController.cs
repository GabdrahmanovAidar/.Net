using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using MayakElectronics.Data.Models;
using MayakElectronics.Database;
using MayakElectronics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ValueType = MayakElectronics.Models.ValueType;

namespace MayakElectronics.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly EfProductRepository productRepository;

        public AdminController()
        {
            productRepository = new EfProductRepository();
        }
        
        public IActionResult Index()
        {
            return View(productRepository);
        }

        public ViewResult Edit(string type, int id = -1, string name = null)
        {
            switch (type)
            {
                case "OrderPickUp":
                    var orderPickUp = productRepository.OrderPickUps.First(order => order.OrderPickUpId == int.Parse(name));
                    return View("EditOrderPickUp", orderPickUp);
                case "OrderCourier":
                    var orderCourier = productRepository.OrderCouriers.First(order => order.OrderCourierId == int.Parse(name));
                    return View("EditOrderCorier", orderCourier);
                case "Product":
                    var product = productRepository.Products
                        .FirstOrDefault(p => p.Id == id);
                    var chosenChateg = productRepository.Categories.Where(c => c.Name == product.CategoryName);
                    ViewData["_Characteristics"] =
                        productRepository.Characteristics.Where(ch => ch.Category == chosenChateg.First());
                    ViewData["_CharValues"] = productRepository.CharValues.Where(cv => cv.Product == product);    
                    ViewBag.myCategories = productRepository.Categories.Select(i => new SelectListItem
                    {
                        Text = i.Name, Value = i.Name, Selected = product.CategoryName == i.Name
                    });
                    return View(product);
                case "Category":
                    var category = productRepository.Categories
                        .FirstOrDefault(p => p.Name == name);
                    category.Characteristics = productRepository.Characteristics.Where(c => c.CategoryName == category.Name).ToList();
                    if(category.Characteristics == null)
                        category.Characteristics = new List<Characteristic> 
                            {new Characteristic{Id = 0, 
                                Category = category, 
                                CategoryName = category.Name,
                                Name = "Characteristic",
                                Type = ValueType.@string
                            }};
                    return View("EditCategory", category);
                case "Characteristic":
                    return View(productRepository.Characteristics
                        .FirstOrDefault(p => p.Id == id));
                case "City":
                    return View(productRepository.Cities.FirstOrDefault(c => c.Id == id));
                default:
                    return View("Index", productRepository);
            }
        }
        
        // Перегруженная версия Edit() для сохранения изменений
        [HttpPost]
        public ActionResult ProductEdit(Product product, IFormFile image = null, string category = null)
        {

            if (!ModelState.IsValid)
            {
                var product_ = productRepository.Products
                    .FirstOrDefault(p => p.Id == product.Id);
                var chosenChateg = productRepository.Categories.Where(c => c.Name == product_.CategoryName);ViewData["_Characteristics"] =
                    productRepository.Characteristics.Where(ch => ch.Category == chosenChateg.First());
                ViewData["_CharValues"] = productRepository.CharValues.Where(cv => cv.Product == product);
                ViewBag.myCategories = productRepository.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name, Value = i.Name, Selected = product.CategoryName == i.Name
                });
                return View("Edit", product);
            }
            
            if (image != null)
            {
                product.ImageMimeType = image.ContentType;
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    product.ImageData = memoryStream.ToArray();
                }
            }

            if (category != null)
                product.Category = productRepository.Categories.FirstOrDefault(c => c.Name == category);


            productRepository.SaveProduct(product);
            TempData["message"] = string.Format("\"{0}\" has been changed", product.Name);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CategoryEdit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("EditCategory", category);
            }
            productRepository.SaveCategory(category);
            TempData["message"] = string.Format("\"{0}\" has been changed", category.Name);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CityEdit(City city)
        {
            if (city.Name == null) return View("Edit", city);
            productRepository.SaveCity(city);
            TempData["message"] = string.Format("\"{0}\" has been changed", city.Name);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int productId, string type, string name)
        {
            IComponents deletedProduct = null;
            switch (type)
            {
                case "Products":
                    deletedProduct = productRepository.DeleteProduct(productId);
                    break;
                case "Categories":
                    deletedProduct = productRepository.DeleteCategory(name);
                    break;
                case "Characteristics":
                    deletedProduct = productRepository.DeleteCharacteristic(productId);
                    break;
                case "CharValues":
                    productRepository.DeleteCharValue(productId);
                    break;
                case "Cities":
                    productRepository.DelteCity(productId);
                    break;
            }
            if (deletedProduct != null)
                TempData["message"] = $"\"{deletedProduct.Name}\" был удален";
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditPickUporder(Status status, int pickUpId)
        {
            var savePickUpOrder = productRepository.SavePickUpOrder(pickUpId, status);
            if (savePickUpOrder != null)
                TempData["message"] = $"Order has been changed";
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult EditCorierOrder(Status status, int corierId)
        {
            var saveCorierOrder = productRepository.SaveCorierOrder(corierId, status);
            if (saveCorierOrder != null)
                TempData["message"] = $"Order has been changed";
            return RedirectToAction("Index");
        }
        
        public IActionResult Create(string propertyName)
        {
            switch (propertyName)
            {
                case "Products":
                    ViewBag.myCategories = productRepository.Categories.Select(i => new SelectListItem
                    {
                        Text = i.Name, Value = i.Name
                    });
                    return View("Edit", new Product());
                case "Categories": return View("EditCategory", new Category
                {
                    Characteristics = new List<Characteristic>
                            {new Characteristic{Id = 0,
                                Name = "",
                                Type = ValueType.@string
                            }}
                });
                case "Cities": return View("Edit", new City());
            }
            return View("Edit");
        }
        
        public FileContentResult GetImage(int productId)
        {
            var product = productRepository.Products
                .FirstOrDefault(p => p.Id == productId);

            return product != null ? File(product.ImageData, product.ImageMimeType) : null;
        }
    }
}