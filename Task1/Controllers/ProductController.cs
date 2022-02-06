using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task1.DAL;
using Task1.Models;

namespace Task1.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInResult;
        private readonly AppDbContext _context;

        public ProductController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInResult, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInResult = signInResult;
        }

      
        public IActionResult Index(int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }


            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 5);

            List<Product> products = _context.Products.Skip((page - 1) * 5).Take(5).ToList();
            return View(products);
        }
        public async Task<IActionResult> RoleTester()
        {

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!user.IsAdmin)
            {
                return Json(new { status = 400 });
            }

            return Json(new { status = 200 });
        }

        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }

            product.DateTime = DateTime.Now;
            product.Earning = product.Count * (product.Price - product.ProductionCost);

            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Product exist = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (exist == null)
            {
                return NotFound();
            }
            exist.Name = product.Name;
            exist.Price = product.Price;
            exist.ProductionCost = product.ProductionCost;
            exist.Count = product.Count;
            exist.Earning = product.Count * (product.Price - product.ProductionCost);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user.IsAdmin == false)
            {
                return Json(new { status = 400 });
            }


            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
