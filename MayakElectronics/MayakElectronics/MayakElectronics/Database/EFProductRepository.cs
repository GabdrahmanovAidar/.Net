using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MayakElectronics.Data.Models;
using MayakElectronics.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace MayakElectronics.Database
{
    public class EfProductRepository 
    {
        ElectronicsDbContext context = new ElectronicsDbContext();

        public IEnumerable<Product> Products => context.Products;
        public IEnumerable<Category> Categories => context.Categories;
        public IEnumerable<Characteristic> Characteristics => context.Characteristics;
        public IEnumerable<CharValue> CharValues => context.CharValues;
        public IEnumerable<City> Cities => context.Cities;
        public IEnumerable<Review> Reviews => context.Reviews;
        public IEnumerable<AreBought> AreBoughts => context.AreBoughts;
        public IEnumerable<OrderCourier> OrderCouriers => context.OrdersCourier;
        public IEnumerable<OrderPickUp> OrderPickUps => context.OrdersPickUp;

        public OrderPickUp SavePickUpOrder(int id, Status status)
        {
            var dbEntity = context.OrdersPickUp.Find(id);
            dbEntity.Status = status;
            var text = $"Your order has changed status. Actual status is {status}. For more information follow the link: https://mayakelectronics20191126070610.azurewebsites.net/Account/YourAccount";
            EmailSender.SendEmail(dbEntity.Email, text);
            context.SaveChanges();
            return dbEntity;
        }
        
        public OrderCourier SaveCorierOrder(int id, Status status)
        {
            var dbEntity = context.OrdersCourier.Find(id);
            dbEntity.Status = status;
            var text = $"Your order has changed status. Actual status is {status}. For more information follow the link: https://mayakelectronics20191126070610.azurewebsites.net/Account/YourAccount";
            EmailSender.SendEmail(dbEntity.Email, text);
            context.SaveChanges();
            return dbEntity;
        }

        public void UpdateAreBoughts(List<Product> products)
        {
            var ids = products
                .Select(p => p.Id)
                .ToList();
            for (var i = 0; i < ids.Count; i++)
            for (var j = 0; j < ids.Count; j++)
            {
                if (j == i) continue;
                var ab = context.AreBoughts
                    .Where(e => e.FirstProductId == products[i].Id)
                    .FirstOrDefault(e => e.SecondProductId == products[j].Id);
                if (ab == null)
                    context.AreBoughts
                        .Add(new AreBought
                        {
                            FirstProductId = products[i].Id,
                            SecondProductId = products[j].Id,
                            Count = 1
                        });
                else ab.Count++;
                context.SaveChanges();
            }
        }

        public void UpdateProductRate(Product product)
        {
            product.Rate = Math.Round((double) product.Reviews.Sum(r => r.Rate) / product.Reviews.Count, 1);
            context.SaveChanges();
        }

        public void SaveReview(Review review)
        {
            context.Reviews.Add(review);
            context.SaveChanges();
        }

        public void SaveCity(City city)
        {
            if (city.Id == 0)
                context.Cities.Add(city);
            else
            {
                var dbEntry = context.Cities.Find(city.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = city.Name;
                }
            }

            context.SaveChanges();
        }

        public void SaveProduct(Product product)
        {
            var blank = false;
            if (product.Id == 0)
                context.Products.Add(product);
            else
            {
                var dbEntry = context.Products.Find(product.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    if (dbEntry.Category != product.Category)
                    {
                        blank = true;
                        foreach (var charValue in product.CharValues)
                        {
                            var delEntry = context.CharValues.Find(charValue.Id);
                            context.CharValues.Remove(delEntry);
                        }
                    }

                    dbEntry.Category = product.Category;
                    dbEntry.Price = product.Price;
                    if (product.ImageData != null)
                    {
                        dbEntry.ImageData = product.ImageData;
                        dbEntry.ImageMimeType = product.ImageMimeType;
                    }

                }
            }

            context.SaveChanges();

            if (blank || product.CharValues == null) return;
            foreach (var characteristicValue in product.CharValues)
            {
                SaveCharacteristicValue(characteristicValue, product);
            }
        }

        public void SaveCharacteristicValue(CharValue characteristicValue, Product product)
        {
            var characteristic = context.Characteristics.Find(characteristicValue.Characteristic.Id);
            characteristicValue.Characteristic = characteristic;
            if (characteristicValue.Id == 0)
            {
                characteristicValue.Product = product;
                context.CharValues.Add(characteristicValue);
            }
            else
            {
                var dbEntry = context.CharValues.Find(characteristicValue.Id);
                dbEntry.Characteristic = characteristicValue.Characteristic;
                dbEntry.numValue = characteristicValue.numValue;
                dbEntry.strValue = characteristicValue.strValue;
                //dbEntry.Product = product;
            }

            context.SaveChanges();
        }

        public void SaveCategory(Category category)
        {
            var dbEntry = context.Categories.Find(category.Name);
            if (dbEntry != null)
            {
                dbEntry.Name = category.Name;
            }
            else
                context.Categories.Add(category);

            context.SaveChanges();

            foreach (var characteristic in category.Characteristics)
            {
                SaveCharacteristic(characteristic, category);
            }

            context.SaveChanges();
        }

        public void SaveCharacteristic(Characteristic characteristic, Category category)
        {
            if (characteristic.Id == 0)
            {
                context.Characteristics.Add(characteristic);
            }
            else
            {
                var dbEntry = context.Characteristics.Find(characteristic.Id);
                dbEntry.Name = characteristic.Name;
                dbEntry.CategoryName = category.Name;
                dbEntry.Type = characteristic.Type;
            }

            context.SaveChanges();
        }

        public Product DeleteProduct(int productId, bool blank = false)
        {
            var dbEntry = context.Products.Find(productId);
            if (dbEntry == null) return null;
            if (context.OrderDetailsCourier.Any(order => order.ProductId == productId) ||
                context.OrderDetailsPickUp.Any(order => order.ProductId == productId))
            {
                dbEntry.Hide = true;
                //if (blank)
                //{
                    dbEntry.CategoryName = null;
                //}
                context.SaveChanges();
                return null;
            }
            
            foreach (var cv in context.CharValues.Where(cv => cv.Product.Id == productId).ToList())
            {
                context.CharValues.Remove(cv);
            }

            context.Products.Remove(dbEntry);
            context.SaveChanges();

            return dbEntry;
        }

        public Category DeleteCategory(string name)
        {
            var dbEntry = context.Categories.Find(name);
            context.Products.ToList();
            if (dbEntry == null) return null;
            foreach (var product in dbEntry.Products ?? new List<Product>())
            {
                DeleteProduct(product.Id, true);
            }
            context.Categories.Remove(dbEntry);
            context.SaveChanges();

            return dbEntry;
        }

        public Characteristic DeleteCharacteristic(int id)
        {
            var dbEntry = context.Characteristics.Find(id);
            if (dbEntry == null) return null;
            context.Characteristics.Remove(dbEntry);
            context.SaveChanges();

            return dbEntry;
        }

        public CharValue DeleteCharValue(int id)
        {
            var dbEntry = context.CharValues.Find(id);
            if (dbEntry == null) return null;
            context.CharValues.Remove(dbEntry);
            context.SaveChanges();

            return dbEntry;
        }

        public City DelteCity(int id)
        {
            var dbEntry = context.Cities.Find(id);
            if (dbEntry == null) return null;
            context.Cities.Remove(dbEntry);
            context.SaveChanges();

            return dbEntry;
        }
    }
}