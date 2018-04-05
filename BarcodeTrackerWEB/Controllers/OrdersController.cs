using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarcodeTrackerWEB.Models;
using BarcodeTrackerWEB.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;

namespace BarcodeTrackerWEB.Controllers
{

    public class OrdersController : Controller
    {
        MainContext db = new MainContext();

        public ActionResult ViewAllOrders()
        {
            using (MainContext db = new MainContext())
            {
                return View(db.Orders.ToList());//
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }


        //public ActionResult OrderDetails(int? oid, int? cid, int? sid, int? pid)
        public ActionResult OrderDetails(int? oid, int? cid, int? sid)
        {
            using (MainContext db = new MainContext())
            {
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
                ViewBag.SaleRepId = new SelectList(db.SalesReps, "SalesRepId", "FirstName");
                OrdersVM selectedOrder = new OrdersVM();
                //search for the customer where customerId equals id
                var order = db.Orders.Find(oid);
                var customer = db.Customers.Find(cid);
                var salesrep = db.SalesReps.Find(sid);
                //var products = db.Products.ToList();



                selectedOrder.OrderVM.OrderId = order.OrderId;

                selectedOrder.CustomerVM.CustomerId = order.CustomerId;

                //selectedOrder.ProductVM.Description = products.Description;

                //selectedOrder.ProductVM.ProductId = products.ProductId;



                return View(selectedOrder);
            }
        }

        [HttpPost]
        public ActionResult AddItemToOrderDetail(OrderDetailVM od)
        {

            try
            {
                var validSequence = false;

                //Barcode Sequence Data
                var barcodePrefix = od.barcodePrefix;
                var startSequence = od.startSequence;
                //var endSequence = od.startSequence + od.Quantity;
                var barcodeSuffix = od.barcodeSuffix ?? "";

                int ss = Int32.Parse(startSequence);



                String.Format("sequenceLen");

                //Barcode Sequence Validation - Used to check existing barcodes sequences to prevent duplication errors ---


                var customerId = db.Orders.FirstOrDefault(c => c.OrderId == od.OrderId).CustomerId;


                List<Order> listOfOrders = new List<Order>();

                foreach (var order in db.Orders.Where(i => i.CustomerId == customerId))
                {
                    listOfOrders.Add(order);
                }

                foreach (var order in listOfOrders)
                {
                    var detail = db.OrderDetails.Find(order.OrderId);
                    int existingSequece = Int32.Parse(detail.endSequence);

                    if (ss > existingSequece)
                    {
                        validSequence = true;
                    }
                }

                //if (validSequence == true)
                //{


                    OrderDetail o = new OrderDetail();

                    var item = od.ProductId;
                    var description = (from p in db.Products where (p.ProductId == item) select p.Description).First();
                    var quantity = od.Quantity;
                    var oid = od.OrderId;
                    var orderDetailsId = oid;
                    var price = (from p in db.Products where (p.ProductId == item) select p.UnitPrice).First();

                    int es = ss + quantity;

                    List<OrderDetail> items = new List<OrderDetail>();

                    od.UnitPrice = price;

                    o.ProductId = item;
                    o.Description = description;
                    o.Quantity = quantity;
                    o.OrderId = oid;
                    o.OrderDetailsId = oid;
                    o.UnitPrice = price;
                    o.barcodePrefix = barcodePrefix;
                    o.barcodeSuffix = o.barcodeSuffix;
                    o.startSequence = startSequence;




                    var sequenceSize = new string('0', startSequence.Length);


                    o.endSequence = es.ToString(sequenceSize);

                    db.OrderDetails.Add(o);
                    db.SaveChanges();

                
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        // GET: Orders/Delete/5
        public ActionResult DeleteOrderItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var oid = (from d in db.OrderDetails
                       where d.OrderDetailsId == id
                       select d.OrderId).SingleOrDefault();


            var row = db.OrderDetails.Find(id);

            //if (row.orderItems.Where(i => i.OrderDetailsId) = id;)
            //{

            //}

            db.OrderDetails.Remove(row);
            db.SaveChanges();





            return RedirectToAction("fillInOrderDetails", new { orderId = oid });

        }



        [HttpGet]
        public JsonResult GetAllOrderItems(int orderId)
        {



            var lineItems =
            (
                from p in db.OrderDetails
                where (p.OrderId == orderId)
                select new
                {
                    p.OrderDetailsId,
                    p.ProductId,
                    p.Description,
                    p.startSequence,
                    p.endSequence,
                    p.barcodePrefix,
                    p.barcodeSuffix,
                    p.Quantity,
                    p.UnitPrice

                });


            return Json(lineItems, JsonRequestBehavior.AllowGet);

        }




        //Step 2: Order Details
        public ActionResult fillInOrderDetails(int orderId)
        {

            OrderDetailVM od = new OrderDetailVM();
            var order = db.Orders.Find(orderId);
            List<Product> products = new List<Product>();
            od.Order = order;
            od.OrderId = orderId;



            foreach (Product p in db.Products)
            {
                var prodDetails = p.Description + p.ProductId + p.UnitPrice;

                products.Add(p);
                //od.ProductList.Add(p.Description);
            }

            od.OrderDetailsId = orderId;
            od.ProductList = products;
            db.SaveChanges();

            return View(od);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult fillInOrderDetails([Bind(Include = "OrderId,ProductId,Quantity,UnitPrice,Description,barcodePrefix,startSequence,endSequence,barcodeSuffix")] OrderDetailVM o)
        {
            if (ModelState.IsValid)
            {
                OrderDetail od = new OrderDetail();
                var item = o.ProductId;
                var description = (from p in db.Products where (p.ProductId == o.ProductId) select p.Description).FirstOrDefault();
                var quantity = od.Quantity = o.Quantity;
                var oid = od.OrderId = o.OrderId;
                var orderDetailsId = oid = o.OrderDetailsId;
                var price = (from p in db.Products where (p.ProductId == o.ProductId) select p.UnitPrice).FirstOrDefault();


                //Barcode Sequence Data
                var barcodePrefix = od.barcodePrefix;
                var startSequence = od.startSequence;
                var endSequence = od.startSequence + od.Quantity;
                var barcodeSuffix = od.barcodeSuffix;

                List<OrderDetail> items = new List<OrderDetail>();

                od.UnitPrice = price;

                od.ProductId = item;
                od.Description = description;
                od.Quantity = quantity;
                od.OrderId = oid;
                //od.OrderDetailsId = oid;
                od.UnitPrice = price;
                od.barcodePrefix = barcodePrefix;
                od.barcodeSuffix = o.barcodeSuffix;
                od.startSequence = o.startSequence;
                od.endSequence = o.startSequence + quantity;


                //db.OrderDetails.Add(od);
                //db.SaveChanges();

            }

            decimal total = 0;

            foreach (var item in db.OrderDetails.Where(d => d.OrderId == o.OrderId))
            {

                total += (item.UnitPrice * item.Quantity) * 115 / 100;


            }
            var updateTotal = o.Quantity * o.UnitPrice;
            return RedirectToAction("updateOrderTotal", new { amount = total, oid = o.OrderId });


        }


        public ActionResult GetProductPrice(int id)
        {
            using (MainContext db = new MainContext())
            {
                var Prod = db.Products.Find(id);

                var result = new { Result = Prod };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { data = Prod }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult updateOrderTotal(decimal amount, int oid)
        {
            using (MainContext db = new MainContext())
            {
                var orderToUpdate = db.Orders.Find(oid);

                orderToUpdate.TotalAmount = amount;
                db.SaveChanges();


                return RedirectToAction("ShowAllOrders");
            }
        }


        public JsonResult GetOrdersList()
        {
            using (MainContext db = new MainContext())
            {

                var OrderList = db.Orders.Select(x => new Models.Order
                {

                    OrderId = x.OrderId,
                    CustomerId = x.CustomerId,
                    SaleRepId = x.SaleRepId,
                    OrderDate = x.OrderDate,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status,
                    PurchaseOrderDoc = x.PurchaseOrderDoc

                });




                return Json(OrderList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowAllOrders()
        {

            using (MainContext db = new MainContext())
            {
                foreach (var order in db.Orders)
                {
                    var oid = order.OrderId;
                    decimal grandTotal = 0;
                    var Total = order.TotalAmount;

                    var orderTotal = (from o in db.Orders where o.OrderId == oid select o.TotalAmount);

                    using (MainContext db2 = new MainContext())
                    {


                        foreach (var record in db2.OrderDetails)
                        {

                            var id = record.OrderId;
                            var up = record.UnitPrice;
                            var quant = record.Quantity;
                            var sumTotal = (up * quant);



                            if (id == oid)
                            {
                                grandTotal += sumTotal;

                            }
                        }
                    };

                    grandTotal.ToString("0.00");

                    order.TotalAmount = grandTotal;



                    grandTotal = 0;
                }


                db.SaveChanges();
            }

            ViewBag.Layout = "~/Views/Shared/_ManageOrders.cshtml";

            return View();
        }

        [HttpGet]
        public ActionResult ShowImages(int id)
        {
            using (MainContext db = new MainContext())
            {
                //var listofpic = ODB.Orders.ToList(); // Get List of images stored in FileTB table  
                var order = db.Orders.Find(id);
                return View(order);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        //Action to return database data
        public ActionResult loaddata()
        {
            MainContext db = new MainContext();
            {
                var data = db.Orders.Include(o => o.Customer).Include(o => o.SalesRep).ToList();

                string amt = String.Format("Order Total: {0:C}", data);

                foreach (var item in data)
                {

                    if (item.PurchaseOrderDoc != null && item.SignedLabelProofDoc != null && item.Invoice != null)
                    {
                        item.Status = 4;
                        item.StatusName = "✔️ Completed";
                        db.SaveChanges();
                    }
                    else if (item.PurchaseOrderDoc != null && item.SignedLabelProofDoc == null)
                    {
                        item.Status = 1;
                        item.StatusName = "📑 Need Label Proof";
                        db.SaveChanges();
                    }

                    else if (item.PurchaseOrderDoc == null && item.SignedLabelProofDoc != null)
                    {
                        item.Status = 1;
                        item.StatusName = "📑 Need Purchase Order";
                        db.SaveChanges();
                    }
                    else if (item.PurchaseOrderDoc != null && item.SignedLabelProofDoc != null && item.Invoice == null)
                    {
                        item.Status = 3;
                        item.StatusName = "💸 Ready for Invoice";
                    } 
                    else
                    {
                        item.Status = 0;
                        item.StatusName = "⌛️ Pending";
                    }
                }

                // other changed properties
                db.SaveChanges();



                return Json(new
                {

                    aaData = data.Select(x => new object[] { x.Customer.Name, x.SalesRep.FirstName, x.OrderDate.ToShortDateString(), x.PurchaseOrderNumber, "R " + (x.TotalAmount*114/100).ToString("0.00"), x.StatusName, "<a style='color: orange;' href='Edit/"
                                  + x.OrderId + "'>Edit</a>", "<a style='color: red;' href='Delete/"
                                  + x.OrderId + "'>Delete</a>", "<a style='color: blue;' href='fillInOrderDetails?orderId="
                                  + x.OrderId + "'>Details</a>", "<a href='/Orders/UploadPurchaseOrderDoc/" + x.OrderId + "' class='btn btn-default' style='width:100%;'>Attach</a>",
                    "<a href='/Orders/UploadProofDoc/" + x.OrderId + "' class='btn btn-default' style='width:100%;'>Attach</a>"})

                }, JsonRequestBehavior.AllowGet);








                //var data = db.Orders.Include(o => o.Customer).Include(o => o.SalesRep).ToList();

                //return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Orders
        //public ActionResult Index()
        //{
        //    var orders = db.Orders.Include(o => o.Customer).Include(o => o.SalesRep);
        //    return View(orders.ToList());
        //}

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);

        }

        // GET: Orders/Create
        public ActionResult Create()
        {

            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
            ViewBag.SaleRepId = new SelectList(db.SalesReps, "SalesRepId", "FirstName");


            return View();

        }

        //

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,SaleRepId,OrderDate,PurchaseOrderNumber,Status")] Models.Order order)
        {
            if (ModelState.IsValid)
            {
                if (order.PurchaseOrderDoc != null)
                {
                    order.Status = 1;

                    if (order.SignedLabelProofDoc != null)
                    {
                        order.Status = 2;
                    }
                    else
                    {
                        order.Status = 0;
                    }
                }

                switch (order.Status)
                {
                    case 0:
                        {
                            order.StatusName = "⌛️ Pending";
                            break;
                        }

                    case 1:
                        {
                            order.StatusName = "Need Label Proof";
                            break;
                        }

                    case 2:
                        {
                            order.StatusName = "In Progress";
                            break;
                        }

                    case 3:
                        {
                            order.StatusName = "Ready For Invoice";
                            break;
                        }

                    case 4:
                        {
                            order.StatusName = "Complete";
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }



                db.Orders.Add(order);
                db.SaveChanges();

            }

            var latestId = db.Orders.Max(p => p.OrderId);
            int? a = 0;

            var z = a == 0 ? 1 : 2;


            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.SaleRepId = new SelectList(db.SalesReps, "SalesRepId", "FirstName", order.SaleRepId);

            var x = $"customerID = {order.CustomerId}, salesRepID = {order.SaleRepId}";

            //int? oid, int? cid, int? sid, int? pid
            return RedirectToAction("fillInOrderDetails", new { orderId = latestId });

        }
        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.SaleRepId = new SelectList(db.SalesReps, "SalesRepId", "FirstName", order.SaleRepId);
            return View(order);

        }
        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,CustomerId,SaleRepId,OrderDate,PurchaseOrderNumber,TotalAmount,Status")] Models.Order order)
        {
            if (ModelState.IsValid)
            {

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", order.CustomerId);
            ViewBag.SaleRepId = new SelectList(db.SalesReps, "SalesRepId", "FirstName", order.SaleRepId);
            return View(order);

        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Models.Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Models.Order order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("showAllOrders");

        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


        public ActionResult UploadPurchaseOrderDoc(int id)
        {
            //search for the customer where customerId equals id
            var order = db.Orders.Find(id);

            return View(order);
        }

        //UPLOAD PURCHASE ORDER DOCUMENT TO DATABASE
        [HttpPost]
        public ActionResult UploadPurchaseOrderDoc(HttpPostedFileBase image, int id)
        {
            if (image != null)
            {
                var imageContent = new byte[image.ContentLength];
                image.InputStream.Read(imageContent, 0, imageContent.Length);

                var order = db.Orders.Find(id);
                order.PurchaseOrderDoc = imageContent;
                db.SaveChanges();

            }

            return RedirectToAction("showAllOrders");

        }

        public ActionResult UploadProofDoc(int id)
        {
            //search for the customer where customerId equals id
            var order = db.Orders.Find(id);

            return View(order);
        }

        //UPLOAD LABEL PROOF APPROVAL DOCUMENT TO DATABASE
        [HttpPost]
        public ActionResult UploadProofDoc(HttpPostedFileBase image, int id)
        {
            if (image != null)
            {
                var imageContent = new byte[image.ContentLength];
                image.InputStream.Read(imageContent, 0, imageContent.Length);

                var order = db.Orders.Find(id);
                order.SignedLabelProofDoc = imageContent;
                db.SaveChanges();

            }

            return RedirectToAction("showAllOrders");

        }

        public ActionResult OrdersReadyForInvoice()
        {
            ViewBag.Layout = "~/Views/Shared/_ManageOrders.cshtml";
            return View();
        }

        public ActionResult getReadyForInvoiceData()
        {
            //Get Orders that are ready for invoice


            var data = db.Orders.Include(o => o.Customer).Include(o => o.SalesRep).Where(s => s.Status == 3).ToList();

            string amt = String.Format("Order Total: {0:C}", data);

            return Json(new
            {

                aaData = data.Select(x => new object[] { x.Customer.Name, x.SalesRep.FirstName, x.OrderDate.ToShortDateString(), x.PurchaseOrderNumber, "R " + (x.TotalAmount*115/100).ToString("0.00"), x.Status, "<a class='btn btn-default' href='/Orders/fillInOrderDetails?orderId="
                                  + x.OrderId + "'>Details</a>", "<a class='btn btn-primary' href='/Orders/ViewPO/" + x.OrderId + "' style='width:100%;'>View PO</a>",
                    "<a href='/Orders/emailInvoice/" + x.OrderId + "' class='btn btn-success' style='width:100%;'>Email Invoice</a>"})

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ViewPO(int id)
        {
            //search for the order where orderId equals id
            var order = db.Orders.Find(id);

            return View(order);

        }

        public ActionResult emailInvoice(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
        //UPLOAD LABEL PROOF APPROVAL DOCUMENT TO DATABASE
        [HttpPost]
        public ActionResult emailInvoice(HttpPostedFileBase image, int id)
        {
            var order = db.Orders.Find(id);

            if (image != null)
            {
                var imageContent = new byte[image.ContentLength];
                image.InputStream.Read(imageContent, 0, imageContent.Length);


                order.Invoice = imageContent;
                db.SaveChanges();

                if (order.Invoice != null)
                {
                    order.Status = 4;
                    db.SaveChanges();
                }

            }

            return RedirectToAction("InvoiceConfirmation/" + id);

        }
        public ActionResult InvoiceConfirmation(int id)
        {
            var order = db.Orders.Find(id);

            return View(order);
        }
        public ActionResult AddItemsToOrder(int oid)
        {

            var order = db.Orders.Find(oid);

            OrderDetailVM odVM = new OrderDetailVM();

            odVM.OrderDetailsId = oid;
            odVM.OrderId = oid;

            //if (db.Products != null)
            //{
            //    foreach (Product p in db.Products)
            //    {

            //        odVM.ProductList.Add(p);
            //    }
            //}




            return View(odVM);

        }

        [HttpPost]
        public ActionResult AddItemsToOrder(OrderDetailVM detailsVM)
        {
            if (ModelState.IsValid)
            {

                var itemToAdd = new OrderDetail();

                var orderId = detailsVM.OrderId;
                var orderDetailId = detailsVM.OrderDetailsId;
                var itemId = detailsVM.Product.ProductId;
                var itemQuantity = detailsVM.Quantity;

                itemToAdd.OrderId = orderId;
                itemToAdd.OrderDetailsId = orderDetailId;
                itemToAdd.Product.ProductId = itemId;
                itemToAdd.Quantity = itemQuantity;

                db.SaveChanges();


            }

            return View();

        }


    }
}
