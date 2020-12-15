using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestProj.Models;

namespace TestProj.Controllers
{
    public class UserController : Controller
    {
        // Registration Action (Render the form)
        [HttpGet]
        public ActionResult Registration()
        {
            return View();  
        }

        // Registration POST Action (Submit data to DB from the form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified , ActivationCode")] User user)
        {
            // Submit and save the form data to User table (Logic)
            return View(User);
        }

        // Verify E-mail (Account validation)

        // Verify E-mail link

        // Login

        // Login POST

        // Logout

    }
}