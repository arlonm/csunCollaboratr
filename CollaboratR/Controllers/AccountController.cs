using CollaboratR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CollaboratR.Repositories;
using System.Web.Security;
using Newtonsoft.Json;
using System.Net; //new
using System.Net.Mail; //new
using CollaboratR.ViewModels;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using CollaboratR;

namespace CollaboratR.Controllers
{
    public class AccountController : Controller
    {
        //These are currently unused variables
        private int saltLength;
        private int hashStrength;
        private int saltWorkFactor;
      
        /// <summary>
        /// Global AccountRepository object for access to the database.
        /// </summary>
        protected AccountRepository globalAccountRep { get; set; }

        public AccountController()
        {
            hashStrength = 10;
            saltWorkFactor = 5;
            globalAccountRep = new AccountRepository(System.Configuration.ConfigurationManager.ConnectionStrings["defaultConnection"].ToString());
        }
        
        // GET: /Account/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //Sanitize all input.
                model.LoginName = model.LoginName.Sanitize(Utility.EmailWhiteList);
                model.Password = model.Password.Sanitize(Utility.PasswordWhiteList);
                
                //Gather the salt. this will be the ONLY instance of us hitting the
                //Database more than once in one request
                string salt = globalAccountRep.GetSaltByLoginName(model.LoginName);
                model.Password = hashString(model.Password + salt);
                
                //
                AccountModel acct = globalAccountRep.Login(model);
                
                if (acct.Success)
                {
                    string userData = string.Join("|", "");
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,                                     // ticket version
                        acct.AccountId + "," + acct.Username,  // authenticated username
                        DateTime.Now,                          // issueDate
                        DateTime.Now.AddDays(180),              // expiryDate
                        true,                                  // true to persist across browser sessions
                        userData,                              // can be used to store additional user data
                        FormsAuthentication.FormsCookiePath);  // the path for the cookie

                    // Encrypt the ticket using the machine key
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    
                    // Add the cookie to the request to save it
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                }
                return Json(new AccountViewModel(acct));
            }
            model.Success = false;
            return Json(model);
        }

        //--So it looks like you need this to render the page??
        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegistrationViewModel());
        }

        [HttpPost]
        public ActionResult Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                model.Registration.Username = model.Registration.Username.Sanitize(Utility.UserInfoWhiteList);
                model.Registration.Email = model.Registration.Email.Sanitize(Utility.EmailWhiteList);
                model.Registration.Password = model.Registration.Password.Sanitize(Utility.PasswordWhiteList);
                model.Registration.ConfirmPassword = model.Registration.ConfirmPassword.Sanitize(Utility.PasswordWhiteList);

                model.Registration.Salt = BCrypt.Net.BCrypt.GenerateSalt(saltWorkFactor);

                //Hash the password
                model.Registration.Password = hashString(model.Registration.Password + model.Registration.Salt);

                AccountModel m = globalAccountRep.Register(model.Registration);

                //--This does not return a success which is why it goes through the else--
                
                if (m.Success)
                {
                    Login(new LoginModel { Password = model.Registration.Password, LoginName = m.Email });
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    //input validation not successful
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Do i need to add a model for this? 


        
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();//this might not be right since we are using cookies?
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");//this might not be right since we are using cookies?
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index","Home");
        }

        private string hashString(String input)
        {
            SHA512 Sha = SHA512.Create();
            byte[] hashedBytes  = Sha.ComputeHash(Encoding.ASCII.GetBytes(input));
            return Convert.ToBase64String(hashedBytes);
        }
	}
}