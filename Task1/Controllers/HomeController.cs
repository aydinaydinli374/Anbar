using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task1.Models;

namespace Task1.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
            //return RedirectToAction("Login","Account",new {area = "Admin" });
            //return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

    }
}
