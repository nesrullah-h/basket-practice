using fiorello.DAL;
using fiorello.Models;
using fiorello.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fiorello.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            int totalCount = 0;

            if (Request.Cookies["basket"] != null)
            {
                List<BasketProduct> products = JsonConvert.DeserializeObject<List<BasketProduct>>(Request.Cookies["basket"]);


                foreach (var item in products)
                {
                    totalCount += item.Count;

                }

            }
            else
            {
                return Content("basket bosdur");
            }
            ViewBag.BasketLength = totalCount;

            Bio bio = _context.bios.FirstOrDefault();
            return View(await Task.FromResult(bio));
        }
    }
}