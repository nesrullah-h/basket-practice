using fiorello.DAL;
using fiorello.Extentions;
using fiorello.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fiorello.Areas.AdminF.Controllers
{
        [Area("AdminF")]
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly AppDbContext _context;
        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Blog> blogs = _context.blogs.ToList();
            return View(blogs);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog blog)
        {
            if (!ModelState.IsValid) return View();
            Blog SameName = _context.blogs.FirstOrDefault(b => b.Title.ToLower().Trim().Contains(blog.Title.ToLower().Trim()));
            if (blog.ImageFile != null)
            {
                if (!blog.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekil formati duzgun deyil");
                    return View();
                }
                if (!blog.ImageFile.IsSizeOk(10))
                {
                    ModelState.AddModelError("ImageFile", "Sekil 10 mb yuksek olmaz");
                    return View();
                }
                blog.Image = blog.ImageFile.SaveImg(_env.WebRootPath, "blogimg");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Sekil daxil edin");
                return View();
            }

            _context.blogs.Add(blog);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            return View(dbBlog);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            _context.blogs.Remove(dbBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            return View(dbBlog);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Blog dbBlog = await _context.blogs.FindAsync(id);

            if (dbBlog == null) return NotFound();
            dbBlog.Title = dbBlog.Title;
            dbBlog.Description = dbBlog.Description;
            dbBlog.Image = dbBlog.Image;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}


