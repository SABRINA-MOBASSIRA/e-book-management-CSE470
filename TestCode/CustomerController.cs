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
    public class CustomerController : Controller
    {
        CheckAccessRoll car = new CheckAccessRoll();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customer
        public ActionResult Index()
        {
            //if (!car.CheckAccessPermission("Customer", "IsRead"))
            //{
            //    string URL = Request.UrlReferrer.ToString();
            //    Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
            //    return Redirect(URL);
            //}
            return View("Index");
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetCustomers()
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
                var data = ((from l in db.Customers
                             select new CustomerGridData
                             {
                                 Name = l.Name,
                                 PhoneNumber = l.PhoneNumber,
                                 Email = l.EmailAddress,
                                 Address = l.Address,
                                 Action = "<a href='/Customer/Details/" + l.CustomerID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Customer/Edit/" + l.CustomerID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Customer/Delete/" + l.CustomerID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.Name)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<CustomerGridData> searchData = new List<CustomerGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Name)) ? item.Name.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.PhoneNumber)) ? item.PhoneNumber.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Email)) ? item.Email.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Address)) ? item.Address.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<CustomerGridData>();
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
        private List<CustomerGridData> SortByColumnWithOrder(string order, string orderDir, List<CustomerGridData> data)
        {
            // Initialization.   
            List<CustomerGridData> lst = new List<CustomerGridData>();
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

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Customer customer = (from c in db.Customers where c.CustomerID == id select c).FirstOrDefault();
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            if (!car.CheckAccessPermission("Customer", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,Name,PhoneNumber,Address,EmailAddress")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                db.Customers.Add(customer);
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

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!car.CheckAccessPermission("Customer", "IsEdit"))
            {
                string URL = Request.UrlReferrer.ToString();
                Content("<script language='javascript' type='text/javascript'>alert('You have no access!');</script>");
                return Redirect(URL);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Customer customer = (from c in db.Customers where c.CustomerID == id select c).FirstOrDefault();
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,Name,PhoneNumber,Address,EmailAddress")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
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

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!car.CheckAccessPermission("Customer", "IsDelete"))
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
            Customer customer = (from c in db.Customers where c.CustomerID == id select c).FirstOrDefault();
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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

    public class CustomerGridData
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Action { get; set; }

    }
}
