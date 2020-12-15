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
            // variables to be used in ViewBag
            bool Status = false;
            string message = "";

            // Submit and save the form data to User table (Logic)
            // 1. Model Validation
            if (ModelState.IsValid)
            {

                #region 2. Duplicate E-mails (Email already exists?)
                if (IsEmailExist(user.Email))
                {
                    ModelState.AddModelError("DuplicateEmail", "Email already exists");
                    return View();
                }
                #endregion

                #region 3. Generate Activation Code
                user.ActivationCode = Guid.NewGuid();
                #endregion


            } else
            {
                message = "Invalid Request";
            }

            // 4. Password Hashing

            // 5. Save data to DB

            // 6. Send E-mail to user

            return View(User);
        }

        // Verify E-mail (Account validation)

        // Verify E-mail link

        // Login

        // Login POST

        // Logout

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var duplicateEmail = dc.Users.Where(a => a.Email == emailID).FirstOrDefault();
                if (duplicateEmail != null)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }


    }
}