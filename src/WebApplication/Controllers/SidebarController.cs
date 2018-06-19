using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class SidebarController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
        public IActionResult CPU_Memory()
        {
            return View("~/Pages/Sidebar/CPU_Memory.cshtml");
        }

        public IActionResult HTTP_Requests()
        {
            return View();
        }

        public IActionResult Exceptions()
        {
            return View();
        }
        public IActionResult Contentions()
        {
            return View();
        }
        public IActionResult Garbage_Collection()
        {
            return View();
        }
        public IActionResult JIT()
        {
            return View();
        }
    }
}