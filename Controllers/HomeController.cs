using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Barkodai.Models;
using Barkodai.Repositories;

namespace Barkodai.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestRepository testRepository;

        public HomeController(ILogger<HomeController> logger, ITestRepository testRepository)
        {
            _logger = logger;
            this.testRepository = testRepository;
        }

        public IActionResult Index()
        {
            return View(testRepository.GetTests());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
