using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarcodeTrackerWEB.Models;

namespace BarcodeTrackerWEB.Controllers
{
    public class SalesRepsController : Controller
    {
        private MainContext db = new MainContext();

        // GET: SalesReps
        public ActionResult Index()
        {
            return View(db.SalesReps.ToList());
        }

        // GET: SalesReps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesRep salesRep = db.SalesReps.Find(id);
            if (salesRep == null)
            {
                return HttpNotFound();
            }
            return View(salesRep);
        }

        // GET: SalesReps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SalesReps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SalesRepId,FirstName,LastName,email,phone")] SalesRep salesRep)
        {
            if (ModelState.IsValid)
            {
                db.SalesReps.Add(salesRep);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salesRep);
        }

        // GET: SalesReps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesRep salesRep = db.SalesReps.Find(id);
            if (salesRep == null)
            {
                return HttpNotFound();
            }
            return View(salesRep);
        }

        // POST: SalesReps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SalesRepId,FirstName,LastName,email,phone")] SalesRep salesRep)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesRep).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salesRep);
        }

        // GET: SalesReps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesRep salesRep = db.SalesReps.Find(id);
            if (salesRep == null)
            {
                return HttpNotFound();
            }
            return View(salesRep);
        }

        // POST: SalesReps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SalesRep salesRep = db.SalesReps.Find(id);
            db.SalesReps.Remove(salesRep);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
