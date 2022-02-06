using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task1.DAL;
using Task1.Models;
using Task1.ViewModels;

namespace Task1.Controllers
{

    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInResult;
        private readonly AppDbContext _context;


        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInResult, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInResult = signInResult;
        }


        public IActionResult Register()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();

            AppUser exist = await _userManager.FindByNameAsync(register.Username);
            if (exist != null)
            {
                ModelState.AddModelError("", "This username already taken");
                return View();
            }

            AppUser user = new AppUser
            {
                UserName = register.Username,
                Email = register.Email,
                FullName = register.Fullname
            };

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Moderator");
            await _signInResult.PasswordSignInAsync(user, register.Password, true, true);


            return RedirectToAction("Index", "Product");

        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(login.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }


            Microsoft.AspNetCore.Identity.SignInResult result = await _signInResult.PasswordSignInAsync(user, login.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            return RedirectToAction("Index", "Product");

        }

        public async Task<IActionResult> Logout()
        {
            await _signInResult.SignOutAsync();

            //return Content(User.Identity.Name);
            return RedirectToAction("Login", "Account");

        }
        public IActionResult Testuser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { status = 400 });
            }

            return Json(new { status = 200 });
        }



        [Authorize(Roles = "Admin")]
        public IActionResult UserList()
        {

            List<AppUser> users = _userManager.Users.ToList();

            return View(users);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AdminStatus(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (!ModelState.IsValid) return RedirectToAction("UserList", "Account");
            if (user == null)
            {
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(user, "Admin") == true)
            {
                return Json(new { status = 400 });
            }


            if (!user.IsAdmin == true)
            {

                user.IsAdmin = true;
            }
            else
            {

                user.IsAdmin = false;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction("UserList", "Account");
        }


        public IActionResult TestAdmin()
        {
            if (!User.IsInRole("Admin"))
            {
                return Json(new { status = 400 });

            }

            return Json(new { status = 200 });
        }


        // run this method for creating admin!

        //public async Task CreateAdmin()
        //{
        //    AppUser admin = new AppUser
        //    {
        //        UserName = "admin",
        //        FullName = "Super Admin",
        //        EmailConfirmed = true,
        //        IsAdmin = true
        //    };

        //    await _userManager.CreateAsync(admin, "admin123");
        //    await _userManager.AddToRoleAsync(admin, "Admin");

        //}
    }
}