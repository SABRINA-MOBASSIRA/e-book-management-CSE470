using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Models;
using EBM.Helper;
using Microsoft.AspNet.Identity;

namespace EBM.Controllers
{
    public class SellerController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Seller
        public ActionResult Index()
        {
            //if (!car.CheckAccessPermission("Seller", "IsRead"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}
            return View("Index");
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetSellers()
        {
            // Initialization.   
            JsonResult result = new JsonResult();
            try
            {
                // Initialization.   
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                // Loading.
                string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var data = ((from l in db.Sellers
                             select new SellerGridData
                             {
                                 Name = l.Name,
                                 PhoneNumber = l.PhoneNumber,
                                 Email = l.EmailAddress,
                                 Action = "<a href='/Seller/Details/" + l.SellerID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Seller/Edit/" + l.SellerID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Seller/Delete/" + l.SellerID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.Name)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<SellerGridData> searchData = new List<SellerGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Name)) ? item.Name.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.PhoneNumber)) ? item.PhoneNumber.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Email)) ? item.Email.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<SellerGridData>();
                        data = searchData;
                    }
                }
                // Sorting.   
                data = this.SortByColumnWithOrder(order, orderDir, data);
                // Filter record count.   
                int recFilter = data.Count;
                // Apply pagination.   
                data = data.Skip(startRec).Take(pageSize).ToList();
                // Loading drop down lists.   
                result = this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info   
                Console.Write(ex);
            }
            // Return info.   
            return result;
        }

        /// <summary>  
        /// Sort by column with order method.   
        /// </summary>  
        /// <param name="order">Order parameter</param>  
        /// <param name="orderDir">Order direction parameter</param>  
        /// <param name="data">Data parameter</param>  
        /// <returns>Returns - Data</returns>  
        private List<SellerGridData> SortByColumnWithOrder(string order, string orderDir, List<SellerGridData> data)
        {
            // Initialization.   
            List<SellerGridData> lst = new List<SellerGridData>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Name).ToList() : data.OrderBy(p => p.Name).ToList();
                        break;
                    case "1":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.PhoneNumber).ToList() : data.OrderBy(p => p.PhoneNumber).ToList();
                        break;
                    case "2":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Email).ToList() : data.OrderBy(p => p.Email).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                // info.   
                Console.Write(ex);
            }
            // info.   
            return lst;
        }
        #endregion

        // GET: Seller/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Seller seller = (from c in db.Sellers where c.SellerID == id select c).FirstOrDefault();
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Seller/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Seller", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        // POST: Seller/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SellerID,Name,PhoneNumber,EmailAddress")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                db.Sellers.Add(seller);
                int res = db.SaveChanges();
                //return RedirectToAction("Index");
                if (res > 0)
                    return Json("success", JsonRequestBehavior.AllowGet);
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Seller/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Seller", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Seller seller = (from c in db.Sellers where c.SellerID == id select c).FirstOrDefault();
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Seller/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SellerID,Name,PhoneNumber,EmailAddress")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                int res = db.SaveChanges();
                //return RedirectToAction("Index");
                if (res > 0)
                    return Json("success", JsonRequestBehavior.AllowGet);
                else
                    return Json("error", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Seller/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Seller", "IsDelete"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Seller seller = (from c in db.Sellers where c.SellerID == id select c).FirstOrDefault();
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Seller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seller seller = db.Sellers.Find(id);
            db.Sellers.Remove(seller);
            int res = db.SaveChanges();
            //return RedirectToAction("Index");
            if (res > 0)
                return Json("success", JsonRequestBehavior.AllowGet);
            else
                return Json("error", JsonRequestBehavior.AllowGet);
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

    public class SellerGridData
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }

    }
}