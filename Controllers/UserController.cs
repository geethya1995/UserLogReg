using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestProj.Models;
using System.Net.Mail;
using System.Net;

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
            bool status = false;
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

                #region 4. Password Hashing
                user.Password = Encryptor.Hash(user.Password);

                // To avoid confirm password does not match issue (DB context validate again on set changes)
                user.ConfirmPassword = Encryptor.Hash(user.ConfirmPassword);
                #endregion

                // 1st time user
                user.IsEmailVerified = false;

                #region 5. Save data to DB
                using(MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    /* If data write is successful,
                     * send E-mail to user for E-mail verification
                     */

                    // 6. Send E-mail to user
                    SendVerificationEmail(user.Email, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id: " + user.Email;
                    status = true;
                }
                #endregion


            }
            else
            {
                message = "Invalid Request";
            }

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

        [NonAction]
        public void SendVerificationEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var senderMail = new MailAddress("geethya1995@gmail.com", "User Registration");
            var senderPassword = "GeethyA@1995";
            var receiverMail = new MailAddress(emailID);

            string subject = "Your account is successfully created!";
            string body = "<br/><br/> We are excited to tell you that your user account is" +
                " successfully created. Please click on the below link to verify your account." +
                " <br/><br/><a href='" + link + "'>" + link + "</a>";

            // Configure SMTP (Simple Mail Transfer Protocol) client
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderMail.Address, senderPassword)
            };

            using (var message = new MailMessage(senderMail, receiverMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtpClient.Send(message);

        }

    }
}