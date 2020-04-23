using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Barkodai.Models;
using Barkodai.Repositories;
using Barkodai.Core;

namespace Barkodai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItemsAPI itemsAPI;

        public HomeController(ILogger<HomeController> logger, IItemsAPI itemsAPI)
        {
            _logger = logger;
            this.itemsAPI = itemsAPI;
        }

        public async Task<IActionResult> Index()
        {
            return View("~/Core/UserMainView.cshtml", await itemsAPI.getItems());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
