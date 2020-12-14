using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EBM.Models;

namespace EBM.Controllers
{
    public class PrivilageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Privilage
        public ActionResult Index()
        {
            return View();
        }

        #region Grid Machenism
        [HttpPost]
        public JsonResult GetPrivilages()
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
                var data = ((from l in db.Privilages
                             select new PrivilageGridData
                             {
                                 Name = l.PrivilageName,
                                 Description = l.Description,
                                 Remarks = l.Remarks,
                                 Action = "<a href='/Privilage/Details/" + l.PrivilageID + "' class='btn btn-primary btn-xs'><i class='fa fa-folder'></i> View </a>" +
                                 "<a href='/Privilage/Edit/" + l.PrivilageID + "' class='btn btn-info btn-xs'><i class='fa fa-pencil'></i> Edit </a>" +
                                 "<a href='/Privilage/Delete/" + l.PrivilageID + "' class='btn btn-danger btn-xs'><i class='fa fa-trash - o'></i> Delete </a>"
                             }).OrderBy(l => l.Name)).ToList();
                // Total record count.   
                int totalRecords = data.Count;
                // Verification.   
                if (!string.IsNullOrEmpty(search) &&
                    !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    List<PrivilageGridData> searchData = new List<PrivilageGridData>();
                    foreach (var item in data)
                    {
                        if (((!string.IsNullOrEmpty(item.Name)) ? item.Name.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Description)) ? item.Description.ToString().ToLower().Contains(search.ToLower()) : false) ||
                        ((!string.IsNullOrEmpty(item.Remarks)) ? item.Remarks.ToString().ToLower().Contains(search.ToLower()) : false))
                        {
                            searchData.Add(item);
                        }
                    }
                    if (searchData.Count() > 0)
                    {
                        data = new List<PrivilageGridData>();
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
        private List<PrivilageGridData> SortByColumnWithOrder(string order, string orderDir, List<PrivilageGridData> data)
        {
            // Initialization.   
            List<PrivilageGridData> lst = new List<PrivilageGridData>();
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
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Description).ToList() : data.OrderBy(p => p.Description).ToList();
                        break;
                    case "2":
                        // Setting.   
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Remarks).ToList() : data.OrderBy(p => p.Remarks).ToList();
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

        // GET: Privilage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilage privilage = db.Privilages.Find(id);
            if (privilage == null)
            {
                return HttpNotFound();
            }
            return View(privilage);
        }

        // GET: Privilage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Privilage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrivilageID,PrivilageName,Description,Remarks,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] Privilage privilage)
        {
            if (ModelState.IsValid)
            {
                db.Privilages.Add(privilage);
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

        // GET: Privilage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilage privilage = db.Privilages.Find(id);
            if (privilage == null)
            {
                return HttpNotFound();
            }
            return View(privilage);
        }

        // POST: Privilage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrivilageID,PrivilageName,Description,Remarks,IsActive,Status,CreateBy,CreateOn,UpdateBy,UpdateOn,IsDeleted,DeleteBy,DeleteOn")] Privilage privilage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(privilage).State = EntityState.Modified;
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

        // GET: Privilage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privilage privilage = db.Privilages.Find(id);
            if (privilage == null)
            {
                return HttpNotFound();
            }
            return View(privilage);
        }

        // POST: Privilage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Privilage privilage = db.Privilages.Find(id);
            db.Privilages.Remove(privilage);
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

    public class PrivilageGridData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string Action { get; set; }

    }
}
