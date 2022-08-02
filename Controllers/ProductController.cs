using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Visionmore.Data;

namespace Visionmore.Controllers {
    [Authorize]
    public class ProductController : Controller {
        private readonly DataContext _db;
        public readonly IWebHostEnvironment _he;

        public ProductController(DataContext db, IWebHostEnvironment he) {
            _db = db;
            _he = he;
        }

        public IActionResult Index() => View(_db.Products.ToList());
        
        public IActionResult IndexWhere(string Category) => View(_db.Products.Where(p => p.Category == Category).ToList());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexWhere(string Category, string name) {
            if (name == null || name == "")
                return View(_db.Products.Where(p => p.Category == Category).ToList());

            return View(_db.Products.Where(p => p.Category == Category && p.Name.Contains(name)).ToList());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile image) {
            if (!ModelState.IsValid)
                return View(product);

            if (IfExiste(product)) {
                ViewBag.message = "This product is already exist";
                return View(product);
            }

            if (image != null) {
                var name = Guid.NewGuid().ToString() + image.FileName;
                var path = Path.Combine(_he.WebRootPath + "/images", Path.GetFileName(name));
                await image.CopyToAsync(new FileStream(path, FileMode.Create));
                product.Image = name;
            } else {
                product.Image = "no-image.png";
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IfExiste(Product product) {
            var searchProductType = _db.Products.FirstOrDefault(p => p.Name == product.Name && p.Id != product.Id);
            if (searchProductType != null) {
                return true;
            }
            return false;
        }
    }
}