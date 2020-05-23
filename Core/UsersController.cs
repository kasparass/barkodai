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

        [ActionName("ChangeUser")]
        public async Task<IActionResult> changeTestUser(int id)
        {
            Models.User user = await Models.User.changeTestUser(id);
            return RedirectToAction("Index", "Home", new { message = "Changed test user to: " + user.id + "-" + user.first_name });
        }
    }
}