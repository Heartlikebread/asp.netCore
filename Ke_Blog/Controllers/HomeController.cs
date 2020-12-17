using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ke_Blog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ke_Blog.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Post()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Edit()
        {
            return View(new Post());
        }
        [HttpPost]
        public IActionResult Edit(Post post)
        {
            return RedirectToAction("Index");
        }
    }
}