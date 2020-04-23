using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Barkodai.Core
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}