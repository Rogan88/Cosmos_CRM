using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarcodeTrackerWEB.Models;
using BarcodeTrackerWEB.ViewModels;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace BarcodeTrackerWEB.Controllers
{
    public class BIDashboardController : Controller
    {

        private MainContext db = new MainContext();
        // GET: BIDashboard
        public ActionResult Index()
        {
            //initialize db connection to maincontext
            using (MainContext db = new MainContext())
            {
                //Linq query to join customer and order table
                var usersWithCount = (from c in db.Customers
                                      join o in db.Orders on c.CustomerId equals o.CustomerId
                                      into result
                                      from sub in result.DefaultIfEmpty()
                                      select new ViewModels.CustomerOrder
                                      {
                                          OrderId = sub != null ? sub.OrderId : (int?)null,
                                          CustomerId = c.CustomerId,
                                      }
                                        ) // Here we have the left join result.
                                        .GroupBy(g => g.CustomerId, (k, orders) => new
                                        {
                                            CustomerId = k,
                                            //do count on number of orders per customer
                                            OrderCount = orders.Count(t => t.OrderId != null),
                                            //order the query result from highest number of orders to lowest.
                                        }).OrderByDescending(t => t.OrderCount).ToList();
                //select the top 3 customers from query results
                var top3Customers = usersWithCount.Take(3);

                //initialize new BIDashboard ViewModel
                BIDashboardVM data = new BIDashboardVM();
                //populate the view models with customer data from top 3 performers
                foreach (var item in top3Customers)
                {
                    int cid = item.CustomerId;
                    var customer = db.Customers.Find(cid);
                    data.customerList.Add(customer);
                    db.SaveChanges();
                }

                var allOrders = (from o in db.Orders
                                 select new OrderValue
                                 {
                                     OrderId = o.OrderId,
                                     Amount = o.TotalAmount,
                                     customerId = o.CustomerId

                                 }
                                     ).OrderByDescending(t => t.Amount).ToList();



                var top3OrderValues = allOrders.Take(3);

                //Get top 3 values from order table save to orderLists
                foreach (var item in top3OrderValues)
                {
                    decimal value = item.Amount;
                    int cid = item.customerId;
                    int? oid = item.OrderId;
                    var order = db.Orders.Find(oid);
                    data.ordersLists.Add(order);
                    db.SaveChanges();
                }

                foreach (var item in allOrders)
                {
                    int? oid = item.OrderId;
                    var order = db.Orders.Find(oid);
                    data.allOrderList.Add(order);
                    db.SaveChanges();
                }
                //Load BIDashboardVM with all orders data.


                //OrderbyDecending(o => o.OrderId).ToList();


                //pass ViewModel data to the view
                return View(data);
            }

        }

        [HttpPost]
        public ActionResult getOrderValues()
        {
            var data = db.Orders.ToList();
            return Json(new
            {

                aData = data.Select(x => new object[] { x.TotalAmount })

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrders()
        {

            //var dbResult = from o in db.Orders
            //               select new
            //               {

            //                   o.Customer.Name,
            //                   o.OrderId
            //               };
            var dbResult = new[] { new Models.Order() { OrderId = 20, TotalAmount = 2008 }, new Models.Order() { OrderId = 30, TotalAmount = 777 } };


            return Json(dbResult, JsonRequestBehavior.AllowGet);


        }

        //[HttpPost]
        public ActionResult test()
        {
            return View();
        }
        public ActionResult ShowLargestOrder()
        {

            using (MainContext db = new MainContext())
            {

                BIDashboardVM data = new BIDashboardVM();


                return View(data);
            }


        }

        public ActionResult Details(int orderId)
        {

            return RedirectToAction("Details", "Orders", new { id = orderId });
        }
        public ActionResult MyChart()
        {
            return View();
        }

        public ActionResult MainDash()
        {
            //initialize db connection to maincontext
            using (MainContext db = new MainContext())
            {
                //Linq query to join customer and order table
                var usersWithCount = (from c in db.Customers
                                      join o in db.Orders on c.CustomerId equals o.CustomerId
                                      into result
                                      from sub in result.DefaultIfEmpty()
                                      select new ViewModels.CustomerOrder
                                      {
                                          OrderId = sub != null ? sub.OrderId : (int?)null,
                                          CustomerId = c.CustomerId,
                                      }
                                        ) // Here we have the left join result.
                                        .GroupBy(g => g.CustomerId, (k, orders) => new
                                        {
                                            CustomerId = k,
                                            //do count on number of orders per customer
                                            OrderCount = orders.Count(t => t.OrderId != null),
                                            //order the query result from highest number of orders to lowest.
                                        }).OrderByDescending(t => t.OrderCount).ToList();
                //select the top 3 customers from query results
                var top3Customers = usersWithCount.Take(3);

                //initialize new BIDashboard ViewModel
                BIDashboardVM data = new BIDashboardVM();
                //populate the view models with customer data from top 3 performers
                foreach (var item in top3Customers)
                {
                    int cid = item.CustomerId;
                    var customer = db.Customers.Find(cid);
                    data.customerList.Add(customer);
                    db.SaveChanges();
                }

                var allOrders = (from o in db.Orders
                                 select new OrderValue
                                 {
                                     OrderId = o.OrderId,
                                     Amount = o.TotalAmount,
                                     customerId = o.CustomerId

                                 }
                                     ).OrderByDescending(t => t.Amount).ToList();



                var top3OrderValues = allOrders.Take(3);

                //Get top 3 values from order table save to orderLists
                foreach (var item in top3OrderValues)
                {
                    decimal value = item.Amount;
                    int cid = item.customerId;
                    int? oid = item.OrderId;
                    var order = db.Orders.Find(oid);
                    data.ordersLists.Add(order);
                    db.SaveChanges();
                }

                foreach (var item in allOrders)
                {
                    int? oid = item.OrderId;
                    var order = db.Orders.Find(oid);
                    data.allOrderList.Add(order);
                    db.SaveChanges();
                }
                //Load BIDashboardVM with all orders data.

                //get total number of clients/customers
                data.numberOfCustomers = db.Customers.Count();

                //OrderbyDecending(o => o.OrderId).ToList();

                ViewBag.Layout = "~/Views/Shared/_BIDashLayout.cshtml";
                //pass ViewModel data to the view
                return View(data);
            }
        }


        
        public ActionResult Dashboard()
        {

            using (MainContext _context = new MainContext())
            {
                ViewBag.CountCustomers = _context.Customers.Count();
                ViewBag.CountOrders = _context.Orders.Count();
                ViewBag.CountProducts = _context.Products.Count();
            }

            return View();
        }

        public ActionResult BestSellingProduct()
        {
            List<TopProductViewModel> topFiveProducts = null;
            using (MainContext db = new MainContext())
            {
                var OrderDetailsByProduct = (from p in db.OrderDetails
                                             group p by p.ProductId into g
                                             orderby g.Count() descending
                                             select new
                                             {
                                                 ProductId = g.Key,
                                                 Count = g.Count(),

                                             }).Take(5);



                topFiveProducts = (from c in db.Products
                                   join o in OrderDetailsByProduct
                                   on c.ProductId equals o.ProductId
                                   select new TopProductViewModel
                                   {

                                       ProductName = c.Description,
                                       ProductId = c.ProductId,
                                       CountOrder = o.Count,
                                       TotalSold = db.OrderDetails.GroupBy(x => x.OrderId)
   .Select(n => n.Sum(m => m.UnitPrice * m.Quantity)).FirstOrDefault().ToString()


                                   }).ToList();

                foreach (var product in topFiveProducts)
                {
                    var TotalVal = db.OrderDetails.Where(i => i.ProductId == product.ProductId).Sum(s => s.UnitPrice * s.Quantity).ToString("N2");


                    product.TotalSold = TotalVal.ToString();
                    db.SaveChanges();
                }

            }

            return PartialView("~/Views/BIDashboard/TopProducts.cshtml", topFiveProducts);
        }

        public ActionResult TopCustomers()
        {
            List<TopCustomerViewModel> topFiveCustomers = null;
            using (MainContext db = new MainContext())
            {
                var OrderByCustomer = (from o in db.Orders
                                       group o by o.Customer.CustomerId into g
                                       orderby g.Sum(s => s.TotalAmount) descending
                                       select new
                                       {
                                           CustomerID = g.Key,
                                           Count = g.Count()
                                       }).Take(5);



                topFiveCustomers = (from c in db.Customers
                                    join o in OrderByCustomer
                                    on c.CustomerId equals o.CustomerID
                                    select new TopCustomerViewModel
                                    {
                                        Logo = c.Logo,
                                        CustomerName = c.Name,
                                        CustomerId = c.CustomerId,
                                        CustomerProvince = c.Province.ToString(),
                                        CountOrder = o.Count,
                                        TotalValue = db.Orders.GroupBy(x => x.CustomerId)
    .Select(n => n.Sum(m => m.TotalAmount)).FirstOrDefault().ToString()


                                    }).ToList();

                foreach (var customer in topFiveCustomers)
                {
                    var TotalVal = db.Orders.Where(i => i.CustomerId == customer.CustomerId).Sum(s => s.TotalAmount).ToString("N2");


                    customer.TotalValue = TotalVal.ToString();
                    db.SaveChanges();
                }

            }

            return PartialView("~/Views/BIDashboard/TopCustomers.cshtml", topFiveCustomers);
        }

        
        public ActionResult GetOrdersInProvinceCode()
        {
            MainContext db = new MainContext();

            var geoData = (from o in db.Orders
                           group o by o.Customer.Province into g

                           select new
                           {
                               Province = g.Key,
                               CountOrders = g.Count() 
                               
                           }).ToList();

           
                
            
                    
            //['ZA-EC', null],
            //['ZA-FS', 1],
            //['ZA-GT', 12],
            //['ZA-NL', null],
            //['ZA-LP', null],
            //['ZA-MP', null],
            //['ZA-NC', null],
            //['ZA-NW', null],
            //['ZA-WC', 1]
            

            return Json(new { result = geoData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerOrders()
        {
            MainContext _context = new MainContext();

            var lowestOrders = (from o in _context.Orders
                                   group o by o.Customer.Name into g
                                   orderby g.Count() ascending
                                   select new
                                   {
                                       Name = g.Key,
                                       CountOrders = g.Count()
                                   }).ToList();

            return Json(new { result = lowestOrders }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OrdersByProvince()
        {
            MainContext _context = new MainContext();

            var ordersByCountry = (from o in _context.Orders
                                   group o by o.Customer.Province into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       Country = g.Key,
                                       CountOrders = g.Count()
                                   }).ToList();

            return Json(new { result = ordersByCountry }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OrdersByDate()
        {
            MainContext db = new MainContext();



            var ordersByDate = (from o in db.Orders
                                   group o by o.OrderDate into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       Month = g.Key,
                                       
                                       CountOrders = g.Count()
                                   }).ToList();
            
            

            return Json(new { result = ordersByDate }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetOrdersInProgress()
        {
            MainContext db = new MainContext();
            {
                var data = db.Orders.ToList();

                List<Order> ordersInProgress = new List<Order>();
                

                foreach (var item in data)
                {

                    if (item.Status == 3)
                    {
                        ordersInProgress.Add(item);
                    }
                    
                }
                


                return Json(new
                {

                    aaData = ordersInProgress.Select(x => new object[] { x.OrderId, x.Customer.Name, x.Customer.Province, "R " + x.TotalAmount.ToString("0.00"), x.SalesRep.FirstName + " " + x.SalesRep.LastName, "<a href='Orders/fillInOrderDetails?orderId="
                                  + x.OrderId + "'>Go To Order</a>"})

                }, JsonRequestBehavior.AllowGet);
                
            }
        }

        //public ActionResult CustomersByCountry()
        //{
        //    DashboardContext _context = new DashboardContext();

        //    var customerByCountry = (from c in _context.CustomerSet
        //                             group c by c.CustomerCountry into g
        //                             orderby g.Count() descending
        //                             select new
        //                             {
        //                                 Country = g.Key,
        //                                 CountCustomer = g.Count()
        //                             }).ToList();

        //    return Json(new { result = customerByCountry }, JsonRequestBehavior.AllowGet);
        //}


        //Populate Google Pie Chart 1
        public ActionResult OrdersByCustomer()
        {
            MainContext _context = new MainContext();
            var ordersByCustomer = (from o in _context.Orders
                                    group o by o.Customer.CustomerId into g
                                    select new
                                    {
                                        Name = from c in _context.Customers
                                               where c.CustomerId == g.Key
                                               select c.Name,

                                        CountOrders = g.Count()

                                    }).ToList();


            return Json(new { result = ordersByCustomer }, JsonRequestBehavior.AllowGet);
        }

        //Populate Google Pie Chart 2
        public ActionResult ProductByOrders()
        {
            MainContext db = new MainContext();
            var productByOrder = (from o in db.OrderDetails
                                  group o by o.ProductId into g
                                  select new
                                  {
                                      pName = from p in db.Products
                                              where p.ProductId == g.Key
                                              select p.Description,

                                      CountOrders = g.Count()

                                  }).ToList();


            return Json(new { result = productByOrder }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrdersBySalesRep()
        {
            MainContext db = new MainContext();
            var ordersByRep = (from o in db.Orders
                               group o by o.SaleRepId into g
                               select new
                               {
                                   srName = from s in db.SalesReps
                                            where s.SalesRepId == g.Key
                                            select
                                            
                                                s.FirstName,
                                                CountOrders = g.Count()
                                            

                               }).ToList();


            return Json(new { result = ordersByRep }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult ValueBySalesRep()
        {
            MainContext db = new MainContext();
            
            var totalSumByRep = (from o in db.Orders
                               group o by o.SaleRepId into g
                               select new
                               {
                                   srName = from s in db.SalesReps
                                            where s.SalesRepId == g.Key
                                            select
                                            
                                                s.FirstName,
                                                
                                                SumTotal = g.Sum(t=> t.TotalAmount)
                                            

                               }).ToList();
            

            return Json(new { result = totalSumByRep }, JsonRequestBehavior.AllowGet);
        }
        //public List<ProductOrCustomerViewModel> GetProductOrCustomer(string type)
        //{
        //    List<ProductOrCustomerViewModel> result = null;

        //    using (DashboardContext _context = new DataAccess.DashboardContext())
        //    {
        //        if (!string.IsNullOrEmpty(type))
        //        {
        //            if (type == "customers")
        //            {
        //                result = _context.CustomerSet.Select(c => new ProductOrCustomerViewModel
        //                {
        //                    Name = c.CustomerName,
        //                    Image = c.CustomerImage,
        //                    TypeOrCountry = c.CustomerCountry,
        //                    Type = "Customers"

        //                }).ToList();

        //            }
        //            else if (type == "products")
        //            {
        //                result = _context.ProductSet.Select(p => new ProductOrCustomerViewModel
        //                {
        //                    Name = p.ProductName,
        //                    Image = p.ProductImage,
        //                    TypeOrCountry = p.ProductType,
        //                    Type = p.ProductType

        //                }).ToList();

        //            }
        //        }

        //        return result;
        //    }

    }
}


