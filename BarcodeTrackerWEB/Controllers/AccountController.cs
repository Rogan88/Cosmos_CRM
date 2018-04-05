using BarcodeTrackerWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarcodeTrackerWEB.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            using (MainContext db = new MainContext())
            {
                List<UserAccount> users = new List<UserAccount>();
                    users = db.userAccount.ToList();
                return View(users);
            }
            
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Layout = "~/Views/Shared/_LoggedOut.cshtml";
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            
            if (ModelState.IsValid)
            {
            
                using (MainContext db = new MainContext())
                {
                    if(account.Department == "Admin")
                    {
                        ViewBag.CallJSFuncOnPageLoad = "checkDepartment();";
                    }
                    //saves new user account
                    db.userAccount.Add(account);
                    db.SaveChanges();

                }
                ModelState.Clear();
                
                return RedirectToAction("Login");
            }

            return View(account);
        }

        //Login Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Layout = "~/Views/Shared/_LoggedOut.cshtml";
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
            
            using (MainContext db = new MainContext())
            {
                var usr = db.userAccount.Where(u => user.Username == user.Username && u.Password == user.Password).FirstOrDefault();
                if (usr != null)
                {
                    Session["UserID"] = usr.UserId.ToString();
                    Session["Username"] = usr.Username.ToString();
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "Userame or Password Is wrong");
                    ModelState.Clear();
                }
                ViewBag.Layout = "~/Views/Shared/_LoggedOut.cshtml";
            }
            return View();

        }
        
        public ActionResult LoggedIn()
        {
            if (Session["UserId"] != null)
            {
                return View();
            } else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }


    }
}