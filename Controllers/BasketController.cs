using fiorello.DAL;
using fiorello.Models;
using fiorello.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fiorello.Controllers
{
    public class BasketController : Controller
    {
        private AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return NotFound();
            Product dbProduct = await _context.products.FindAsync(id);

            if (dbProduct == null) return NotFound();
            List<BasketProduct> products;

            string existBasket = Request.Cookies["basket"];

            if (existBasket == null)
            {
                products = new List<BasketProduct>();

            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketProduct>>(Request.Cookies["basket"]);

            }
            BasketProduct existBasketproduct = products.FirstOrDefault(p => p.Id == dbProduct.Id);
            if (existBasketproduct == null)
            {
                BasketProduct basketProduct = new BasketProduct();

                basketProduct.Id = dbProduct.Id;
                basketProduct.Name = dbProduct.Name;
                basketProduct.Count = 1;

                products.Add(basketProduct);
            }
            else
            {
                if (dbProduct.Count <= existBasketproduct.Count)
                {
                    TempData["Fail"] = "not enough count";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    existBasketproduct.Count++;
                }
            }



            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromMinutes(30) });

            return RedirectToAction("Index", "Home");

        }
        public IActionResult Basket()
        {
            List<BasketProduct> products = JsonConvert.DeserializeObject<List<BasketProduct>>(Request.Cookies["basket"]);
            List<BasketProduct> updatesproducts = new List<BasketProduct>();
            foreach (var item in products)
            {
                Product dbproduct = _context.products.FirstOrDefault(p => p.Id == item.Id);
                BasketProduct basket = new BasketProduct()
                {
                    Id = dbproduct.Id,
                    Price = dbproduct.Price,
                    Name = dbproduct.Name,
                    ImageUrl = dbproduct.ImageUrl,
                    Count = item.Count
                };

                updatesproducts.Add(basket);
            }
            return View(updatesproducts);
        }
    }
}