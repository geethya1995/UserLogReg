﻿using System;
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

            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        // Verify Account (By clicking the link in mail)
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line is added to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = status;
            return View();
        }

        // Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();  
        }

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
            var senderPassword = "fifthharmony";
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
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderMail.Address, senderPassword),
                EnableSsl = true,
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