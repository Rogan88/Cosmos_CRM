using BarcodeTrackerWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarcodeTrackerWEB.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(UserAccount user)
        {
            using (MainContext db = new MainContext())
            {
                

              var usr = db.userAccount.Where(u => user.Username == user.Username && u.Password == user.Password).FirstOrDefault();
                if (HttpContext.Session["UserID"] != null )
                {
                    
                        Session["UserID"] = usr.UserId.ToString();
                        Session["Username"] = usr.Username.ToString();
                        return RedirectToAction("LoggedIn", "Account");
                    
                    
                }
                else
                {
                    ModelState.AddModelError("", "please login first");
                }

            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["id1"] = null;
            Session["id2"] = null;
            Session["id3"] = null;
            Session["id4"] = null;
            Session["Region"] = null;
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.AddHeader("Cache-control", "no-store, must-revalidate, private,no-cache");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            Response.AppendToLog("window.location.reload();");

            return RedirectToAction("Login", "Account");
        }
    }
}