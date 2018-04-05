using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BarcodeTrackerWEB.Models;
using BarcodeTrackerWEB.ViewModels;
using System.Net;

namespace BarcodeTrackerWEB.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            var db = new MainContext();
            
            var model = (from c in db.Customers 
                         select new CustomerVM()
                         {
                             customerId = c.CustomerId,
                             customerName = c.Name
                         });

            return View(model.ToList());
        }
        //View All Customers from customer model
        public ActionResult DisplayAllCustomers()
        {
            using(MainContext db = new MainContext())
            {
                List<Customer> allCustomers = new List<Customer>();
                allCustomers = db.Customers.ToList();
                return View(allCustomers);
            }
            
        }
        // GET: Customer/Details/5
        public ActionResult CustomerProfile(int id)
        {
            
            using (MainContext db = new MainContext())
            {

                CustomerProfileVM selectedCustomer = new CustomerProfileVM();
                //search for the customer where customerId equals id
                var customer = db.Customers.Find(id);

                //instantiate a new viewmodel for Customer profile page

                //Bind the csutomer attribute values to the ViewModel
                selectedCustomer.CustomerVM = customer;

                var recentOrders =
                   (
                       from ro in db.Orders

                       where (ro.CustomerId == id)
                       select ro
                   ).ToList();

                var orderCount = 0;

                selectedCustomer.CustomerProfileVMId = id;
                foreach (var item in recentOrders)
                {
                    
                    selectedCustomer.CustomerVM.CustomerId = item.CustomerId;
                    var order = new ViewModels.CustomerOrder();

                    order.OrderId = item.OrderId;
                    order.CustomerId = item.CustomerId;
                    order.OrderTotal = item.TotalAmount;

                    //add to orderCount
                    orderCount += 1;
                    //Populate recent activity model.
                    selectedCustomer.recentActivity.Add(order);
                }
                //Determine Customer Rating
                int ocSwitch = 0;
                if(orderCount == 0)
                {
                    ocSwitch = 0;
                } else if(orderCount > 0 && orderCount < 2)
                {
                    ocSwitch = 1;
                } else if (orderCount > 2 && orderCount < 3)
                {
                    ocSwitch = 2;
                }
                else if (orderCount > 3 && orderCount < 4)
                {
                    ocSwitch = 3;
                }
                else if (orderCount > 4 && orderCount < 5)
                {
                    ocSwitch = 4;
                }
                else if (orderCount > 5)
                {
                    ocSwitch = 5;
                }

                switch (ocSwitch)
                {
                    case 1: selectedCustomer.rating = 1;
                        break;
                    case 2:
                        selectedCustomer.rating = 2;
                        break;
                    case 3:
                        selectedCustomer.rating = 3;
                        break;
                    case 4:
                        selectedCustomer.rating = 4;
                        break;
                    case 5:
                        selectedCustomer.rating = 5;
                        break;
                    default:
                        break;
                }



                return View(selectedCustomer);
            }
            
        }

        // GET: Customer/Create
        public ActionResult CreateCustomer()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert logic here
                using (MainContext db = new MainContext())
                {
                    //saves new customer to the database
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                ModelState.Clear();
                
            }
            return RedirectToAction("DisplayAllCustomers");

        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {

            using (MainContext db = new MainContext())
            {
                //search for the customer where customerId equals id
                var customer = db.Customers.Find(id);
                
                return View(customer);
            }
        }

        // POST: Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {

            return View();
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            using (MainContext db = new MainContext())
            {
                var customer = db.Customers.Find(id);
                return View(customer);
            }
            
        }


        // POST: Customer/Delete/5

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            using(MainContext db = new MainContext())
            {

                try
                {
                    
            var oid = (from d in db.OrderDetails
                       where d.OrderDetailsId == id
                       select d.OrderId).SingleOrDefault();


            var row = db.Customers.Find(id);


            db.Customers.Remove(row);
            db.SaveChanges();
                }
                catch (Exception)
                {
                    string message = "Cannot Delete Customer with orders that are linked.";
                    throw new Exception(message);
                }

            }

            return RedirectToAction("DisplayAllCustomers");
        }
        
        public ActionResult getRecentActivity(int cid)
        {
       
            using (MainContext db = new MainContext())
            {
                
                var recentOrders =
                   (
                       from ro in db.Orders

                       where (ro.CustomerId == cid)
                       select ro
                   ).ToList();
                

                var cpObject = new CustomerProfileVM();
                
                foreach (var item in recentOrders)
                {
                    var order = new ViewModels.CustomerOrder();

                    order.OrderId = item.OrderId;
                    order.CustomerId = item.CustomerId;


                    cpObject.recentActivity.Add(order);
                    cpObject.CustomerProfileVMId = cid;
                    db.SaveChanges();

                }

                return Redirect("/CustomerProfile/" + cid);

                //var data = db.Orders.Where(e => e.CustomerId == cid);
                //return Json(data, JsonRequestBehavior.AllowGet);

            }

            
        }

        
        public ActionResult UploadLogo(int id)
        {
            using (MainContext db = new MainContext())
            {
                
                //search for the customer where customerId equals id
                var customer = db.Customers.Find(id);
                
                return View(customer);
            }
         
        }


        [HttpPost]
        public ActionResult UploadLogo(HttpPostedFileBase image, int id)
        {
            
                // If an image exists
                if (image != null)
                {
                    // Read the image data into a byte array
                    var imageContent = new byte[image.ContentLength];
                    image.InputStream.Read(imageContent, 0, imageContent.Length);

                    // At this point, you have the byte[] content for the image, so store it
                    using (var db = new MainContext())
                    {
       

                    var customer = db.Customers.Find(id);
                    customer.Logo = imageContent;
                    // Save it to your context
             
                    db.SaveChanges();
                    }
                }

                
                return RedirectToAction("CustomerProfile/" + id);
            
            
        }

        public ActionResult GeoMapping(int id)
        {
            MainContext db = new MainContext();

            var customer = db.Customers.Find(id);

            var location = customer.Address;

            return View(location);
        }
    }
}
