using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Korzh.EasyQuery.Linq;
using MayakElectronics.Database;
using MayakElectronics.Models;
using Microsoft.AspNetCore.Mvc;

namespace MayakElectronics.Controllers
{
    public class ElectronicsController : Controller
    {
        private ElectronicsDbContext _db;

        public ElectronicsController(ElectronicsDbContext db)
        {
            _db = db;
        }

        // GET: Electronics
        public ActionResult Index()
        {
            ViewData["CategoriesIndex"] = _db.Categories.ToList();
            var novelty = _db.Products
                .Where(p => p.Hide == false)
                .Skip(Math.Max(0, _db.Products.Count() - 8))
                .ToList();
            
            return View(novelty);
        }

        // GET: Electronics/TrackOrder/
        public ActionResult TrackOrder()
        {
            return View();
        }

        // GET: Electronics/Store
        public ActionResult Store()
        {
            return View();
        }

        // GET: Electronics/ShopGrid
        public IActionResult ShopGrid(string categoryName)
        {
            var products = new List<Product>();
            var category = _db.Categories.Find(categoryName);
            if (category != null)
            {
                var characteristics = _db.Characteristics.Where(c => c.Category == category).ToList();
                _db.CharValues.Where(cv => characteristics.Contains(cv.Characteristic)).ToList();
                products = _db.Products
                    .Where(p => p.Category.Name == categoryName)
                    .ToList();
                ViewData["Category"] = category;
            }
            else
            {
                _db.Categories.ToList();
                products = _db.Products.ToList();
            }

            ViewData["rate"] = 0;
            ViewData["from"] = 0;
            ViewData["to"] = 0;

            return View(products.Where(p => p.Hide == false).ToList());
        }

        [HttpPost]
        public IActionResult Filter(string category,string sortby, int from, int to, 
            int rate, List<CharValueStr> charValueStr, List<CharValueSNum> charValueSNum)
        {
            var cat              = _db.Categories.Find(category);
            ViewData["Category"] = cat;
            ViewData["rate"]     = rate;
            ViewData["from"]     = from;
            ViewData["to"]       = to;

            var cvStr = charValueStr
                .Where(cv => cv.check == true)
                .GroupBy(cv => cv.Characteristic)
                .ToDictionary(group => group.Key, group => group.Select(i => i.charValueStr).ToList());
            var cvNum = charValueSNum
                .Where(cv => cv.check == true)
                .GroupBy(cv => cv.Characteristic)
                .ToDictionary(group => group.Key, group => group.Select(i => i.charValueNum).ToList());

            var products = new List<Product>();
            if (to == 0 || from > to)
                products = _db
                    .Products
                    .Where(p => p.Rate >= rate).ToList();
            else products = _db
                    .Products
                    .Where(p => p.Price <= to)
                    .Where(p => p.Price >= from)
                    .Where(p => p.Rate >= rate).ToList();

            _db.Characteristics.ToList();
            _db.CharValues.ToList();

            var result = new List<Product>();

            foreach (var product in products)
            {
                var isGoodChoise = true;
                foreach (var cv in product.CharValues ?? new List<CharValue>())
                {
                    if (
                        !((cvStr.ContainsKey(cv.Characteristic.Name)
                        && cvStr[cv.Characteristic.Name].Contains(cv.strValue))
                        ||
                        (cvNum.ContainsKey(cv.Characteristic.Name)
                        && cvNum[cv.Characteristic.Name].Contains(cv.numValue)))
                        )
                        isGoodChoise = false;
                    
                }
                if (product.CharValues != null && isGoodChoise)
                    result.Add(product);
            }

            switch (sortby)
            {
                case "rating":
                    result = result.OrderByDescending(p => p.Rate).ToList();
                    break;
                case "price-asc":
                    result = result.OrderBy(p => p.Price).ToList();
                    break;
                case "price-desc" :
                    result = result.OrderByDescending(p => p.Price).ToList();
                    break;
            }

            return View("ShopGrid", result.Where(p => p.Hide == false).ToList());
        }

        // GET: Electronics/Wishlist
        public ActionResult Wishlist()
        {
            return View();
        }

        // GET: Electronics/Contact
        public ActionResult Contact()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Search(string text)
        {
            var result = new List<Product>(); 
            var categoriesList = new List<Category>();
            var products = _db.Products.ToList();
            var categories = _db.Categories.ToList();
            FullTextSearch(text, products, result);
            FullTextSearchOnCategory(text, categories, categoriesList);

            foreach (var category in categoriesList)
            {
                result.AddRange(category.Products.ToList());
            }

            if (result.Count == 0)
            {
                var a = _db.Products as IQueryable<Product>;
                result = a.FullTextSearchQuery(text).ToList();
            }

            return View("ShopGrid", result.Where(p => p.Hide == false).ToList());
        }
        
        private static void FullTextSearchOnCategory(string text, List<Category> items, List<Category> result)
        {
            foreach (var product in items)
            {
                var minLDWordsLength = 0;
                var minSumLevenshteinDistance = 0;
                foreach (var text_word in text.Split(' ', ',', '.', '/', '\\', '!', '\'', '"', ':', ';', '[', ']', '@',
                    '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '~', '|'))
                {
                    var minLevenshteinDistance = int.MaxValue;
                    var minLDWord = "";
                    foreach (var name in product.Name.Split(' ', ',', '.', '/', '\\', '!', '\'', '"', ':', ';', '[',
                        ']', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '~', '|'))
                    {
                        var simplifyText = SimplifyText(text_word);
                        var simplifyName = SimplifyText(name);
                        var levenshteinDistance = LevenshteinDistance(simplifyText, simplifyName);
                        if (levenshteinDistance < minLevenshteinDistance)
                        {
                            minLevenshteinDistance = levenshteinDistance;
                            minLDWord = name;
                        }
                    }

                    minSumLevenshteinDistance += minLevenshteinDistance;
                    minLDWordsLength += minLDWord.Length;
                }

                if (minSumLevenshteinDistance < minLDWordsLength * 0.5)
                {
                    result.Add(product);
                }
            }
        }

        private static void FullTextSearch(string text, List<Product> items, List<Product> result)
        {
            foreach (var product in items)
            {
                var minLDWordsLength = 0;
                var minSumLevenshteinDistance = 0;
                foreach (var text_word in text.Split(' ', ',', '.', '/', '\\', '!', '\'', '"', ':', ';', '[', ']', '@',
                    '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '~', '|'))
                {
                    var minLevenshteinDistance = int.MaxValue;
                    var minLDWord = "";
                    foreach (var name in product.Name.Split(' ', ',', '.', '/', '\\', '!', '\'', '"', ':', ';', '[',
                        ']', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '~', '|'))
                    {
                        var simplifyText = SimplifyText(text_word);
                        var simplifyName = SimplifyText(name);
                        var levenshteinDistance = LevenshteinDistance(simplifyText, simplifyName);
                        if (levenshteinDistance < minLevenshteinDistance)
                        {
                            minLevenshteinDistance = levenshteinDistance;
                            minLDWord = name;
                        }
                    }

                    minSumLevenshteinDistance += minLevenshteinDistance;
                    minLDWordsLength += minLDWord.Length;
                }

                if (minSumLevenshteinDistance < minLDWordsLength * 0.5)
                {
                    result.Add(product);
                }
            }
        }

//        [HttpPost]
//        public IActionResult Search(string text)
//        {
//            var _efProductRepository = new EfProductRepository();
//            var a = _efProductRepository.Products as IQueryable<Product>;
//            var b = a.FullTextSearchQuery(text).ToList();
//            return View("ShopGrid", b);
//        }

        private static int LevenshteinDistance(string first, string second)
        {
            var opt = new int[first.Length + 1, second.Length + 1];
            opt[0,0] = 0;
            for (var i = 1; i <= first.Length; ++i) opt[i, 0] = opt[i - 1, 0] + 1;
            for (var i = 1; i <= second.Length; ++i) opt[0, i] = opt[0, i - 1] + 1;
            for (var i = 1; i <= first.Length; ++i)
            for (var j = 1; j <= second.Length; ++j)
            {
                if (first[i - 1] == second[j - 1]) 
                    opt[i, j] = opt[i - 1, j - 1];
                else
                    opt[i, j] = Math.Min(Math.Min(1 + opt[i - 1, j],
                            1 + opt[i - 1, j - 1]),
                        1 + opt[i, j - 1]);
            }
            return opt[first.Length , second.Length];
        }

        private static string SimplifyText(string text)
        {
            var builder = new StringBuilder();
            var lastChar = ' ';
            foreach (var _char in text)
            {
                if(_char == lastChar) continue;
                builder.Append(_char);
                lastChar = _char;
            }

            return builder.ToString();
        }
    }

    public class CharValueStr
    {
        public bool check { get; set; }
        public string charValueStr { get; set; }
        public string Characteristic { get; set; }  // TODO: dfsdf
    }
    
    public class CharValueSNum
    {
        public bool check { get; set; }
        public decimal charValueNum { get; set; }
        public string Characteristic { get; set; }
    }
}